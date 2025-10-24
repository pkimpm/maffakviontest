using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private TMP_Text hintNumberText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("Puzzle Input")]
    [SerializeField] private PuzzleInput puzzleInput;

    [Header("Звуки")] // 🔥 Новый раздел
    [SerializeField] private string buttonClickSound = "sound_ui_buttonclick_add";
    [SerializeField] private string buttonHoverSound = "sound_ui_buttonhover";

    public bool IsHintOpen { get; private set; }

    public enum HintButtonType { Close, Next }
    private HintButtonType lastButtonType;

    public event Action OnHintClosed;
    public event Action OnNextHintClosed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        panel.alpha = 0f;
        panel.blocksRaycasts = false;
        panel.interactable = false;

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => 
            {
                PlayButtonClick();
                Hide(HintButtonType.Next);
            });
            AddHoverSound(nextButton);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => 
            {
                PlayButtonClick();
                Hide(HintButtonType.Close);
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

    public void ShowHint(HintData hint, HintButtonType buttonType)
    {
        if (hint == null) return;
        
        IsHintOpen = true;
        
        hintNumberText.text = hint.hintNumber;
        hintText.text = hint.hintText;
        nextButton.gameObject.SetActive(buttonType == HintButtonType.Next);
        closeButton.gameObject.SetActive(buttonType == HintButtonType.Close);
        lastButtonType = buttonType;

        if (puzzleInput != null) puzzleInput.enabled = false;

        LeanTween.cancel(panel.gameObject);
        LeanTween.alphaCanvas(panel, 1f, fadeDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                panel.blocksRaycasts = true;
                panel.interactable = true;
            });
    }

    private void Hide(HintButtonType buttonType)
    {
        if (!IsHintOpen) return;

        IsHintOpen = false;
        lastButtonType = buttonType;

        LeanTween.cancel(panel.gameObject);
        LeanTween.alphaCanvas(panel, 0f, fadeDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                panel.blocksRaycasts = false;
                panel.interactable = false;
                
                if (puzzleInput != null && !DialogueManager.IsDialogueActive)
                {
                    puzzleInput.enabled = true;
                }

                OnHintClosed?.Invoke();
                if (lastButtonType == HintButtonType.Next) OnNextHintClosed?.Invoke();
            });
    }

    public IEnumerator WaitUntilClosed()
    {
        while (IsHintOpen) yield return null;
    }
}