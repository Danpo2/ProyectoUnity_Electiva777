using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
// Importamos soporte de gestos multitouch
// Alias para usar `Touch` directamente

[RequireComponent(typeof(Camera))]
public class PinchZoom : MonoBehaviour
{

    public float zoomSpeed = 0.15f;
    public float fovMin = 30f;
    public float fovMax = 75f;
    public float orthoMin = 2f;
    public float orthoMax = 20f;
    private Camera cam;
    private float? lastDistance;

// Velocidad de respuesta del zoom (qu tan rpido cambia)
// Valor manimo de Field of View (camara en perspectiva)
// Valor maximo de Field of View (camara en perspectiva)
// Valor manimo para cmaras ortograficas (usada
    void OnEnable()
    {
        cam = GetComponent<Camera>();
        EnhancedTouchSupport.Enable();
        // Activamos el sistema de Input avanzado para multitouch
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        var touches = Touch.activeTouches;
        // Obtenemos todos los toques activos en la pantalla

        if (touches.Count < 2)
        {
            // Si hay menos de dos dedos, reiniciamos
            lastDistance = null;
            return;
        }

        // Tomamos los dos primeros dedos
        Vector2 p0 = touches[0].screenPosition;
        Vector2 p1 = touches[1].screenPosition;

        // Distancia actual entre dedos
        float currentDistance = Vector2.Distance(p0, p1);

        if (lastDistance.HasValue)
        {
            float delta = currentDistance - lastDistance.Value;
            // Positivo = los dedos se alejan, Negativo = se acercan

            float zoomAmount = -delta * zoomSpeed * Time.deltaTime;

            if (cam.orthographic)
            {
                // Cámara ortográfica (2D): modificamos el tamaño
                cam.orthographicSize = Mathf.Clamp(
                    cam.orthographicSize + zoomAmount,
                    orthoMin,
                    orthoMax
                );
            }
            else
            {
                // Cámara en perspectiva (3D): modificamos el Field of View
                cam.fieldOfView = Mathf.Clamp(
                    cam.fieldOfView + zoomAmount * 10f,
                    fovMin,
                    fovMax
                );
            }
        }

        // Guardamos la distancia actual como referencia para el próximo frame
        lastDistance = currentDistance;
    }
}