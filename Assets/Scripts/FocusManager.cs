using UnityEngine;

/// <summary>
/// Блокирует весь UI при потере фокуса окна
/// </summary>
public class FocusManager : MonoBehaviour
{
    public static FocusManager Instance { get; private set; }

    [Tooltip("Перетащите FocusBlocker из сцены")]
    [SerializeField] private CanvasGroup focusBlocker;

    // 🔥 ПУБЛИЧНЫЙ ФЛАГ: Другие скрипты могут проверить состояние фокуса
    public bool IsApplicationFocused { get; private set; } = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (focusBlocker == null)
        {
            Debug.LogError("❌ FocusBlocker не назначен!");
            enabled = false;
            return;
        }

        focusBlocker.alpha = 0;
        focusBlocker.blocksRaycasts = false;
    }

    private void Update()
    {
        if (focusBlocker == null) return;

        bool isFocused = Application.isFocused;

        if (IsApplicationFocused != isFocused)
        {
            IsApplicationFocused = isFocused;
        }

        bool shouldBlock = !isFocused;

        if (focusBlocker.blocksRaycasts != shouldBlock)
        {
            focusBlocker.blocksRaycasts = shouldBlock;
            
            if (shouldBlock)
            {
                focusBlocker.transform.SetAsLastSibling();
                Debug.Log("🪟 Фокус потерян. UI заблокирован.");
            }
            else
            {
                Debug.Log("🪟 Фокус восстановлен. UI разблокирован.");
            }
        }
    }
}