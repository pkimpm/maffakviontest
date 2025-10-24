using UnityEngine;


public class VFXAutoSound : MonoBehaviour
{
    [Header("Звук")]
    [SerializeField] private string soundName;
    [SerializeField] private bool ignoreGamePause = true;

    [Header("VFX компоненты")]
    [SerializeField] private ParticleSystem[] vfxList;

    private bool[] wasPlaying;

    private void Awake()
    {
        if (vfxList != null)
        {
            wasPlaying = new bool[vfxList.Length];
        }
    }

    private void Update()
    {
        if (vfxList == null || string.IsNullOrEmpty(soundName)) return;

        for (int i = 0; i < vfxList.Length; i++)
        {
            if (vfxList[i] == null) continue;

            bool isPlaying = vfxList[i].isPlaying;

            if (isPlaying && !wasPlaying[i])
            {
                PlaySound();
            }

            wasPlaying[i] = isPlaying;
        }
    }

    private void PlaySound()
    {
        if (AudioManager.Instance == null) return;

        if (ignoreGamePause)
        {
            AudioManager.Instance.PlaySFXUnpausable(soundName);
        }
        else
        {
            AudioManager.Instance.PlaySFX(soundName);
        }

        Debug.Log($"🔊 VFXAutoSound: Playing '{soundName}'");
    }
}