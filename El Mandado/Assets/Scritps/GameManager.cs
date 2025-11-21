using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("UI Game Over / Victoria")]
    public GameObject losePanel;
    public GameObject winPanel;

    public TextMeshProUGUI loseTimeText;
    public TextMeshProUGUI loseCoinsText;

    public TextMeshProUGUI winTimeText;
    public TextMeshProUGUI winCoinsText;

    bool isGameOver;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        Time.timeScale = 1f;
        isGameOver = false;

        if (losePanel) losePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    // ---------- Llamado cuando pierde (choque) ----------
    public void PlayerCrashed()
    {
        if (isGameOver) return;
        isGameOver = true;

        SFXPlayer.I?.PlayLose();
        ShowEndUI(false);
        _ = SaveRunToFirebase();
    }


    // ---------- Llamado cuando llega a la meta ----------
    public void PlayerReachedGoal()
    {
        if (isGameOver) return;
        isGameOver = true;

        SFXPlayer.I?.PlayWin();
        ShowEndUI(true);
        _ = SaveRunToFirebase();
    }


    void ShowEndUI(bool win)
    {
        Time.timeScale = 0f;

        int coins = HUDGameUI.I != null ? HUDGameUI.I.GetRunCoins() : 0;
        float time = HUDGameUI.I != null ? HUDGameUI.I.GetElapsedSeconds() : 0f;
        string timeStr = FormatTime(time);

        if (win)
        {
            if (winPanel) winPanel.SetActive(true);
            if (winCoinsText) winCoinsText.text = $"Monedas: {coins}";
            if (winTimeText) winTimeText.text = $"Tiempo: {timeStr}";
        }
        else
        {
            if (losePanel) losePanel.SetActive(true);
            if (loseCoinsText) loseCoinsText.text = $"Monedas: {coins}";
            if (loseTimeText) loseTimeText.text = $"Tiempo: {timeStr}";
        }
    }

    string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        int m = total / 60;
        int s = total % 60;
        return $"{m:00}:{s:00}";
    }

    async Task SaveRunToFirebase()
    {
        int coins = HUDGameUI.I != null ? HUDGameUI.I.GetRunCoins() : 0;

        if (FirebaseManager.I != null && FirebaseManager.I.IsReady)
        {
            await FirebaseManager.I.SubmitHighScoreAsync(coins);
        }
    }

    // ---------- Botones UI ----------
    public void OnPlayAgainButton()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}
