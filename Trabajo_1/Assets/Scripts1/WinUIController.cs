using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUIController : MonoBehaviour
{
    [Header("Panel de victoria")]
    public GameObject winPanel;

    [Header("Encuesta")]
    [Tooltip("URL del formulario/encuesta")]
    public string surveyUrl = "https://tusitio.com/mi-encuesta";

    [Header("Opcional")]
    public bool pauseOnWin = true;     // Pausar el juego al ganar
    public string menuSceneName = "";  // Si quieres volver al menú

    [Header("Timing")]
    [Tooltip("Tiempo (seg) antes de mostrar el panel")]
    public float showDelay = 1.5f;
    [Tooltip("Usar tiempo real (recomendado si luego pausas Time.timeScale)")]
    public bool useRealtimeDelay = true;

    // Llamado por CraftingManager
    public void ShowWin()
    {
        StartCoroutine(ShowWinDelayed());
    }

    private IEnumerator ShowWinDelayed()
    {
        if (showDelay > 0f)
        {
            if (useRealtimeDelay) yield return new WaitForSecondsRealtime(showDelay);
            else yield return new WaitForSeconds(showDelay);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (pauseOnWin) Time.timeScale = 0f;
        }
    }

    public void OnSurvey()
    {
        if (!string.IsNullOrEmpty(surveyUrl))
            Application.OpenURL(surveyUrl);
    }

    public void OnExit()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
            return;
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
