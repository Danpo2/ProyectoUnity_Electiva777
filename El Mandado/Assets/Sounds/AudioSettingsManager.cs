using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("Sliders UI")]
    public Slider musicSlider;
    public Slider sfxSlider;

    // NOMBRES EXACTOS de los parámetros expuestos en el AudioMixer
    private const string MUSIC_PARAM = "MusicVol";  // debe coincidir con el Exposed Parameter
    private const string SFX_PARAM = "SFXVol";    // debe coincidir con el Exposed Parameter

    // Claves para PlayerPrefs
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    [Range(0.2f, 1f)]
    public float defaultMusic = 0.75f;

    [Range(0.2f, 1f)]
    public float defaultSfx = 0.75f;

    void Awake()
    {
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, defaultMusic);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, defaultSfx);

        if (musicSlider) musicSlider.value = music;
        if (sfxSlider) sfxSlider.value = sfx;

        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    public void OnMusicSliderChanged(float value)
    {
        SetMusicVolume(value);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);   // comenta si no quieres persistencia
    }

    public void OnSFXSliderChanged(float value)
    {
        SetSFXVolume(value);
        PlayerPrefs.SetFloat(SFX_KEY, value);     // comenta si no quieres persistencia
    }

    private void SetMusicVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);      // evita log10(0)
        float dB = Mathf.Log10(value) * 20f;
        mixer.SetFloat(MUSIC_PARAM, dB);
    }


    private void SetSFXVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        float dB = Mathf.Log10(value) * 20f;
        mixer.SetFloat(SFX_PARAM, dB);
    }
}
