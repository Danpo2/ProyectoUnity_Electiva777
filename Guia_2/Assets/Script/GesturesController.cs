using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronGolemGestures : MonoBehaviour
{
    [Header("Rotación (1 dedo) - Solo Y (360°)")]
    public float rotationSpeed = 0.12f;
    public float rotationDeadZone = 0.5f;
    private float yawDeg;
    private float baseEulerX, baseEulerZ;

    [Header("Gestos 2 dedos")]
    public float pinchThresholdPx = 12f;   // decide pinch
    public float panThresholdPx = 8f;      // decide pan Y
    public float sameDirMinDot = 0.7f;     // paralelismo dedos para pan

    [Header("Mover en Y (2 dedos drag)")]
    public float moveYSensitivity = 0.1f;

    [Header("Límites opcionales en Y")]
    public bool clampY = false;
    public float minY = -2f, maxY = 2f;

    private enum TwoFingerMode { None, Unknown, Pinch, PanY }
    private TwoFingerMode twoMode = TwoFingerMode.None;

    // Métricas 2 dedos
    private float initialDistance, prevDistance;
    private Vector2 initialMidpoint, prevMidpoint;

    void Start()
    {
        Vector3 e = transform.localEulerAngles;
        yawDeg = e.y;
        baseEulerX = e.x;
        baseEulerZ = e.z;
    }

    void Update()
    {
        int n = Input.touchCount;

        // === 1 dedo: ROTACIÓN Y (360° continuo) ===
        if (n == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved && Mathf.Abs(t.deltaPosition.x) > rotationDeadZone)
            {
                yawDeg += -t.deltaPosition.x * rotationSpeed;
                yawDeg = Mathf.Repeat(yawDeg, 360f);
                transform.localRotation = Quaternion.Euler(baseEulerX, yawDeg, baseEulerZ);
            }
            if (twoMode != TwoFingerMode.None) twoMode = TwoFingerMode.None;
            return;
        }

        // === 2+ dedos: PINCH o PAN Y (bloqueado) ===
        if (n >= 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            float dist = Vector2.Distance(t0.position, t1.position);
            Vector2 midpoint = (t0.position + t1.position) * 0.5f;

            if (twoMode == TwoFingerMode.None || t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                twoMode = TwoFingerMode.Unknown;
                initialDistance = prevDistance = dist;
                initialMidpoint = prevMidpoint = midpoint;
            }

            if (twoMode == TwoFingerMode.Unknown && (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved))
            {
                float distDelta = Mathf.Abs(dist - initialDistance);
                Vector2 d0 = t0.deltaPosition, d1 = t1.deltaPosition;
                float sameDir = (d0.sqrMagnitude > 0.001f && d1.sqrMagnitude > 0.001f)
                                ? Vector2.Dot(d0.normalized, d1.normalized) : 0f;
                float midDy = Mathf.Abs(midpoint.y - initialMidpoint.y);

                if (distDelta > pinchThresholdPx) twoMode = TwoFingerMode.Pinch;
                else if (midDy > panThresholdPx && sameDir > sameDirMinDot) twoMode = TwoFingerMode.PanY;
            }

            if (twoMode == TwoFingerMode.Pinch)
            {
                // ZOOM ILIMITADO (sin clamp). Protección mínima contra colapso a 0.
                if (prevDistance > 0.001f)
                {
                    float factor = dist / prevDistance;
                    Vector3 s = transform.localScale * factor;
                    float u = s.x < 1e-6f ? 1e-6f : s.x; // evita 0 o valores negativos
                    transform.localScale = new Vector3(u, u, u);
                }
                prevDistance = dist;
            }
            else if (twoMode == TwoFingerMode.PanY)
            {
                float dy = (midpoint.y - prevMidpoint.y) * moveYSensitivity;
                if (Mathf.Abs(dy) > 0.00005f)
                {
                    transform.Translate(0f, dy, 0f, Space.World);
                    if (clampY)
                    {
                        var p = transform.position;
                        p.y = Mathf.Clamp(p.y, minY, maxY);
                        transform.position = p;
                    }
                }
                prevMidpoint = midpoint;
            }
            return;
        }

        if (n == 0 && twoMode != TwoFingerMode.None) twoMode = TwoFingerMode.None;
    }
}