using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GestureControllerUnified : MonoBehaviour
{
    public Transform target;     // objeto a rotar (1 dedo) y a mover en Y (2 dedos)
    public Camera cam;           // cámara para zoom

    [Header("Rotación (1 dedo)")]
    public float rotateSpeed = 0.2f;   // grados por píxel
    public bool horizontalOnly = true; // true: solo yaw

    [Header("Mover en Y (2 dedos)")]
    public float yMoveSpeed = 0.01f;   // unidades por píxel vertical
    public float moveThreshold = 3f;   // px mínimos para considerar "mover en Y"

    [Header("Zoom (2 dedos)")]
    public float zoomSpeed = 0.15f;    // factor de sensibilidad
    public float fovMin = 30f, fovMax = 75f;    // si cam en perspectiva
    public float orthoMin = 2f, orthoMax = 20f; // si cam ortográfica
    public float pinchThreshold = 3f;  // px de cambio de distancia para considerar "pinch"

    [Header("Desambiguación")]
    [Tooltip("Para decidir gesto dominante: pinchDelta debe superar dominanceRatio * |avgMoveY| o viceversa.")]
    public float dominanceRatio = 1.5f;

    private enum TwoFingerMode { None, PinchZoom, PanY }
    private TwoFingerMode mode = TwoFingerMode.None;

    private Vector2? lastSinglePos;
    private Vector2? lastTwoAvg;
    private float? lastTwoDistance;

    void OnEnable() { EnhancedTouchSupport.Enable(); TouchSimulation.Enable(); }
    void OnDisable() { TouchSimulation.Disable(); EnhancedTouchSupport.Disable(); }

    void Update()
    {
        var touches = Touch.activeTouches;
        if (touches.Count == 0)
        {
            ResetTwoFingerState();
            lastSinglePos = null;
            return;
        }

        // 1 dedo: Rotación
        if (touches.Count == 1)
        {
            var p = touches[0].screenPosition;
            if (lastSinglePos == null) lastSinglePos = p;
            else
            {
                var d = p - lastSinglePos.Value;
                lastSinglePos = p;

                if (target)
                {
                    if (horizontalOnly) target.Rotate(0f, d.x * rotateSpeed, 0f, Space.World);
                    else target.Rotate(-d.y * rotateSpeed, d.x * rotateSpeed, 0f, Space.World);
                }
            }
            // limpiar estado de 2 dedos
            ResetTwoFingerState();
            return;
        }

        // 2 dedos o más: decidir entre PinchZoom vs PanY
        var p0 = touches[0].screenPosition;
        var p1 = touches[1].screenPosition;
        var avg = (p0 + p1) * 0.5f;
        float dist = Vector2.Distance(p0, p1);

        // inicializar referencias
        if (lastTwoAvg == null) lastTwoAvg = avg;
        if (lastTwoDistance == null) lastTwoDistance = dist;

        // deltas de este frame
        float avgMoveY = avg.y - lastTwoAvg.Value.y;
        float pinchDelta = dist - lastTwoDistance.Value;

        // si aún no hay modo bloqueado, elegir dominante
        if (mode == TwoFingerMode.None)
        {
            bool pinchCandidate = Mathf.Abs(pinchDelta) >= pinchThreshold
                                  && Mathf.Abs(pinchDelta) > Mathf.Abs(avgMoveY) * dominanceRatio;

            bool panYCandidate = Mathf.Abs(avgMoveY) >= moveThreshold
                                 && Mathf.Abs(avgMoveY) > Mathf.Abs(pinchDelta) * dominanceRatio;

            if (pinchCandidate) mode = TwoFingerMode.PinchZoom;
            else if (panYCandidate) mode = TwoFingerMode.PanY;
            // si ninguno supera umbral/dominancia, aún no hacemos nada (esperamos señal más clara)
        }
        else
        {
            // ejecutar solo el gesto bloqueado
            if (mode == TwoFingerMode.PinchZoom)
            {
                if (cam)
                {
                    if (cam.orthographic)
                    {
                        cam.orthographicSize = Mathf.Clamp(
                            cam.orthographicSize - pinchDelta * zoomSpeed * Time.deltaTime,
                            orthoMin, orthoMax
                        );
                    }
                    else
                    {
                        cam.fieldOfView = Mathf.Clamp(
                            cam.fieldOfView - pinchDelta * zoomSpeed * 10f * Time.deltaTime,
                            fovMin, fovMax
                        );
                    }
                }
            }
            else if (mode == TwoFingerMode.PanY && target)
            {
                target.position += new Vector3(0f, avgMoveY * yMoveSpeed, 0f);
            }
        }

        // actualizar referencias para el próximo frame
        lastTwoAvg = avg;
        lastTwoDistance = dist;

        // con 2+ dedos, no rotamos con 1 dedo
        lastSinglePos = null;

        // cuando se sueltan dedos (se maneja arriba con touches.Count == 0), modo vuelve a None
    }

    private void ResetTwoFingerState()
    {
        mode = TwoFingerMode.None;
        lastTwoAvg = null;
        lastTwoDistance = null;
    }
}
