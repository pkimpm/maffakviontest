using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [Header("Настройки")]
    [SerializeField] private bool useHoverSound = true;
    [SerializeField] private string customHoverSound = "sound_ui_buttonhover";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useHoverSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(customHoverSound);
        }
    }
}