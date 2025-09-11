using UnityEngine;
using UnityEngine.InputSystem;

public class TapHandler : MonoBehaviour
{
    [Header("Arrastrar desde el .inputactions")]
    public InputActionReference tapAction;
    // Acción que detecta el toque (Tap). Se configura en el asset de Input Actions.

    public InputActionReference pointerPositionAction;
    // Acción que nos da la posición del dedo (o mouse) en la pantalla.

    [Header("Opcional!")]
    public Camera cam;
    // Cámara a usar para convertir la posición en pantalla a un rayo.
    // Si no se asigna, usará Camera.main.

    public LayerMask raycastLayers = ~0;
    // Permite filtrar en qué capas queremos detectar el toque.
    // ~0 significa todas las capas.

    void OnEnable()
    {
        // Se ejecuta cuando el objeto se activa en la escena.
        if (tapAction != null)
        {
            tapAction.action.performed += OnTap;
            tapAction.action.Enable();
        }

        if (pointerPositionAction != null)
            pointerPositionAction.action.Enable();
    }

    void OnDisable()
    {
        // Se ejecuta cuando el objeto se desactiva o destruye.
        if (tapAction != null)
        {
            tapAction.action.performed -= OnTap;
            tapAction.action.Disable();
        }

        if (pointerPositionAction != null)
            pointerPositionAction.action.Disable();
    }

    private void OnTap(InputAction.CallbackContext ctx)
    {
        // Este método se ejecuta cada vez que ocurre un "tap" en la pantalla.
        Camera cameraToUse = cam != null ? cam : Camera.main;
        if (cameraToUse == null) return;

        // Leemos la posición del toque en coordenadas de pantalla (pixeles).
        Vector2 screenPos = pointerPositionAction.action.ReadValue<Vector2>();

        // Creamos un rayo desde la cámara hacia donde el usuario tocó.
        Ray ray = cameraToUse.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, raycastLayers))
        {
            // Si el rayo golpea un objeto en la escena dentro de 1000 unidades
            Debug.Log("Tap sobre: " + hit.collider.gameObject.name);

            // Ejemplo: crear una esfera en el punto tocado
            // GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = hit.point;
        }
        else
        {
            Debug.Log("Tap en vacío (no golpeó nada).");
        }
    }
}
