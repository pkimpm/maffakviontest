using UnityEngine;
using System;
using System.Collections;

public class FinalBattleManager : MonoBehaviour
{
    [SerializeField] private ItemReceivedPanel itemPanel;
    [SerializeField] private GameObject weaponModel;
    [SerializeField] private VillainController villain;

    [Header("Puzzle Input для битвы")]
    [SerializeField] private PuzzleInput battleInput;

    [Header("Objects to toggle after battle")]
    [SerializeField] private GameObject[] objectsToEnable;
    [SerializeField] private GameObject[] objectsToDisable;

    [Header("Анимация исцеления статуи")]
    [SerializeField] private Animator[] animatorsToPlay;
    [SerializeField] private string animationName = "Healing";
    [SerializeField] private int animationLayer = 0;
    [SerializeField] private bool playReverse = true;
    [SerializeField] private float delayBeforeAnimation = 0.5f;

    [Header("Активация злодейки")]
    [SerializeField] private GameObject villainObject;
    [SerializeField] private float villainActivationDelay = 0.5f;

    [Header("Звуки заклинаний")]
    [Tooltip("Звук заклинания SpellOn (активация статуи)")]
    [SerializeField] private string spellOnSound = "sfx_spellon_statue";
    
    [Tooltip("Звук заклинания SpellOff после 5-го удара")]
    [SerializeField] private string spellOffLastPunchSound = "sfx_spelloff_lastpunch";
    
    [Tooltip("Звук появления злодейки")]
    [SerializeField] private string villainSpawnSound = "sfx_velianspawn";

    [Header("VFX для SpellOn")]
    [Tooltip("VFX которые активируются при SpellOn (исцеление статуи)")]
    [SerializeField] private ParticleSystem[] spellOnVFX;

    [Header("VFX для SpellOff")]
    [Tooltip("VFX которые активируются при SpellOff (после победы)")]
    [SerializeField] private ParticleSystem[] spellOffVFX;

    [Header("VFX для появления злодейки")]
    [Tooltip("VFX которые активируются при появлении злодейки")]
    [SerializeField] private ParticleSystem[] villainSpawnVFX;

    public event Action OnBattleFinished;

    private void Awake()
    {
        if (villain != null)
            villain.OnDefeated += HandleBattleFinished;
        
        if (villainObject != null)
            villainObject.SetActive(false);
    }

    public void ActivateVillain()
    {
        StartCoroutine(ActivateVillainCoroutine());
    }

    private IEnumerator ActivateVillainCoroutine()
    {
        Debug.Log($"🎬 [{Time.frameCount}] Starting villain activation...");
        
        yield return new WaitForSeconds(villainActivationDelay);
        
        if (villainObject != null)
        {
            villainObject.SetActive(true);
            Debug.Log($"✅ [{Time.frameCount}] Villain object activated");
            
            yield return null;
            yield return null;
            
            PlaySoundUnpausable(villainSpawnSound);
            PlayVFXList(villainSpawnVFX);
            
            Debug.Log($"🔊 [{Time.frameCount}] Villain spawn sound and VFX triggered");
        }

        if (villain != null)
        {
            villain.EnableTargetingSilent();
        }
    }

    public void TriggerItemPhase()
    {
        if (itemPanel != null)
        {
            itemPanel.OnContinue += OnItemConfirmed;
            itemPanel.Show("Получено оружие!");
        }
        else
        {
            OnItemConfirmed();
        }
    }

    private void OnItemConfirmed()
    {
        if (itemPanel != null)
            itemPanel.OnContinue -= OnItemConfirmed;

        if (weaponModel != null) 
            weaponModel.SetActive(true);
        
        if (battleInput != null) 
            battleInput.enabled = true;

    }

    private void HandleBattleFinished()
    {
        if (battleInput != null)
            battleInput.enabled = false;

        PlaySoundUnpausable(spellOffLastPunchSound);
        PlayVFXList(spellOffVFX);

        StartCoroutine(PlayStatueHealingAnimation());
    }

    private IEnumerator PlayStatueHealingAnimation()
    {
        yield return new WaitForSeconds(delayBeforeAnimation);

        PlaySoundUnpausable(spellOnSound);
        PlayVFXList(spellOnVFX);

        foreach (var go in objectsToEnable)
            if (go != null) go.SetActive(true);

        foreach (var go in objectsToDisable)
            if (go != null) go.SetActive(false);

        if (animatorsToPlay != null && animatorsToPlay.Length > 0)
        {
            float maxDuration = 0f;

            foreach (Animator animator in animatorsToPlay)
            {
                if (animator == null || animator.runtimeAnimatorController == null) continue;

                animator.speed = playReverse ? -1f : 1f;

                if (playReverse)
                {
                    animator.Play(animationName, animationLayer, 1f);
                }
                else
                {
                    animator.Play(animationName, animationLayer, 0f);
                }

                AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(animationLayer);
                if (clipInfos.Length > 0)
                {
                    float clipLength = clipInfos[0].clip.length;
                    if (clipLength > maxDuration)
                        maxDuration = clipLength;
                }
                else
                {
                    foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
                    {
                        if (clip.name == animationName)
                        {
                            if (clip.length > maxDuration)
                                maxDuration = clip.length;
                            break;
                        }
                    }
                }
            }

            if (maxDuration > 0)
            {
                yield return new WaitForSeconds(maxDuration);

                foreach (Animator animator in animatorsToPlay)
                {
                    if (animator != null)
                    {
                        animator.speed = 0f;
                    }
                }
            }
        }

        OnBattleFinished?.Invoke();
    }

    private void PlaySoundUnpausable(string soundName)
    {
        if (string.IsNullOrEmpty(soundName)) return;
        if (AudioManager.Instance == null || AudioManager.Instance.soundLibrary == null) return;
        
        AudioClip clip = AudioManager.Instance.soundLibrary.GetClip(soundName);
        if (clip == null) return;

        AudioManager.Instance.PlaySFXUnpausable(soundName);
    }

    private void PlayVFXList(ParticleSystem[] vfxList)
    {
        if (vfxList == null) return;
        foreach (var vfx in vfxList)
        {
            if (vfx != null) vfx.Play();
        }
    }
    
    public PuzzleInput GetBattleInput()
    {
        return battleInput;
    }
}