using UnityEngine;

public class VFXSoundPlayer : MonoBehaviour
{
    [Header("VFX и Звуки")]
    [SerializeField] private ParticleSystem[] vfxList;
    [SerializeField] private string soundName;

    public void PlayEffects()
    {
        if (!string.IsNullOrEmpty(soundName) && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXUnpausable(soundName);
        }

        foreach (var vfx in vfxList)
        {
            if (vfx != null)
            {
                vfx.Play();
            }
        }
    }
}