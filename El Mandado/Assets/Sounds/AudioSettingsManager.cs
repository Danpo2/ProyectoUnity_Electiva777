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
        // Valores por defecto al dar Play (si quieres leer lo guardado, usa PlayerPrefs.GetFloat aquí)
        float music = defaultMusic;
        float sfx = defaultSfx;

        if (musicSlider) musicSlider.value = music;
        if (sfxSlider) sfxSlider.value = sfx;

        SetMusicVolume(music);
        SetSFXVolume(sfx);

        // Opcional: si NO quieres que se guarde nada entre ejecuciones, comenta estas dos líneas
        PlayerPrefs.SetFloat(MUSIC_KEY, music);
        PlayerPrefs.SetFloat(SFX_KEY, sfx);
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
        

        // Convertir valor lineal (0–1) a dB
        float dB = Mathf.Log10(value) * 20f;
        mixer.SetFloat(MUSIC_PARAM, dB);
    }

    private void SetSFXVolume(float value)
    {
        

        float dB = Mathf.Log10(value) * 20f;
        mixer.SetFloat(SFX_PARAM, dB);
    }
}
