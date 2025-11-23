using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    public ModalView settingsModal;

    bool isPaused;

    public void OnPauseButton()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        GameManager.I?.SetGameplayObjectsVisible(false);
        settingsModal.Open();
    }

    public void OnCloseSettingsInGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;
        GameManager.I?.SetGameplayObjectsVisible(true);
        settingsModal.Close();
    }
}
