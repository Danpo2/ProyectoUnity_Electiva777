using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using TMPro;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MultiTouchInfo : MonoBehaviour
{
    public TextMeshProUGUI multiTouchInfo;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    void OnDisable()
    {
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        var touches = Touch.activeTouches;
        if (multiTouchInfo == null) return;

        if (touches.Count == 0)
        {
            multiTouchInfo.text = "Sin toques";
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"Toques: {touches.Count}");
        for (int i = 0; i < touches.Count; i++)
        {
            var t = touches[i];
            sb.AppendLine(
                $"#{i} id={t.touchId} pos={t.screenPosition} phase={t.phase}"
            );
        }
        multiTouchInfo.text = sb.ToString();
    }
}
