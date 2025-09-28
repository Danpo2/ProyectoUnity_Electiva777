using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;

    [Header("Audio Sources")]
    public AudioSource music;
    public AudioSource sfx;

    [Header("Clips")]
    public AudioClip menuMusic;
    public AudioClip clickSfx;

    // PlayerPrefs keys
    const string KEY_MUSIC = "am_music_on";
    const string KEY_SFX = "am_sfx_on";
    const string KEY_VOL = "am_volume";

    public bool MusicOn { get; private set; }
    public bool SfxOn { get; private set; }
    public float Volume { get; private set; }

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        MusicOn = PlayerPrefs.GetInt(KEY_MUSIC, 1) == 1;
        SfxOn = PlayerPrefs.GetInt(KEY_SFX, 1) == 1;
        Volume = PlayerPrefs.GetFloat(KEY_VOL, 0.8f);

        ApplyAudioState();
    }

    void Start()
    {
        if (menuMusic != null)
        {
            music.clip = menuMusic;
            if (MusicOn) music.Play();
        }
    }

    public void SetMusic(bool on)
    {
        MusicOn = on;
        PlayerPrefs.SetInt(KEY_MUSIC, on ? 1 : 0);
        if (music != null)
        {
            if (on && !music.isPlaying) music.Play();
            else if (!on && music.isPlaying) music.Pause();
        }
    }

    public void SetSfx(bool on)
    {
        SfxOn = on;
        PlayerPrefs.SetInt(KEY_SFX, on ? 1 : 0);
    }

    public void SetVolume(float v)
    {
        Volume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(KEY_VOL, Volume);
        ApplyAudioState();
    }

    void ApplyAudioState()
    {
        if (music) music.volume = Volume;
        if (sfx) sfx.volume = Volume;
        if (!MusicOn && music && music.isPlaying) music.Pause();
    }

    public void PlayClick()
    {
        if (SfxOn && sfx && clickSfx) sfx.PlayOneShot(clickSfx);
    }
}
