using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System

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

    [Header("Gestures")]
    public bool logGestures = false;                         // para ver logs en consola
    Dictionary<int, Vector2> swipeStart = new();             // inicio por dedo

    


    [Header("Jump Animation")]
    public string jumpClipName = "Jump";       // nombre EXACTO del clip de salto
    public string canExitJumpParam = "CanExitJump"; // parámetro bool en Animator
    public bool lockJumpToClip = true;         // si true, el salto dura lo que el clip
    float jumpClipDuration = 0.5f;             // se detecta en Awake (fallback)
    bool canExitJump = true;                   // control interno
    float jumpTimer = 0f;

    [Header("Movimiento / Salto")]
    public float jumpForce = 12f;
    public float coyoteTime = 0.1f;            // margen tras dejar suelo
    public float jumpBufferTime = 0.1f;        // margen si pulsas justo antes de tocar suelo

    [Header("Crouch / Slide")]
    public float crouchHeightFactor = 0.6f;    // altura del collider al agacharse (60%)
    [Tooltip("Nombre EXACTO del CLIP de animación de deslizar (no el estado)")]
    public string slideClipName = "Crouch";
    [Tooltip("Exigir estar en el suelo para iniciar el slide")]
    public bool requireGroundForSlide = true;

    float slideDuration = 0.60f;               // se toma del clip en Awake; este es fallback
    bool slideActive = false;

    [Header("Swipe (New Input System)")]
    [Tooltip("Mínimo en píxeles para considerar swipe (0 = 6% de la altura de pantalla).")]
    public float minSwipePixels = 0f;
    [Tooltip("Si es true, exige que |deltaY| > |deltaX| para considerar el swipe vertical.")]
    public bool requireVerticalDominance = true;

    // estado
    bool isGrounded;
    bool isCrouching;
    float coyoteCounter;
    float jumpBufferCounter;

    // Landing guard
    bool wasGrounded = false;
    bool justLanded = false;

    // Si activaste el cruce forzado, apágalo para descartar reentradas indeseadas
    public bool forceCrossFadeJump = false;     // <- solo UNA vez en todo el script
    public string jumpStateNameForFade = "Jump";


    // colliders
    Vector2 colSizeOrig, colOffsetOrig;
    Vector2 colSizeCrouch, colOffsetCrouch;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        // Referencias básicas
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!bodyCollider) bodyCollider = GetComponent<BoxCollider2D>();
        if (!anim) anim = GetComponentInChildren<Animator>();

        // Guardar tamaño y offset originales del collider
        colSizeOrig = bodyCollider.size;
        colOffsetOrig = bodyCollider.offset;

        // Calcular tamaño y offset al agacharse
        colSizeCrouch = new Vector2(colSizeOrig.x, colSizeOrig.y * crouchHeightFactor);
        float offsetDelta = (colSizeOrig.y - colSizeCrouch.y) * 0.5f;
        colOffsetCrouch = colOffsetOrig + new Vector2(0f, -offsetDelta);

        // --- Detección automática de duración de clips ---
        if (anim && anim.runtimeAnimatorController != null)
        {
            foreach (var clip in anim.runtimeAnimatorController.animationClips)
            {
                if (clip == null) continue;

                // Duración del clip de deslizar (usa slideClipName)
                if (clip.name == slideClipName)
                {
                    slideDuration = clip.length;
                    // Debug.Log($"[Slide] Duración detectada: {slideDuration:F2}s del clip '{clip.name}'");
                }

                // Duración del clip de salto
                if (clip.name == jumpClipName)
                {
                    jumpClipDuration = clip.length;
                    // Debug.Log($"[Jump] Duración detectada: {jumpClipDuration:F2}s del clip '{clip.name}'");
                }
            }
        }
    }

    void Update()
    {
        if (Time.timeScale < 0.01f) return;

        // Actualiza grounded
        CheckGround();
        // Detectar aterrizaje (cambio de false -> true)
        justLanded = isGrounded && !wasGrounded;
        wasGrounded = isGrounded;

        // Si acaba de aterrizar, limpia cualquier trigger y buffer de salto
        if (justLanded)
        {
            jumpBufferCounter = 0f;
            if (anim) anim.ResetTrigger("Jump");
        }


        // Timers de coyote y buffer
        if (isGrounded) coyoteCounter = coyoteTime;
        else coyoteCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f) jumpBufferCounter -= Time.deltaTime;

        // Si hay buffer y ahora estoy grounded → salto
        if (jumpBufferCounter > 0f && isGrounded)
        {
            DoJump();
            jumpBufferCounter = 0f;
        }

        // Control del lock de salto (duración del clip)
        if (lockJumpToClip && !canExitJump)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f)
            {
                canExitJump = true;
                anim?.SetBool(canExitJumpParam, true); // liberar salida de Jump
            }
        }

        // Entradas
        HandleKeyboardNewInput();
        HandleTouchNewInput();

        // Animator params
        if (anim)
        {
            anim.SetBool("Grounded", isGrounded);
            anim.SetFloat("yVel", rb.linearVelocity.y);
            anim.SetBool("Crouch", isCrouching);
        }
    }

    void CheckGround()
    {
        if (!groundCheck) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }

    // ===== ENTRADAS (New Input System) =====
    void HandleKeyboardNewInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.upArrowKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame)
            TryJump();

        if (kb.downArrowKey.wasPressedThisFrame || kb.sKey.wasPressedThisFrame)
            StartCrouch();

        if (kb.downArrowKey.wasReleasedThisFrame || kb.sKey.wasReleasedThisFrame)
            StopCrouch(); // no corta si slideActive = true
    }

    void HandleTouchNewInput()
    {
        var ts = Touchscreen.current;
        if (ts == null) return;

        var touches = ts.touches;
        for (int i = 0; i < touches.Count; i++)
        {
            var t = touches[i];

            bool down = t.press.wasPressedThisFrame;
            bool up = t.press.wasReleasedThisFrame;

            if (down)
            {
                Vector2 start = t.position.ReadValue();
                swipeStart[i] = start;
                if (logGestures) Debug.Log($"[Touch] Began id={i} start={start}");
            }

            if (up)
            {
                if (!swipeStart.TryGetValue(i, out var start)) continue;

                Vector2 end = t.position.ReadValue();
                Vector2 delta = end - start;

                float min = (minSwipePixels > 0f) ? minSwipePixels : Screen.height * 0.06f;
                if (logGestures) Debug.Log($"[Touch] End id={i} delta={delta} min={min}");

                bool verticalOK = !requireVerticalDominance || Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
                if (verticalOK)
                {
                    if (delta.y >= min) { if (logGestures) Debug.Log("[Swipe] ↑ detectado → Jump"); TryJump(); }
                    else if (delta.y <= -min) { if (logGestures) Debug.Log("[Swipe] ↓ detectado → Slide"); StartSlide(); }
                }

                swipeStart.Remove(i);
            }
        }
    }

    // ===== SALTO =====
    void TryJump()
    {
        // Evita consumir buffer/trigger justo en el frame de aterrizaje
        if (justLanded) return;

        if (isGrounded || coyoteCounter > 0f)
            DoJump();
        else
            jumpBufferCounter = jumpBufferTime; // solo si realmente estás en el aire
    }


    void DoJump()
    {
        if (slideActive) return;
        if (!isGrounded && coyoteCounter <= 0f) return;

        if (isCrouching)
        {
            if (CanStandUp()) StopCrouch(); else return;
        }

        // Limpia Y y aplica impulso
        var v = rb.linearVelocity; v.y = 0f; rb.linearVelocity = v;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // Dispara animación y bloquea salida (el Behaviour la liberará al final del clip)
        if (anim)
        {
            anim.SetBool(canExitJumpParam, false);
            anim.ResetTrigger("Jump");
            anim.SetTrigger("Jump");

            // Mejor dejar esto en false mientras depuras
            if (forceCrossFadeJump)
                anim.CrossFadeInFixedTime(jumpStateNameForFade, 0.05f, 0);
        }

        // Resetea contadores
        coyoteCounter = 0f;
        jumpBufferCounter = 0f;
    }




    // ===== CROUCH / SLIDE =====
    public void StartSlide()
    {
        if (slideActive) return;
        if (requireGroundForSlide && !isGrounded) return;

        StartCoroutine(CoSlide());
    }

    IEnumerator CoSlide()
    {
        slideActive = true;
        StartCrouch();

        float t = 0f;
        float dur = slideDuration > 0f ? slideDuration : 0.6f; // fallback
        while (t < dur)
        {
            t += Time.deltaTime;
            yield return null;
        }

        slideActive = false;
        StopCrouch();
    }

    void StartCrouch()
    {
        if (isCrouching) return;
        isCrouching = true;
        ApplyCrouchCollider(true);
        anim?.SetBool("Crouch", true);
    }

    void StopCrouch()
    {
        if (!isCrouching) return;
        if (slideActive) return;          // no cortar el slide fijo
        if (!CanStandUp()) return;        // respeta techo
        isCrouching = false;
        ApplyCrouchCollider(false);
        anim?.SetBool("Crouch", false);
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
}
