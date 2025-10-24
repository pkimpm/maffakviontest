using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmNewGamePanel : MonoBehaviour
{
    [Header("UI компоненты")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Тексты")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private string confirmMessage = "Начать новую игру? Весь прогресс будет потерян!";

    [Header("Анимация")]
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("Звуки")]
    [SerializeField] private string confirmClickSound = "sound_ui_buttonclick_add";
    [SerializeField] private string cancelClickSound = "sound_ui_buttonclick_add";
    [SerializeField] private string buttonHoverSound = "sound_ui_buttonhover";

    public System.Action OnConfirm;
    public System.Action OnCancel;

    private AudioSource _localAudioSource;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        _localAudioSource = gameObject.AddComponent<AudioSource>();
        _localAudioSource.playOnAwake = false;
        _localAudioSource.ignoreListenerPause = true;
        _localAudioSource.spatialBlend = 0f;

        SetupButtons();
        
        if (messageText != null)
        {
            messageText.text = confirmMessage;
        }

        Debug.Log("✅ ConfirmNewGamePanel initialized");
    }

    private void SetupButtons()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => 
            {
                Debug.Log("🔊 Confirm New Game button clicked!");
                PlaySound(confirmClickSound);
                Hide();
                OnConfirm?.Invoke();
            });
            AddHoverSound(confirmButton);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => 
            {
                Debug.Log("🔊 Cancel New Game button clicked!");
                PlaySound(cancelClickSound);
                Hide();
                OnCancel?.Invoke();
            });
            AddHoverSound(cancelButton);
        }
    }

    private void PlaySound(string soundName)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            Debug.LogWarning("⚠️ Sound name is empty!");
            return;
        }

        Debug.Log($"🔊 Playing sound: {soundName}");

        if (_localAudioSource == null)
        {
            Debug.LogError("❌ Local AudioSource is null!");
            return;
        }

        if (AudioManager.Instance != null && AudioManager.Instance.soundLibrary != null)
        {
            AudioClip clip = AudioManager.Instance.soundLibrary.GetClip(soundName);
            
            if (clip != null)
            {
                _localAudioSource.PlayOneShot(clip);
                Debug.Log($"✅ Sound played: {soundName}");
            }
            else
            {
                Debug.LogError($"❌ Sound '{soundName}' not found!");
            }
        }
        else
        {
            Debug.LogError("❌ AudioManager or SoundLibrary is null!");
        }
    }

    private void AddHoverSound(Button button)
    {
        if (button == null || string.IsNullOrEmpty(buttonHoverSound)) return;

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
                PlaySound(buttonHoverSound);
            }
        });
        
        trigger.triggers.Add(entry);
    }

    public void Show()
    {
        if (canvasGroup == null) return;

        Debug.Log("📋 Showing ConfirmNewGamePanel");

        canvasGroup.transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        LeanTween.cancel(canvasGroup.gameObject);
        canvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setIgnoreTimeScale(true);
    }

    private void Hide()
    {
        if (canvasGroup == null) return;

        Debug.Log("📋 Hiding ConfirmNewGamePanel");

        LeanTween.cancel(canvasGroup.gameObject);
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            });
    }
}