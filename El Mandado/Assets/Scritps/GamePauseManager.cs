using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    public ModalView settingsModal;   // arrastra aquí el SettingsModal de la escena Game

    bool isPaused;

    public void OnPauseButton()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;          // pausa el juego
        settingsModal.Open();         // abre el mismo panel de ajustes
    }

    public void OnCloseSettingsInGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;          // reanuda el juego
        settingsModal.Close();        // cierra el panel
    }
}
