using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager I;

    public AudioSource musicSource;
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMenu()
    {
        PlayClip(menuMusic);
    }

    public void PlayGame()
    {
        PlayClip(gameMusic);
    }

    void PlayClip(AudioClip clip)
    {
        Debug.Log("[MusicManager] Cambiando clip a: " + clip.name);
        if (!clip || musicSource == null) return;

        // Si ya está ese clip, no hace falta cambiar
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
}
