using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Refs")]
    public CanvasGroupFader fader;
    public GameObject settingsPanel;

    [Header("Scene Names")]
    public string gameScene = "Game";
    public string shopScene = "Shop";
    public string podiumScene = "Podium";

    [Header("Modal Settings")]
    public ModalView settingsModal;

    void Start()
    {
        if (fader) fader.Instant(0f);
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    void Update()
    {
        // Botón "atrás" de Android: cierra ajustes o confirma salida del juego
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsModal != null && settingsModal.gameObject.activeSelf)
                settingsModal.Close();
            else
                Application.Quit();
        }
    }

    public void OnPlay()
    {
        AudioManager.I?.PlayClick();
        LoadScene(gameScene);
    }

    public void OnShop()
    {
        AudioManager.I?.PlayClick();
        LoadScene(shopScene);
    }

    public void OnSettings()
    {
        AudioManager.I?.PlayClick();
        settingsModal?.Open();
    }

    public void OnPodium()
    {
        AudioManager.I?.PlayClick();
        LoadScene(podiumScene);
    }

    public void OnCloseSettings()
    {
        AudioManager.I?.PlayClick();
        settingsModal?.Close();
    }

    void ToggleSettings(bool show)
    {
        if (settingsPanel) settingsPanel.SetActive(show);
    }

    async void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        if (fader) await fader.FadeTo(1f, 0.25f);
        SceneManager.LoadScene(sceneName);
    }
}
