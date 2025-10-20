using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDGameUI : MonoBehaviour
{
    public static HUDGameUI I;

    [Header("UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI coinsText;

    [Header("Opcional: icono de pausa")]
    public Button pauseButton;
    public Sprite pauseIcon;
    public Sprite playIcon;
    public Image pauseButtonImage;

    int coins;
    float elapsed;
    float uiUpdateClock;

    void Awake()
    {
        // Singleton
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        Time.timeScale = 1f;  // asegurar que el juego empieza activo
        coins = 0;
        elapsed = 0f;

        UpdateCoinsUI();
        UpdateTimeUI(force: true);

        SetPauseIcon();
    }

    void Update()
    {
        // No acumules tiempo si el juego está pausado
        if (Time.timeScale == 0f) return;

        elapsed += Time.deltaTime;

        uiUpdateClock += Time.deltaTime;
        if (uiUpdateClock >= 0.1f)
        {
            uiUpdateClock = 0f;
            UpdateTimeUI();
        }
    }

    // ----------------- API pública -----------------
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
    }

    // ----------------- Helpers -----------------
    [SerializeField] string coinsPrefix = "x "; // o "× "

    void UpdateCoinsUI()
    {
        if (!coinsText) return;
        coinsText.text = $"{coinsPrefix}{coins}";
    }

    void UpdateTimeUI(bool force = false)
    {
        if (!timeText) return;
        int totalSeconds = Mathf.FloorToInt(elapsed);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    void SetPauseIcon()
    {
        if (!pauseButtonImage) return;
        if (Time.timeScale != 0f && pauseIcon) pauseButtonImage.sprite = pauseIcon;
        else if (Time.timeScale == 0f && playIcon) pauseButtonImage.sprite = playIcon;
    }

    public float GetElapsedSeconds() => elapsed;
    public int GetRunCoins() => coins;
}
