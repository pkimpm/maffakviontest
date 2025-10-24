using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Точка входа в игру - инициализирует все системы
/// </summary>
public class Bootstrap : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string firstSceneName = "MainMenu";
    
    private void Awake()
    {
        Debug.Log("=== ТОЧКА ВХОДА: Bootstrap ===");
        
        // 1. Инициализация менеджеров
        InitializeManagers();
        
        // 2. Загрузка настроек
        LoadSettings();
        
        // 3. Настройка Application
        SetupApplication();
    }
    
    private void Start()
    {
        // Загружаем первую игровую сцену
        SceneManager.LoadScene(firstSceneName);
    }
    
    private void InitializeManagers()
    {
        // Убедитесь что AudioManager существует
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager не найден!");
        }
        
        // Инициализация других синглтонов
        // GameManager, SaveSystem и т.д.
    }
    
    private void LoadSettings()
    {
        // Загрузка настроек из PlayerPrefs
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 0);
        Application.targetFrameRate = PlayerPrefs.GetInt("TargetFPS", 60);
    }
    
    private void SetupApplication()
    {
        // Настройки для WebGL
        #if UNITY_WEBGL && !UNITY_EDITOR
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        #endif
        
        // Не давать устройству засыпать
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}