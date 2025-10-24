// Замените ваш существующий скрипт на этот:

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OcclusionCullingHelper : MonoBehaviour
{
    [Header("Occlusion Settings")]
    [Tooltip("Минимальный размер объекта для Occluder")]
    public float minOccluderSize = 5f;
    
    [Tooltip("Минимальный размер объекта для Occludee")]
    public float minOccludeeSize = 1f;
    
    [Header("Auto Setup")]
    [Tooltip("Автоматически пометить статичные объекты")]
    public bool autoMarkStatic = true;
    
    [Tooltip("Искать в дочерних объектах")]
    public bool includeChildren = true;

    [Header("Leaves Protection")] // 🔥 НОВОЕ
    [Tooltip("Отключить Occlusion для листьев")]
    public bool disableOcclusionForLeaves = true;

    [Header("Camera Culling Override")]
    [Tooltip("Всегда отрисовывать объекты перед камерой")]
    public bool alwaysRenderInFront = true;
    
    [Tooltip("Камера для проверки")]
    public Camera targetCamera;
    
    [Tooltip("Расстояние принудительной отрисовки")]
    public float forceRenderDistance = 100f; // 🔥 Увеличено для листьев
    
    [Tooltip("Угол обзора для принудительной отрисовки")]
    [Range(0, 180)]
    public float forceRenderAngle = 120f; // 🔥 Увеличено

    [Header("Performance")]
    [Tooltip("Проверять каждые N кадров")]
    public int updateFrequency = 5;

    private Renderer[] allRenderers;
    private int frameCounter = 0;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (alwaysRenderInFront)
        {
            allRenderers = includeChildren 
                ? GetComponentsInChildren<Renderer>(true) 
                : GetComponents<Renderer>();
            
            // 🔥 Принудительно включаем все рендереры при старте
            ForceEnableAllRenderers();
        }
    }

    // 🔥 НОВЫЙ МЕТОД: Принудительно включает все рендереры
    private void ForceEnableAllRenderers()
    {
        if (allRenderers == null) return;

        foreach (var renderer in allRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = true;
                renderer.forceRenderingOff = false; // 🔥 Отключаем принудительное скрытие
            }
        }

        Debug.Log($"🍃 ForceEnabled {allRenderers.Length} renderers");
    }

    private void Start()
    {
        // 🔥 Дополнительная проверка после загрузки сцены
        if (alwaysRenderInFront)
        {
            Invoke(nameof(ForceEnableAllRenderers), 0.1f);
        }
    }

    private void Update()
    {
        if (!alwaysRenderInFront || targetCamera == null || allRenderers == null)
            return;

        frameCounter++;
        if (frameCounter >= updateFrequency)
        {
            frameCounter = 0;
            UpdateRenderersVisibility();
        }
    }

    private void UpdateRenderersVisibility()
    {
        Vector3 cameraPos = targetCamera.transform.position;
        Vector3 cameraForward = targetCamera.transform.forward;

        foreach (var renderer in allRenderers)
        {
            if (renderer == null) continue;

            Vector3 toRenderer = renderer.transform.position - cameraPos;
            float distance = toRenderer.magnitude;
            float angle = Vector3.Angle(cameraForward, toRenderer.normalized);

            bool shouldForceRender = distance <= forceRenderDistance && angle <= forceRenderAngle;

            if (shouldForceRender && !renderer.enabled)
            {
                renderer.enabled = true;
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Setup Occlusion Culling")]
    public void SetupOcclusionCulling()
    {
        Transform[] transforms = includeChildren 
            ? GetComponentsInChildren<Transform>() 
            : new Transform[] { transform };
        
        int occluderCount = 0;
        int occludeeCount = 0;
        int staticCount = 0;
        int leavesExcluded = 0; // 🔥 НОВОЕ
        
        foreach (var t in transforms)
        {
            if (t == transform) continue;
            
            MeshRenderer renderer = t.GetComponent<MeshRenderer>();
            if (renderer == null) continue;

            // 🔥 НОВОЕ: Проверяем если это листья
            bool isLeaf = false;
            if (disableOcclusionForLeaves)
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat != null && (mat.shader.name.Contains("Leaves") || mat.name.Contains("Leaves")))
                    {
                        isLeaf = true;
                        break;
                    }
                }
            }

            // 🔥 Пропускаем листья
            if (isLeaf)
            {
                // Убираем все static флаги с листьев
                GameObjectUtility.SetStaticEditorFlags(t.gameObject, 0);
                leavesExcluded++;
                Debug.Log($"🍃 Excluded leaf from occlusion: {t.name}");
                continue;
            }
            
            Bounds bounds = renderer.bounds;
            float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            
            if (autoMarkStatic && !t.gameObject.isStatic)
            {
                GameObjectUtility.SetStaticEditorFlags(t.gameObject, 
                    StaticEditorFlags.OccluderStatic | 
                    StaticEditorFlags.OccludeeStatic);
                staticCount++;
            }
            
            if (size >= minOccluderSize)
            {
                GameObjectUtility.SetStaticEditorFlags(t.gameObject, 
                    GameObjectUtility.GetStaticEditorFlags(t.gameObject) | 
                    StaticEditorFlags.OccluderStatic);
                occluderCount++;
            }
            
            if (size >= minOccludeeSize)
            {
                GameObjectUtility.SetStaticEditorFlags(t.gameObject, 
                    GameObjectUtility.GetStaticEditorFlags(t.gameObject) | 
                    StaticEditorFlags.OccludeeStatic);
                occludeeCount++;
            }
        }
        
        Debug.Log($"Occlusion Culling Setup Complete!\n" +
                  $"Static: {staticCount} | Occluders: {occluderCount} | Occludees: {occludeeCount}\n" +
                  $"🍃 Leaves Excluded: {leavesExcluded}");
    }
    
    [ContextMenu("Disable Occlusion for All Leaves")]
    public void DisableOcclusionForAllLeaves()
    {
        Transform[] transforms = includeChildren 
            ? GetComponentsInChildren<Transform>() 
            : new Transform[] { transform };
        
        int count = 0;
        foreach (var t in transforms)
        {
            MeshRenderer renderer = t.GetComponent<MeshRenderer>();
            if (renderer == null) continue;

            bool isLeaf = false;
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat != null && (mat.shader.name.Contains("Leaves") || mat.name.Contains("Leaves")))
                {
                    isLeaf = true;
                    break;
                }
            }

            if (isLeaf)
            {
                GameObjectUtility.SetStaticEditorFlags(t.gameObject, 0);
                count++;
            }
        }
        
        Debug.Log($"🍃 Disabled occlusion for {count} leaves");
    }

    [ContextMenu("Bake Occlusion Culling (Conservative)")]
    public void BakeOcclusion()
    {
        StaticOcclusionCulling.smallestOccluder = minOccluderSize;
        StaticOcclusionCulling.smallestHole = 0.5f; // 🔥 Увеличено
        StaticOcclusionCulling.backfaceThreshold = 100f;
        
        StaticOcclusionCulling.Compute();
        Debug.Log("Occlusion Culling baked with conservative settings!");
    }

    [ContextMenu("Clear Occlusion Culling")]
    public void ClearOcclusion()
    {
        StaticOcclusionCulling.Clear();
        Debug.Log("Occlusion Culling data cleared!");
    }
#endif

    private void OnDrawGizmosSelected()
    {
        if (!alwaysRenderInFront || targetCamera == null) return;

        Gizmos.color = Color.green;
        Vector3 cameraPos = targetCamera.transform.position;
        
        // Рисуем сферу видимости
        Gizmos.DrawWireSphere(cameraPos, forceRenderDistance);
    }
}