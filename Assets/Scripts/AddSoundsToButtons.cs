#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Автоматически добавляет UIButtonSound на все кнопки в сцене
/// </summary>
public class AddSoundsToButtons : EditorWindow
{
    [MenuItem("Tools/Audio/Add Sounds To All Buttons")]
    static void AddSoundsToAllButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        int addedCount = 0;

        foreach (Button button in allButtons)
        {
            if (button.GetComponent<UIButtonSound>() == null)
            {
                button.gameObject.AddComponent<UIButtonSound>();
                addedCount++;
            }
        }

        Debug.Log($"✅ Добавлено UIButtonSound на {addedCount} кнопок");
        EditorUtility.DisplayDialog("Готово", $"Звуки добавлены на {addedCount} кнопок!", "OK");
    }

    [MenuItem("Tools/Audio/Remove Sounds From All Buttons")]
    static void RemoveSoundsFromAllButtons()
    {
        UIButtonSound[] allSounds = FindObjectsOfType<UIButtonSound>(true);
        int removedCount = allSounds.Length;

        foreach (UIButtonSound sound in allSounds)
        {
            DestroyImmediate(sound);
        }

        Debug.Log($"❌ Удалено UIButtonSound с {removedCount} кнопок");
        EditorUtility.DisplayDialog("Готово", $"Звуки удалены с {removedCount} кнопок!", "OK");
    }
}
#endif