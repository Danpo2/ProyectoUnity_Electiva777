using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer I;

    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip buttonClick;
    public AudioClip coinPickup;
    public AudioClip loseSound;
    public AudioClip winSound;

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


    public void PlayButton()
    {
        PlayOneShot(buttonClick);
    }

    public void PlayCoin()
    {
        PlayOneShot(coinPickup);
    }

    public void PlayLose()
    {
        PlayOneShot(loseSound);
    }

    public void PlayWin()
    {
        PlayOneShot(winSound);
    }

    void PlayOneShot(AudioClip clip)
    {
        if (!clip || !sfxSource) return;
        sfxSource.PlayOneShot(clip);
    }
}
