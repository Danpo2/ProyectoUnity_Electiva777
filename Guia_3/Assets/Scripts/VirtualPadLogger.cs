using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class VirtualPadLogger : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAction; // Player/Move

    [Header("UI")]
    public TextMeshProUGUI hudText;

    [Header("Filtrado")]
    [Range(0f, 1f)] public float deadzone = 0.15f;  // ignora micro-movimientos
    public float cardinalThreshold = 0.65f;         // para decir Up/Down/Left/Right
    public float diagThreshold = 0.35f;             // diagonales

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
        if (hudText == null || moveAction == null) return;

        Vector2 v = moveAction.action.ReadValue<Vector2>();
        float mag = v.magnitude;

        string dir = "Idle";
        if (mag > deadzone)
        {
            // Dirección cardinal/diagonal (como “registrar dirección” en Guía 2)
            float x = v.x;
            float y = v.y;

            // Cardinales dominantes
            if (Mathf.Abs(x) >= cardinalThreshold && Mathf.Abs(y) < diagThreshold)
                dir = x > 0f ? "Right" : "Left";
            else if (Mathf.Abs(y) >= cardinalThreshold && Mathf.Abs(x) < diagThreshold)
                dir = y > 0f ? "Up" : "Down";
            else
            {
                // Diagonales
                if (x > 0 && y > 0) dir = "Up-Right";
                else if (x < 0 && y > 0) dir = "Up-Left";
                else if (x < 0 && y < 0) dir = "Down-Left";
                else if (x > 0 && y < 0) dir = "Down-Right";
            }
        }

        float angle = mag > deadzone ? Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg : 0f;

        hudText.text =
            $"Move: {v:0.00}\n" +
            $"Mag:  {mag:0.00}\n" +
            $"Dir:  {dir}\n" +
            $"Angle:{angle:0.0}°";
    }
}
