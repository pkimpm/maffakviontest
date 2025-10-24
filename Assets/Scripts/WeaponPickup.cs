using UnityEngine;

[RequireComponent(typeof(PulsingHighlight))]
public class WeaponPickup : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private VillainController villain;

    [Header("Анимация появления")]
    [SerializeField] private float riseDistance = 0.5f;
    [SerializeField] private float riseDuration = 0.5f;

    [Header("Анимация при нажатии")]
    [SerializeField] private float sinkDistance = 0.3f;
    [SerializeField] private float pickupAnimationDuration = 0.4f;

    [Header("Звуки")]
    [SerializeField] private string weaponSpawnSound = "sound_ui_weaponspawn";
    [SerializeField] private string weaponPickupSound = "sound_ui_weaponuse";

    private bool isClickable;
    private Vector3 startLocalPos;
    private PulsingHighlight _highlight;

    private void Awake()
    {
        _highlight = GetComponent<PulsingHighlight>();
    }

    private void OnEnable()
    {
        isClickable = false;
        
        PlaySound(weaponSpawnSound);
        
        startLocalPos = transform.localPosition; 
        Vector3 raisedPos = startLocalPos + new Vector3(0, riseDistance, 0);
        transform.localPosition = startLocalPos;

        LeanTween.moveLocal(gameObject, raisedPos, riseDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() =>
            {
                isClickable = true;
                startLocalPos = transform.localPosition;
                _highlight.StartHighlight();
            });
    }

    public void Pickup()
    {
        if (!isClickable) return;
        isClickable = false;

        PlaySound(weaponPickupSound);
        Debug.Log("🔊 Weapon picked up - playing weaponuse sound");

        _highlight.StopHighlight();

        Vector3 targetPos = startLocalPos - new Vector3(0, sinkDistance, 0);
        
        LeanTween.moveLocal(gameObject, targetPos, pickupAnimationDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                if (villain != null)
                {
                    villain.EnableTargeting();
                }
            });
    }

    private void PlaySound(string soundName)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            Debug.LogWarning("⚠️ Sound name is empty!");
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(soundName);
            Debug.Log($"🔊 Playing sound: {soundName}");
        }
        else
        {
            Debug.LogError("❌ AudioManager.Instance is NULL!");
        }
    }
}