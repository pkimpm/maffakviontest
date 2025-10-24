using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GamePauseController : MonoBehaviour
{
    [Header("Главные элементы управления")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button closeSettingsButton;
    [SerializeField] private Button muteButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button exitButton;

    [Header("Диалог подтверждения выхода")]
    [SerializeField] private ConfirmExitPanel confirmExitPanel;

    [Header("Диалог подтверждения новой игры")]
    [SerializeField] private ConfirmNewGamePanel confirmNewGamePanel;

    [Header("UI для управления")]
    [SerializeField] private List<GameObject> uiElementsToManage;

    [Header("Слайдеры громкости")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider ambienceVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;

    [Header("Настройки кнопки Mute")]
    [SerializeField] private Image muteButtonImage;
    [SerializeField] private Color muteActiveColor = Color.red;

    [Header("Звуки UI")]
    [SerializeField] private string buttonClickSound = "sound_ui_buttonclick_add";
    [SerializeField] private string buttonHoverSound = "sound_ui_buttonhover";

    [Header("Затемнение для перезагрузки")]
    [SerializeField] private CanvasGroup blackoutPanel;
    [SerializeField] private float blackoutDuration = 1.5f;

    [Header("Настройки перезагрузки")]
    [SerializeField] private string firstSceneName = "LoadingScreen";
    [SerializeField] private bool useSceneIndex = true;
    [SerializeField] private int firstSceneIndex = 0;

    private List<GameObject> _activeUiBeforePause;
    private AudioSource _uiAudioSource;
    private bool isMuted = false;
    private Color originalMuteColor;
    private bool isPaused = false;
    
    private bool _dialogueContinueButtonWasInteractable = false;
    private Button _dialogueContinueButton = null;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string AmbienceVolumeKey = "AmbienceVolume";
    private const string VoiceVolumeKey = "VoiceVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string UiVolumeKey = "UiVolume";

    private void Awake()
    {
        _uiAudioSource = GetComponent<AudioSource>();
        _uiAudioSource.playOnAwake = false;
        _uiAudioSource.ignoreListenerPause = true;
        _uiAudioSource.spatialBlend = 0f;
        
        _activeUiBeforePause = new List<GameObject>();

        if (blackoutPanel != null)
        {
            blackoutPanel.alpha = 0f;
            blackoutPanel.blocksRaycasts = false;
        }
    }

    private void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        isMuted = false;

        if (muteButtonImage != null)
        {
            originalMuteColor = muteButtonImage.color;
        }

        SetupButtons();
        SetupSliders();
    }

    private void SetupButtons()
    {
        if (openSettingsButton != null)
        {
            openSettingsButton.onClick.RemoveAllListeners();
            openSettingsButton.onClick.AddListener(() => 
            { 
                PlayUISound(buttonClickSound);
                PauseGame(); 
            });
            AddHoverSound(openSettingsButton);
        }
        
        if (closeSettingsButton != null)
        {
            closeSettingsButton.onClick.RemoveAllListeners();
            closeSettingsButton.onClick.AddListener(() => 
            { 
                PlayUISound(buttonClickSound);
                ResumeGame(); 
            });
            AddHoverSound(closeSettingsButton);
        }
        
        if (muteButton != null)
        {
            muteButton.onClick.RemoveAllListeners();
            muteButton.onClick.AddListener(() => 
            { 
                PlayUISound(buttonClickSound);
                ToggleMute(); 
            });
            AddHoverSound(muteButton);
        }

        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(() => 
            { 
                PlayUISound(buttonClickSound);
                ShowConfirmNewGame(); 
            });
            AddHoverSound(newGameButton);
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() => 
            { 
                PlayUISound(buttonClickSound);
                ShowConfirmExit(); 
            });
            AddHoverSound(exitButton);
        }
    }

    private void SetupSliders()
    {
        if (masterVolumeSlider != null) 
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider != null) 
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (ambienceVolumeSlider != null)
            ambienceVolumeSlider.onValueChanged.AddListener(OnAmbienceVolumeChanged);
        if (voiceVolumeSlider != null) 
            voiceVolumeSlider.onValueChanged.AddListener(OnVoiceVolumeChanged);
        if (sfxVolumeSlider != null) 
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        if (uiVolumeSlider != null)
            uiVolumeSlider.onValueChanged.AddListener(OnUiVolumeChanged);
    }

    private void OnDestroy()
    {
        if (openSettingsButton != null) openSettingsButton.onClick.RemoveAllListeners();
        if (closeSettingsButton != null) closeSettingsButton.onClick.RemoveAllListeners();
        if (muteButton != null) muteButton.onClick.RemoveAllListeners();
        if (newGameButton != null) newGameButton.onClick.RemoveAllListeners();
        if (exitButton != null) exitButton.onClick.RemoveAllListeners();

        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        if (ambienceVolumeSlider != null) ambienceVolumeSlider.onValueChanged.RemoveListener(OnAmbienceVolumeChanged);
        if (voiceVolumeSlider != null) voiceVolumeSlider.onValueChanged.RemoveListener(OnVoiceVolumeChanged);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        if (uiVolumeSlider != null) uiVolumeSlider.onValueChanged.RemoveListener(OnUiVolumeChanged);
    }

    private void PlayUISound(string soundName)
    {
        if (string.IsNullOrEmpty(soundName)) return;
        if (_uiAudioSource == null) return;

        if (AudioManager.Instance != null && AudioManager.Instance.soundLibrary != null)
        {
            AudioClip clip = AudioManager.Instance.soundLibrary.GetClip(soundName);
            if (clip != null)
            {
                _uiAudioSource.PlayOneShot(clip);
            }
        }
    }

    private void AddHoverSound(Button button)
    {
        if (button == null) return;

        var trigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => 
        {
            if (button.interactable)
            {
                PlayUISound(buttonHoverSound);
            }
        });
        
        trigger.triggers.Add(entry);
    }

    private void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        
        SaveDialogueContinueButtonState();

        _activeUiBeforePause.Clear();
        foreach (var ui in uiElementsToManage)
        {
            if (ui != null && ui.activeSelf)
            {
                _activeUiBeforePause.Add(ui);
                ui.SetActive(false);
            }
        }

        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        UpdateSlidersWithCurrentVolume();
    }

    private void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        foreach (var ui in _activeUiBeforePause)
        {
            if (ui != null)
            {
                ui.SetActive(true);
            }
        }
        _activeUiBeforePause.Clear();

        RestoreDialogueContinueButtonState();
    }

    private void SaveDialogueContinueButtonState()
    {
        var dialogueView = FindObjectOfType<DialogueView>();
        if (dialogueView != null)
        {
            _dialogueContinueButton = dialogueView.GetComponentInChildren<Button>();
            if (_dialogueContinueButton != null)
            {
                _dialogueContinueButtonWasInteractable = _dialogueContinueButton.interactable;
            }
        }
    }

    private void RestoreDialogueContinueButtonState()
    {
        if (_dialogueContinueButton == null)
        {
            return;
        }

        if (!DialogueManager.IsDialogueActive)
        {
            _dialogueContinueButton = null;
            return;
        }

        _dialogueContinueButton.interactable = _dialogueContinueButtonWasInteractable;
        _dialogueContinueButton = null;
    }

    private void ToggleMute()
    {
        isMuted = !isMuted;

        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>(true);
        
        foreach (AudioSource source in allAudioSources)
        {
            if (source == _uiAudioSource) continue;
            source.mute = isMuted;
        }

        if (muteButtonImage != null)
        {
            muteButtonImage.color = isMuted ? muteActiveColor : originalMuteColor;
        }
    }

    private void ShowConfirmNewGame()
    {
        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.OnConfirm = () => 
            {
                StartCoroutine(RestartGame());
            };
            
            confirmNewGamePanel.OnCancel = () => 
            {
                Debug.Log("❌ Cancel New Game clicked");
            };
            
            confirmNewGamePanel.Show();
        }
    }

    private void ShowConfirmExit()
    {
        if (confirmExitPanel != null)
        {
            confirmExitPanel.OnConfirm = () => 
            {
                StartCoroutine(ExitGame());
            };
            
            confirmExitPanel.OnCancel = () => 
            {
                Debug.Log("❌ Cancel clicked");
            };
            
            confirmExitPanel.Show();
        }
    }

    private IEnumerator RestartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllAudio();
        }

        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        if (confirmNewGamePanel != null)
        {
            var cg = confirmNewGamePanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0f;
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        LeanTween.cancelAll();

        if (blackoutPanel != null)
        {
            blackoutPanel.transform.SetAsLastSibling();
            blackoutPanel.blocksRaycasts = true;

            LeanTween.cancel(blackoutPanel.gameObject);
            blackoutPanel.alpha = 0f;

            bool finished = false;
            LeanTween.alphaCanvas(blackoutPanel, 1f, blackoutDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setIgnoreTimeScale(true)
                .setOnComplete(() => finished = true);

            float elapsedTime = 0f;
            while (!finished && elapsedTime < blackoutDuration + 1f)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }

        if (useSceneIndex)
        {
            SceneManager.LoadScene(firstSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(firstSceneName);
        }
    }

    private IEnumerator ExitGame()
    {
        if (blackoutPanel != null)
        {
            blackoutPanel.transform.SetAsLastSibling();
            blackoutPanel.blocksRaycasts = true;

            LeanTween.cancel(blackoutPanel.gameObject);
            blackoutPanel.alpha = 0f;

            bool finished = false;
            LeanTween.alphaCanvas(blackoutPanel, 1f, blackoutDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setIgnoreTimeScale(true)
                .setOnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void UpdateSlidersWithCurrentVolume()
    {
        if (AudioManager.Instance != null)
        {
            if (masterVolumeSlider != null) 
            {
                float vol = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
                masterVolumeSlider.value = vol;
                AudioManager.Instance.SetMasterVolume(vol);
            }

            if (musicVolumeSlider != null) 
            {
                float vol = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
                musicVolumeSlider.value = vol;
                AudioManager.Instance.SetMusicVolume(vol);
            }

            if (ambienceVolumeSlider != null) 
            {
                float vol = PlayerPrefs.GetFloat(AmbienceVolumeKey, 1f);
                ambienceVolumeSlider.value = vol;
                AudioManager.Instance.SetAmbienceVolume(vol);
            }

            if (voiceVolumeSlider != null) 
            {
                float vol = PlayerPrefs.GetFloat(VoiceVolumeKey, 1f);
                voiceVolumeSlider.value = vol;
                AudioManager.Instance.SetVoiceVolume(vol);
            }

            if (sfxVolumeSlider != null) 
            {
                float vol = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
                sfxVolumeSlider.value = vol;
                AudioManager.Instance.SetSFXVolume(vol);
            }

            if (uiVolumeSlider != null) 
            {
                float vol = PlayerPrefs.GetFloat(UiVolumeKey, 1f);
                uiVolumeSlider.value = vol;
                AudioManager.Instance.SetUIVolume(vol);
            }
        }
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
            PlayerPrefs.SetFloat(MasterVolumeKey, value);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
        }
    }

    private void OnAmbienceVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetAmbienceVolume(value);
            PlayerPrefs.SetFloat(AmbienceVolumeKey, value);
        }
    }

    private void OnVoiceVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVoiceVolume(value);
            PlayerPrefs.SetFloat(VoiceVolumeKey, value);
        }
    }

    private void OnSfxVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
            PlayerPrefs.SetFloat(SfxVolumeKey, value);
        }
    }

    private void OnUiVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetUIVolume(value);
            PlayerPrefs.SetFloat(UiVolumeKey, value);
        }
    }
}