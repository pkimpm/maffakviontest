using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemReceivedPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button continueButton;

    [Header("Звуки")] // 🔥 Новый раздел
    [SerializeField] private string buttonClickSound = "sound_ui_buttonclick_add";
    [SerializeField] private string buttonHoverSound = "sound_ui_buttonhover";

    public System.Action OnContinue;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            PlayButtonClick();
            Hide();
            OnContinue?.Invoke();
        });
        AddHoverSound(continueButton);
    }

    private void PlayButtonClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(buttonClickSound);
        }
    }

    private void AddHoverSound(Button button)
    {
        if (button == null) return;

        var trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => 
        {
            if (AudioManager.Instance != null && button.interactable)
            {
                AudioManager.Instance.PlayUI(buttonHoverSound);
            }
        });
        trigger.triggers.Add(entry);
    }

    public void Show(string message)
    {
        messageText.text = message;
        LeanTween.cancel(canvasGroup.gameObject);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        LeanTween.alphaCanvas(canvasGroup, 1f, 0.3f).setEase(LeanTweenType.easeInOutQuad);
    }

    private void Hide()
    {
        LeanTween.alphaCanvas(canvasGroup, 0f, 0.3f).setOnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }
}