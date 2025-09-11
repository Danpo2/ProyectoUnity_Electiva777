using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using TMPro;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchPhaseManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI timerText;

    private float phaseTimer = 0f;
    private string currentPhase = "No Touch";

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable(); // Opcional: simular con mouse en Editor
        phaseTimer = 0f;
        currentPhase = "No Touch";
        UpdateUI();
    }

    void OnDisable()
    {
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        var touches = Touch.activeTouches;

        if (touches.Count == 0)
        {
            UpdatePhase("No Touch");
        }
        else
        {
            // Tomamos el primer toque activo
            var t = touches[0];

            // Mapeo simple de fases
            string phase =
                t.phase == UnityEngine.InputSystem.TouchPhase.Began ? "Began" :
                t.phase == UnityEngine.InputSystem.TouchPhase.Moved ? "Moved" :
                t.phase == UnityEngine.InputSystem.TouchPhase.Stationary ? "Stationary" :
                t.phase == UnityEngine.InputSystem.TouchPhase.Ended ? "Ended" :
                t.phase == UnityEngine.InputSystem.TouchPhase.Canceled ? "Canceled" :
                "Unknown";

            UpdatePhase(phase);
        }

        phaseTimer += Time.deltaTime;
        if (timerText) timerText.text = $"Tiempo en fase: {phaseTimer:0.00}s";
    }

    private void UpdatePhase(string newPhase)
    {
        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            phaseTimer = 0f;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (phaseText) phaseText.text = $"Fase: {currentPhase}";
    }
}
