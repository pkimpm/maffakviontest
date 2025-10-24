using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Слайдеры")]
    public Slider masterSlider; 
    public Slider musicSlider;
    public Slider ambienceSlider;
    public Slider voiceSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string AmbienceVolumeKey = "AmbienceVolume";
    private const string VoiceVolumeKey = "VoiceVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string UiVolumeKey = "UiVolume";

    private bool isInitialized = false;

    private void Start()
    {
        SetupSliders();
        LoadSettings();
        isInitialized = true;
    }

    private void SetupSliders()
    {
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (ambienceSlider != null)
            ambienceSlider.onValueChanged.AddListener(OnAmbienceVolumeChanged);
        if (voiceSlider != null)
            voiceSlider.onValueChanged.AddListener(OnVoiceVolumeChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        if (uiSlider != null)
            uiSlider.onValueChanged.AddListener(OnUiVolumeChanged);
    }

    private void OnDestroy()
    {
        if (masterSlider != null)
            masterSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        if (ambienceSlider != null)
            ambienceSlider.onValueChanged.RemoveListener(OnAmbienceVolumeChanged);
        if (voiceSlider != null)
            voiceSlider.onValueChanged.RemoveListener(OnVoiceVolumeChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        if (uiSlider != null)
            uiSlider.onValueChanged.RemoveListener(OnUiVolumeChanged);
    }

    private void LoadSettings()
    {
        if (masterSlider != null)
        {
            float masterVol = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
            masterSlider.value = masterVol;
            ApplyMasterVolume(masterVol);
        }

        if (musicSlider != null)
        {
            float musicVol = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            musicSlider.value = musicVol;
            ApplyMusicVolume(musicVol);
        }

        if (ambienceSlider != null)
        {
            float ambienceVol = PlayerPrefs.GetFloat(AmbienceVolumeKey, 1f);
            ambienceSlider.value = ambienceVol;
            ApplyAmbienceVolume(ambienceVol);
        }

        if (voiceSlider != null)
        {
            float voiceVol = PlayerPrefs.GetFloat(VoiceVolumeKey, 1f);
            voiceSlider.value = voiceVol;
            ApplyVoiceVolume(voiceVol);
        }

        if (sfxSlider != null)
        {
            float sfxVol = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            sfxSlider.value = sfxVol;
            ApplySfxVolume(sfxVol);
        }

        if (uiSlider != null)
        {
            float uiVol = PlayerPrefs.GetFloat(UiVolumeKey, 1f);
            uiSlider.value = uiVol;
            ApplyUiVolume(uiVol);
        }

        Debug.Log($"🔊 Audio settings loaded: Master={masterSlider?.value}, Music={musicSlider?.value}, Ambience={ambienceSlider?.value}, Voice={voiceSlider?.value}, SFX={sfxSlider?.value}, UI={uiSlider?.value}");
    }

    private void ApplyMasterVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
        }
    }

    private void ApplyMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    private void ApplyAmbienceVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetAmbienceVolume(value);
        }
    }

    private void ApplyVoiceVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVoiceVolume(value);
        }
    }

    private void ApplySfxVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }

    private void ApplyUiVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetUIVolume(value);
        }
    }

    public void OnMasterVolumeChanged(float value)
    {
        ApplyMasterVolume(value);
        
        if (isInitialized)
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, value);
            PlayerPrefs.Save();
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        ApplyMusicVolume(value);
        
        if (isInitialized)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
            PlayerPrefs.Save();
        }
    }

    public void OnAmbienceVolumeChanged(float value)
    {
        ApplyAmbienceVolume(value);
        
        if (isInitialized)
        {
            PlayerPrefs.SetFloat(AmbienceVolumeKey, value);
            PlayerPrefs.Save();
        }
    }
    
    public void OnVoiceVolumeChanged(float value)
    {
        ApplyVoiceVolume(value);
        
        if (isInitialized)
        {
            PlayerPrefs.SetFloat(VoiceVolumeKey, value);
            PlayerPrefs.Save();
        }
    }

    public void OnSfxVolumeChanged(float value)
    {
        ApplySfxVolume(value);
        
        if (isInitialized)
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, value);
            PlayerPrefs.Save();
        }
    }

    public void OnUiVolumeChanged(float value)
    {
        ApplyUiVolume(value);
        
        if (isInitialized)
        {
            PlayerPrefs.SetFloat(UiVolumeKey, value);
            PlayerPrefs.Save();
        }
    }

    public void ResetToMaxVolume()
    {
        if (masterSlider != null) masterSlider.value = 1f;
        if (musicSlider != null) musicSlider.value = 1f;
        if (ambienceSlider != null) ambienceSlider.value = 1f;
        if (voiceSlider != null) voiceSlider.value = 1f;
        if (sfxSlider != null) sfxSlider.value = 1f;
        if (uiSlider != null) uiSlider.value = 1f;

        PlayerPrefs.SetFloat(MasterVolumeKey, 1f);
        PlayerPrefs.SetFloat(MusicVolumeKey, 1f);
        PlayerPrefs.SetFloat(AmbienceVolumeKey, 1f);
        PlayerPrefs.SetFloat(VoiceVolumeKey, 1f);
        PlayerPrefs.SetFloat(SfxVolumeKey, 1f);
        PlayerPrefs.SetFloat(UiVolumeKey, 1f);
        PlayerPrefs.Save();

        Debug.Log("🔊 Audio reset to maximum volume");
    }
}