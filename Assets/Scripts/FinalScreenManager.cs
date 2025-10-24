using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FinalScreenManager : MonoBehaviour
{
    [Header("UI компоненты")]
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text promoCodeText;
    [SerializeField] private Button copyButton;
    [SerializeField] private TMP_Text copyButtonLabel;
    [SerializeField] private Button closeButton;

    [Header("Контент")]
    [SerializeField] private string title;
    [TextArea][SerializeField] private string body;
    [SerializeField] private string promoCode;

    [Header("Анимация")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Кнопка копирования")]
    [SerializeField] private string copyTextDefault = "Скопировать";
    [SerializeField] private string copyTextAfter = "Скопировано!";
    [SerializeField] private float resetDelay = 2f;

    [Header("Затемнение для выхода")]
    [SerializeField] private CanvasGroup blackoutPanel;
    [SerializeField] private float blackoutDuration = 1.5f;

    [Header("Звуки")] // 🔥 Новый раздел
    [SerializeField] private string buttonClickSound = "sound_ui_buttonclick_add";
    [SerializeField] private string buttonHoverSound = "sound_ui_buttonhover";

    private bool isCopyButtonResetScheduled;

    private void Awake()
    {
        panel.alpha = 0f;
        panel.blocksRaycasts = false;
        panel.interactable = false;

        if (blackoutPanel != null)
        {
            blackoutPanel.alpha = 0f;
            blackoutPanel.blocksRaycasts = false;
        }

        copyButton.onClick.RemoveAllListeners();
        copyButton.onClick.AddListener(() => 
        {
            PlayButtonClick();
            CopyPromoCode();
        });
        AddHoverSound(copyButton);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => 
        {
            PlayButtonClick();
            StartCoroutine(CloseGameSequence());
        });
        AddHoverSound(closeButton);
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
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUI(buttonHoverSound);
            }
        });
        trigger.triggers.Add(entry);
    }

    public void Show()
    {
        titleText.text = title;
        bodyText.text = body;
        promoCodeText.text = promoCode;

        panel.transform.SetAsLastSibling();
        panel.blocksRaycasts = true;
        panel.interactable = true;

        LeanTween.cancel(panel.gameObject);
        panel.alpha = 0f;
        LeanTween.alphaCanvas(panel, 1f, fadeDuration)
            .setEase(LeanTweenType.easeInOutQuad);

        ResetCopyButtonLabel();
    }

    private void CopyPromoCode()
    {
        GUIUtility.systemCopyBuffer = promoCode;
        UpdateCopyButtonLabel(copyTextAfter);

        if (!isCopyButtonResetScheduled)
        {
            isCopyButtonResetScheduled = true;
            LeanTween.delayedCall(resetDelay, () =>
            {
                UpdateCopyButtonLabel(copyTextDefault);
                isCopyButtonResetScheduled = false;
            });
        }
    }

    private void UpdateCopyButtonLabel(string text)
    {
        if (copyButtonLabel != null) copyButtonLabel.text = text;
    }

    private void ResetCopyButtonLabel() => UpdateCopyButtonLabel(copyTextDefault);

    private IEnumerator CloseGameSequence()
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
                .setOnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);
        }

        LeanTween.cancel(panel.gameObject);
        LeanTween.alphaCanvas(panel, 0f, fadeDuration)
            .setEase(LeanTweenType.easeInOutQuad);

        yield return new WaitForSeconds(fadeDuration);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}