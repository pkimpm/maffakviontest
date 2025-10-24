using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Микшер и Библиотека")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] public SoundLibrary soundLibrary;

    [Header("Настройки громкости для кода")]
    [Tooltip("Стандартная громкость музыки (от 0 до 1)")]
    public float defaultMusicVolume = 1.0f;
    
    [Tooltip("Стандартная громкость эмбиента (от 0 до 1)")]
    public float defaultAmbienceVolume = 0.8f;

    private AudioSource _musicSource1, _musicSource2;
    private AudioSource _ambienceSource1, _ambienceSource2;
    private AudioSource _uiSource, _sfxSource, _voiceSource;
    private AudioSource _unpausableSFXSource; 
    private bool _isMusicSource1Active = true;
    private bool _isAmbienceSource1Active = true;

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        } 
        else 
        { 
            Destroy(gameObject); 
            return; 
        }

        _musicSource1 = gameObject.AddComponent<AudioSource>(); 
        _musicSource2 = gameObject.AddComponent<AudioSource>();
        _ambienceSource1 = gameObject.AddComponent<AudioSource>(); 
        _ambienceSource2 = gameObject.AddComponent<AudioSource>();
        _uiSource = gameObject.AddComponent<AudioSource>(); 
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _voiceSource = gameObject.AddComponent<AudioSource>();
        _unpausableSFXSource = gameObject.AddComponent<AudioSource>(); 
        
        _musicSource1.outputAudioMixerGroup = GetGroup("Music"); 
        _musicSource2.outputAudioMixerGroup = GetGroup("Music");
        _ambienceSource1.outputAudioMixerGroup = GetGroup("Ambience"); 
        _ambienceSource2.outputAudioMixerGroup = GetGroup("Ambience");
        _uiSource.outputAudioMixerGroup = GetGroup("UI");
        _sfxSource.outputAudioMixerGroup = GetGroup("SFX");
        _voiceSource.outputAudioMixerGroup = GetGroup("Voice");
        _unpausableSFXSource.outputAudioMixerGroup = GetGroup("SFX");

        _musicSource1.loop = true; 
        _musicSource1.spatialBlend = 0.0f; 
        _musicSource2.loop = true; 
        _musicSource2.spatialBlend = 0.0f;
        _ambienceSource1.loop = true; 
        _ambienceSource1.spatialBlend = 0.0f; 
        _ambienceSource2.loop = true; 
        _ambienceSource2.spatialBlend = 0.0f;
        _voiceSource.loop = false; 
        _voiceSource.spatialBlend = 0.0f; 
        _uiSource.spatialBlend = 0.0f; 
        _sfxSource.spatialBlend = 0.0f;
   
        _unpausableSFXSource.loop = false;
        _unpausableSFXSource.spatialBlend = 0.0f;
        _unpausableSFXSource.ignoreListenerPause = true; 
        
        if (soundLibrary != null) soundLibrary.Initialize();
        
        InitializeDefaultVolumes();
    }

    private void InitializeDefaultVolumes()
    {
        SetMasterVolume(1f);
        SetMusicVolume(1f);
        SetAmbienceVolume(1f);
        SetVoiceVolume(1f);
        SetSFXVolume(1f);
        SetUIVolume(1f);

        Debug.Log("🔊 AudioManager: Default volumes initialized to maximum");
    }

    private AudioMixerGroup GetGroup(string groupName)
    {
        if (mainMixer == null) return null;
        
        AudioMixerGroup[] groups = mainMixer.FindMatchingGroups(groupName);
        if (groups != null && groups.Length > 0)
        {
            return groups[0];
        }
        return null;
    }

    public void CrossfadeMusic(string musicName, float duration)
    {
        AudioClip clip = soundLibrary.GetClip(musicName);
        if (clip == null) return;

        AudioSource activeSource = _isMusicSource1Active ? _musicSource1 : _musicSource2;
        AudioSource newSource = _isMusicSource1Active ? _musicSource2 : _musicSource1;

        newSource.clip = clip;
        newSource.Play();
        
        LeanTween.value(gameObject, 0f, defaultMusicVolume, duration)
            .setOnUpdate((float val) => {
                if (newSource != null) newSource.volume = val;
                if (activeSource != null) activeSource.volume = defaultMusicVolume - val;
            })
            .setOnComplete(() => { if (activeSource != null) activeSource.Stop(); });

        _isMusicSource1Active = !_isMusicSource1Active;
    }

    public void CrossfadeAmbience(string ambienceName, float duration)
    {
        AudioClip clip = soundLibrary.GetClip(ambienceName);
        if (clip == null) return;

        AudioSource activeSource = _isAmbienceSource1Active ? _ambienceSource1 : _ambienceSource2;
        AudioSource newSource = _isAmbienceSource1Active ? _ambienceSource2 : _ambienceSource1;

        newSource.clip = clip;
        newSource.Play();
        
        LeanTween.value(gameObject, 0f, defaultAmbienceVolume, duration)
            .setOnUpdate((float val) => {
                if (newSource != null) newSource.volume = val;
                if (activeSource != null) activeSource.volume = defaultAmbienceVolume - val;
            })
            .setOnComplete(() => { if (activeSource != null) activeSource.Stop(); });

        _isAmbienceSource1Active = !_isAmbienceSource1Active;
    }

    public void PlayUI(string soundName)
    {
        AudioClip clip = soundLibrary.GetClip(soundName);
        if (clip != null && _uiSource != null) _uiSource.PlayOneShot(clip);
    }
    
    public void PlaySFX(string soundName)
    {
        AudioClip clip = soundLibrary.GetClip(soundName);
        if (clip != null && _sfxSource != null) _sfxSource.PlayOneShot(clip);
    }

    public void PlaySFXUnpausable(string soundName)
    {
        AudioClip clip = soundLibrary.GetClip(soundName);
        if (clip != null && _unpausableSFXSource != null)
        {
            _unpausableSFXSource.PlayOneShot(clip);
            Debug.Log($"🔊 Playing unpausable SFX: {soundName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Cannot play unpausable SFX: {soundName}");
        }
    }
    
    public void PlayVoice(AudioClip voiceClip)
    {
        if (voiceClip != null && _voiceSource != null)
        {
            _voiceSource.Stop();
            _voiceSource.PlayOneShot(voiceClip);
        }
    }

    public void StopAllAudio()
    {
        if (_musicSource1 != null)
        {
            _musicSource1.Stop();
            _musicSource1.volume = 0f;
        }
        if (_musicSource2 != null)
        {
            _musicSource2.Stop();
            _musicSource2.volume = 0f;
        }

        if (_ambienceSource1 != null)
        {
            _ambienceSource1.Stop();
            _ambienceSource1.volume = 0f;
        }
        if (_ambienceSource2 != null)
        {
            _ambienceSource2.Stop();
            _ambienceSource2.volume = 0f;
        }

        if (_voiceSource != null)
        {
            _voiceSource.Stop();
        }

        LeanTween.cancel(gameObject);

        Debug.Log("🔇 All audio stopped");
    }

    public void SetMasterVolume(float value) => SetVolume("MasterVolume", value);
    public void SetMusicVolume(float value) => SetVolume("MusicVolume", value);
    public void SetAmbienceVolume(float value) => SetVolume("AmbienceVolume", value);
    public void SetVoiceVolume(float value) => SetVolume("VoiceVolume", value);
    public void SetSFXVolume(float value) => SetVolume("SFXVolume", value);
    public void SetUIVolume(float value) => SetVolume("UIVolume", value);

    public void SetVFXVolume(float value) => SetVolume("SFXVolume", value);

    private void SetVolume(string exposedParam, float value)
    {
        if (mainMixer != null)
            mainMixer.SetFloat(exposedParam, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
    }
}