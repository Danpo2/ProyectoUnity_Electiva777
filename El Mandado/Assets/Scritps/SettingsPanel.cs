using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("UI")]
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Toggle vibrationToggle;
    public Slider volumeSlider;

    const string KEY_VIB = "am_vibration_on";

    void Awake()
    {
        // Autowire por nombre si faltan refs (opcional pero útil)
        if (!musicToggle) musicToggle = FindToggleContains("Music");
        if (!sfxToggle) sfxToggle = FindToggleContains("Sfx");
        if (!vibrationToggle) vibrationToggle = FindToggleContains("Vibration");
        if (!volumeSlider) volumeSlider = GetComponentInChildren<Slider>(true);
    }

    void OnEnable()
    {
        // Puede faltar AudioManager si aún no cargó; no falles.
        if (AudioManager.I != null)
        {
            if (musicToggle) musicToggle.isOn = AudioManager.I.MusicOn;
            if (sfxToggle) sfxToggle.isOn = AudioManager.I.SfxOn;
            if (volumeSlider) volumeSlider.value = AudioManager.I.Volume;
        }
        if (vibrationToggle) vibrationToggle.isOn = PlayerPrefs.GetInt(KEY_VIB, 1) == 1;
    }

    public void OnMusicChanged(bool on) { AudioManager.I?.SetMusic(on); }
    public void OnSfxChanged(bool on) { AudioManager.I?.SetSfx(on); }
    public void OnVolumeChanged(float v) { AudioManager.I?.SetVolume(v); }
    public void OnVibrationChanged(bool on)
    {
        PlayerPrefs.SetInt(KEY_VIB, on ? 1 : 0);
#if UNITY_ANDROID && !UNITY_EDITOR
        if (on) Handheld.Vibrate();
#endif
    }

    Toggle FindToggleContains(string token)
    {
        token = token.ToLower();
        foreach (var t in GetComponentsInChildren<Toggle>(true))
            if (t.name.ToLower().Contains(token)) return t;
        return null;
    }
}
