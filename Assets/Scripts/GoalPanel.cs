using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GoalPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text goalText;
    [SerializeField] private Button closeButton;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private LeanTweenType fadeEase = LeanTweenType.easeInOutQuad;

    [Header("Звуки")] // 🔥 Новый раздел
    [SerializeField] private string buttonClickSound = "sound_ui_buttonclick_add";
    [SerializeField] private string buttonHoverSound = "sound_ui_buttonhover";

    private bool isOpen;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() =>
            {
                PlayButtonClick();
                Hide();
            });
            AddHoverSound(closeButton);
        }
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

    public void Show(GoalData data)
    {
        if (data == null || isOpen) return;
        isOpen = true;

        goalText.text = data.goalText;
        canvasGroup.transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        LeanTween.cancel(canvasGroup.gameObject);
        canvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration).setEase(fadeEase);
    }

    public void Hide()
    {
        if (!isOpen) return;
        LeanTween.cancel(canvasGroup.gameObject);
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration)
            .setEase(fadeEase)
            .setOnComplete(() =>
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                isOpen = false;
            });
    }

    public IEnumerator WaitUntilClosed()
    {
        while (isOpen)
            yield return null;
    }
}