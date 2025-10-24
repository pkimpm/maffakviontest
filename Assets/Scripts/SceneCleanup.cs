using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCleanup : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllAudio();
        }

        LeanTween.cancelAll();
    }
}