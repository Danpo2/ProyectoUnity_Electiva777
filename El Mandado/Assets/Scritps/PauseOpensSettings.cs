using UnityEngine;
using UnityEngine.UI;

public class PauseOpensSettings : MonoBehaviour
{
    [Header("Referencias")]
    public Button pauseButton;             // botón de pausa del HUD
    public GameObject settingsPanel;       // tu SettingsPanel_Game (prefab colocado en escena)
    public Button closeButton;             // botón Cerrar del panel (opcional si el panel ya llama a Close)

    [Header("Opcional: cerrar tocando afuera")]
    public Button backdropButton;          // el botón del fondo oscuro si lo tienes (opcional)

    bool paused;

    void Awake()
    {
        if (settingsPanel) settingsPanel.SetActive(false);

        if (pauseButton) pauseButton.onClick.AddListener(Open);
        if (closeButton) closeButton.onClick.AddListener(Close);
        if (backdropButton) backdropButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        if (paused) return;
        paused = true;

        if (settingsPanel) settingsPanel.SetActive(true);

        // Pausar juego
        Time.timeScale = 0f;

        // Si tu panel usa Animator/CanvasGroup para animar,
        // asegúrate que el Animator Update Mode sea UnscaledTime.
    }

    public void Close()
    {
        if (!paused) return;
        paused = false;

        if (settingsPanel) settingsPanel.SetActive(false);

        // Reanudar juego
        Time.timeScale = 1f;
    }
}
