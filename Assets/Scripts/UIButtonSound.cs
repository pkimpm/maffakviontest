using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Универсальный компонент для добавления звуков на UI кнопки
/// Добавьте этот скрипт на любую кнопку для автоматических звуков
/// </summary>
[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Звуки")]
    [SerializeField] private string clickSound = "sound_ui_buttonclick_add";
    [SerializeField] private string hoverSound = "sound_ui_buttonhover";
    
    [Header("Настройки")]
    [SerializeField] private bool playClickSound = true;
    [SerializeField] private bool playHoverSound = true;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playHoverSound && button.interactable && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(hoverSound);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playClickSound && button.interactable && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(clickSound);
        }
    }

    // Публичный метод для проигрывания клика из других мест
    public void PlayClick()
    {
        if (playClickSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(clickSound);
        }
    }
}