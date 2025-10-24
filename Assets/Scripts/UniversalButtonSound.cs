using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UniversalButtonSound : MonoBehaviour
{
    [Header("Звуки")]
    [SerializeField] private string clickSound = "sound_ui_buttonclick_add";
    [SerializeField] private string hoverSound = "sound_ui_buttonhover";
    [SerializeField] private bool playClickSound = true;
    [SerializeField] private bool playHoverSound = true;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        
        if (playClickSound)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnDestroy()
    {
        if (button != null && playClickSound)
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(clickSound);
        }
    }

    public void OnPointerEnter()
    {
        if (playHoverSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(hoverSound);
        }
    }
}