using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string gameSceneName = "Game";

    bool done = false;

    void Start()
    {
        // Apagar música de menú al entrar en esta escena
        MusicManager.I?.StopMusic();
    }

    void Awake()
    {
        if (videoPlayer)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnDestroy()
    {
        if (videoPlayer)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadGameIfNotDone();
    }

    public void OnSkipButton()
    {
        LoadGameIfNotDone();
    }

    void LoadGameIfNotDone()
    {
        if (done) return;
        done = true;
        SceneManager.LoadScene(gameSceneName);
    }
}
