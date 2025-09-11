using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [Header("Acción Move (Vector2) desde el .inputactions")]
    public InputActionReference moveAction;
    // Acción configurada en Input Actions que devuelve un Vector2.

    [Header("Movimiento")]
    public float speed = 3.5f;
    // Velocidad de movimiento del objeto.

    public bool moveInCameraPlane = true;
    // Si es true, el movimiento se calcula relativo a la cámara principal.

    void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
    }

    void Update()
    {
        if (moveAction == null) return;

        // Leemos el input en Vector2 (x, y)
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 dir = new Vector3(input.x, 0f, input.y);

        // Movimiento relativo a la cámara
        if (moveInCameraPlane && Camera.main)
        {
            Vector3 camFwd = Camera.main.transform.forward;
            camFwd.y = 0f;
            camFwd.Normalize();

            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            dir = camFwd * dir.z + camRight * dir.x;
        }

        // Aplicamos el movimiento
        transform.position += dir * speed * Time.deltaTime;
    }
}
