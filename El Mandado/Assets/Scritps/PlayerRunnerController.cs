using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerRunnerController : MonoBehaviour
{
    [Header("Refs")]
    public Animator anim;
    public Rigidbody2D rb;
    public BoxCollider2D bodyCollider;

    [Header("Grounding")]
    public Transform groundCheck;
    public float groundRadius = 0.12f;
    public LayerMask groundMask;

    [Header("Ceiling (para levantarse del crouch)")]
    public Transform ceilingCheck;
    public float ceilingRadius = 0.12f;

    [Header("Movimiento / Salto")]
    public float jumpForce = 12f;
    public float coyoteTime = 0.1f;       // margen tras dejar el suelo
    public float jumpBufferTime = 0.1f;   // margen si presionas justo antes de tocar suelo

    [Header("Crouch / Slide")]
    public float crouchHeightFactor = 0.6f;
    [UnityEngine.Serialization.FormerlySerializedAs("crouchSwipeDuration")]
    public float slideDuration = 0.60f;   // duración fija del slide al hacer swipe abajo
    bool slideActive = false;             // true mientras dure el slide por swipe

    [Header("Swipe")]
    [Tooltip("Proporción de la altura de pantalla que debe superar el swipe (0.08 = 8%)")]
    public float minSwipeHeightRatio = 0.08f;

    bool isGrounded;
    bool isCrouching;
    float coyoteCounter;
    float jumpBufferCounter;

    Vector2 colSizeOrig, colOffsetOrig;
    Vector2 colSizeCrouch, colOffsetCrouch;

    Vector2 touchStartPos;
    bool touchActive;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!bodyCollider) bodyCollider = GetComponent<BoxCollider2D>();
        if (!anim) anim = GetComponentInChildren<Animator>();

        colSizeOrig   = bodyCollider.size;
        colOffsetOrig = bodyCollider.offset;

        colSizeCrouch   = new Vector2(colSizeOrig.x, colSizeOrig.y * crouchHeightFactor);
        float offsetDelta = (colSizeOrig.y - colSizeCrouch.y) * 0.5f;
        colOffsetCrouch  = colOffsetOrig + new Vector2(0f, -offsetDelta);
    }

    void Update()
    {
        // No leer entradas ni animar si el juego está en pausa
        if (Time.timeScale < 0.01f) return;

        CheckGround();
        HandleKeyboard();
        HandleTouch();

        // Timers de coyote y buffer
        if (isGrounded) coyoteCounter = coyoteTime;
        else            coyoteCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f) jumpBufferCounter -= Time.deltaTime;

        // Si hubo buffer y ahora estoy grounded → salto
        if (jumpBufferCounter > 0f && isGrounded)
        {
            DoJump();
            jumpBufferCounter = 0f;
        }

        // Animator params
        if (anim)
        {
            anim.SetBool("Grounded", isGrounded);
            anim.SetBool("Crouch",   isCrouching);
            anim.SetFloat("yVel",    GetVelocity().y);
        }
    }

    void CheckGround()
    {
        if (!groundCheck) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }

    void HandleKeyboard()
    {
        // Saltar: ↑ / W
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            TryJump();
        }

        // Agacharse con teclado (mantener ↓ / S). No afecta el slide fijo.
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            StartCrouch();
        }
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            StopCrouch(); // si hay slide activo, no se levantará (ver guardia abajo)
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);

        // Ignora toques sobre UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId))
            return;

        switch (t.phase)
        {
            case TouchPhase.Began:
                touchActive = true;
                touchStartPos = t.position;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (!touchActive) break;
                touchActive = false;

                Vector2 end = t.position;
                Vector2 delta = end - touchStartPos;

                float minSwipe = Screen.height * minSwipeHeightRatio;
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) && Mathf.Abs(delta.y) >= minSwipe)
                {
                    if (delta.y > 0f)      TryJump();   // swipe arriba → salto
                    else                   StartSlide(); // swipe abajo → slide de tiempo fijo
                }
                break;
        }
    }

    void TryJump()
    {
        // Puedes saltar si estás en el suelo o dentro de coyote
        if (isGrounded || coyoteCounter > 0f)
        {
            DoJump();
        }
        else
        {
            // Guarda intención de salto por si aterrizas en breve
            jumpBufferCounter = jumpBufferTime;
        }
    }

    void DoJump()
    {
        // Si estabas agachado, intenta levantarte antes de saltar (si hay espacio)
        if (isCrouching)
        {
            if (CanStandUp()) StopCrouch();
            else return; // no saltes si no puedes levantarte
        }

        Vector2 v = GetVelocity();
        v.y = 0f;                 // reset vertical para saltos consistentes
        SetVelocity(v);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        coyoteCounter = 0f;
        jumpBufferCounter = 0f;

        anim?.SetTrigger("Jump");
    }

    // ---- Crouch / Slide ----

    public void StartSlide()
    {
        // Sólo desliza si estás en el suelo; si no, ignora
        if (!isGrounded) return;

        // Si ya estás deslizando, no reiniciar (cámbialo si quieres reinicio)
        if (slideActive) return;

        StartCoroutine(CoSlide());
    }

    IEnumerator CoSlide()
    {
        slideActive = true;
        StartCrouch(); // activa el estado agachado (collider + anim)

        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime; // se “pausa” si Time.timeScale=0
            yield return null;
        }

        slideActive = false;
        StopCrouch(); // intenta levantarse (respeta techo)
    }

    void StartCrouch()
    {
        if (isCrouching) return;
        isCrouching = true;
        ApplyCrouchCollider(true);
    }

    void StopCrouch()
    {
        if (!isCrouching) return;

        // Si está activo un slide de duración fija, no permitir levantarse
        if (slideActive) return;

        if (!CanStandUp()) return; // sigue agachado si hay techo
        isCrouching = false;
        ApplyCrouchCollider(false);
    }

    bool CanStandUp()
    {
        if (!ceilingCheck) return true;
        var hit = Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, groundMask);
        return hit == null;
    }

    void ApplyCrouchCollider(bool crouch)
    {
        if (crouch)
        {
            bodyCollider.size = colSizeCrouch;
            bodyCollider.offset = colOffsetCrouch;
        }
        else
        {
            bodyCollider.size = colSizeOrig;
            bodyCollider.offset = colOffsetOrig;
        }
    }

    // ---- Gizmos ----
    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
        if (ceilingCheck)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingRadius);
        }
    }

    // ===== Helpers de compatibilidad: velocity vs linearVelocity =====
    Vector2 GetVelocity()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }
    void SetVelocity(Vector2 v)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }
}
