using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDGameUI : MonoBehaviour
{
    public static HUDGameUI I;

    [Header("UI")]
    public Button pauseButton;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI coinsText;
    public GameObject pauseOverlay;      // Contiene Backdrop + Panel + BtnResume
    public Button resumeButton;          // Asigna el botón “Reanudar” del overlay

    [Header("Iconos (opcional)")]
    public Sprite pauseIcon;
    public Sprite playIcon;              // no se usa si reanudamos solo con botón
    public Image pauseButtonImage;

    int coins;
    bool isPaused;
    float elapsed;
    float uiUpdateClock;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        Time.timeScale = 1f;
        isPaused = false;
        coins = 0;
        elapsed = 0f;

        if (pauseButton) pauseButton.onClick.AddListener(Pause);
        if (resumeButton) resumeButton.onClick.AddListener(Resume);

        UpdateCoinsUI();
        UpdateTimeUI(force: true);

        if (pauseOverlay) pauseOverlay.SetActive(false);
        SetPauseIcon();
    }

    void Update()
    {
        // Botón Atrás: si está pausado, reanuda; si no, pausa.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }

        if (isPaused) return;

        elapsed += Time.deltaTime;

        uiUpdateClock += Time.deltaTime;
        if (uiUpdateClock >= 0.1f)
        {
            uiUpdateClock = 0f;
            UpdateTimeUI();
        }
    }

    // ----- API pública -----
    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseOverlay) pauseOverlay.SetActive(true);
        if (pauseButton) pauseButton.interactable = false; // forzamos reanudar solo desde el overlay

        SetPauseIcon();
        // Opcional: AudioManager.I?.music?.Pause();
        // Opcional: Haptic leve al pausar
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseOverlay) pauseOverlay.SetActive(false);
        if (pauseButton) pauseButton.interactable = true;

        SetPauseIcon();
        // Opcional: reanudar música si estaba activa
        // if (AudioManager.I?.MusicOn == true) AudioManager.I.music.Play();
    }

    public void AddCoins(int amount)
    {
        coins = Mathf.Max(0, coins + amount);
        UpdateCoinsUI();
    }

    public void ResetRun()
    {
        coins = 0;
        elapsed = 0f;
        UpdateCoinsUI();
        UpdateTimeUI(force: true);
        if (isPaused) Resume();
    }

    // ----- Helpers -----
    void SetPauseIcon()
    {
        if (!pauseButtonImage) return;
        // Si prefieres que no cambie el icono, comenta este bloque
        if (!isPaused && pauseIcon) pauseButtonImage.sprite = pauseIcon;
        else if (isPaused && playIcon) pauseButtonImage.sprite = playIcon;
    }

    [SerializeField] string coinsPrefix = "x "; // puedes poner "× " si prefieres el símbolo

    void UpdateCoinsUI()
    {
        if (!coinsText) return;
        coinsText.text = $"{coinsPrefix}{coins}";
        // Si quieres miles con separador: coinsText.text = $"{coinsPrefix}{coins:N0}";
    }


    void UpdateTimeUI(bool force = false)
    {
        if (!timeText) return;
        int totalSeconds = Mathf.FloorToInt(elapsed);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    public float GetElapsedSeconds() => elapsed;
    public int GetRunCoins() => coins;
}
