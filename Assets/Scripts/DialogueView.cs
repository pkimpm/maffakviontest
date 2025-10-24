using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class DialogueView : MonoBehaviour
{
    [Header("UI Компоненты")]
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image speakerIcon;
    [SerializeField] private Button continueButton;

    [Header("Анимация")]
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("Защита от быстрого нажатия")]
    [SerializeField] private float clickCooldown = 0.3f;

    public float FadeDuration => fadeDuration;
    public Action OnContinueClick;

    private CanvasGroup canvasGroup;
    private float lastClickTime = 0f;
    private bool isProcessingClick = false;
    private bool shouldBlockButton = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (continueButton == null)
        {
            continueButton = GetComponentInChildren<Button>(true);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueButtonClicked);
            
            Navigation nav = continueButton.navigation;
            nav.mode = Navigation.Mode.None;
            continueButton.navigation = nav;
        }
    }

    private void OnDestroy()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
        }
    }

    private void LateUpdate()
    {
        if (continueButton == null) return;

        if (shouldBlockButton && continueButton.interactable)
        {
            continueButton.interactable = false;
        }
    }

    private void OnContinueButtonClicked()
    {
        if (!continueButton.interactable)
        {
            Debug.Log("❌ Button clicked but not interactable!");
            return;
        }

        float timeSinceLastClick = Time.unscaledTime - lastClickTime;
        
        if (isProcessingClick || timeSinceLastClick < clickCooldown)
        {
            Debug.Log($"❌ Click blocked: cooldown");
            return;
        }

        Debug.Log($"✅ Button clicked!");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI("sound_ui_dialoguenextbutton");
        }

        if (continueButton != null)
        {
            continueButton.interactable = false;
        }

        isProcessingClick = true;
        lastClickTime = Time.unscaledTime;
        
        OnContinueClick?.Invoke();

        StartCoroutine(ResetClickProtection());
    }

    private IEnumerator ResetClickProtection()
    {
        yield return new WaitForSecondsRealtime(clickCooldown);
        isProcessingClick = false;
    }

    public void SetContent(DialogueNode node)
    {
        if (speakerNameText != null) 
            speakerNameText.text = node.speakerName;
            
        if (dialogueText != null) 
            dialogueText.text = node.dialogueLine;
        
        if (speakerIcon != null)
        {
            if (node.speakerIcon != null)
            {
                speakerIcon.sprite = node.speakerIcon;
                speakerIcon.enabled = true;
            }
            else
            {
                speakerIcon.enabled = false;
            }
        }
    }

    public void Show()
    {
        if (canvasGroup == null) return;
        
        isProcessingClick = false;
        
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration).setIgnoreTimeScale(true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        if (canvasGroup == null) return;
        
        isProcessingClick = true;
        
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration).setIgnoreTimeScale(true);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void SetContinueInteractable(bool isInteractable)
    {
        if (continueButton != null)
        {
            continueButton.interactable = isInteractable;
            shouldBlockButton = !isInteractable;
            
            if (isInteractable)
            {
                isProcessingClick = false;
            }
        }
    }

    public void ForceDisableButton()
    {
        if (continueButton != null)
        {
            continueButton.interactable = false;
            shouldBlockButton = true;
        }
    }

    public bool IsButtonInteractable()
    {
        return continueButton != null && continueButton.interactable;
    }

    public bool IsProcessingClick()
    {
        return isProcessingClick;
    }
}