using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class MenuManager : MonoBehaviour
{
    [Header("Refs")]
    public CanvasGroupFader fader;
    public GameObject settingsPanel;
    public TMP_InputField nameInput;           // Referencia al input de nombre
    public TextMeshProUGUI feedback;           // Referencia al feedback textual


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

        // Recupera el nombre guardado si existe
        string nombre = PlayerPrefs.GetString("PlayerName", "");
        if (nameInput) nameInput.text = nombre;
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

        string playerName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            // Muestra advertencia en feedback y NO inicia escena
            if (feedback) feedback.text = "Pon tu nombre primero";
            return;
        }

        // Guarda el nombre antes de cargar la escena
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        feedback.text = "Nombre guardado";

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
    public PodiumModal podiumModal;
    public PodiumFetcher podiumFetcher; // Arrastra en el inspector

    public void OnPodium()
    {
        Debug.Log("[Menu] OnPodium clickeado");
        AudioManager.I?.PlayClick();
        StartCoroutine(podiumFetcher.FetchPodiumDataCoroutine());
    }






}
