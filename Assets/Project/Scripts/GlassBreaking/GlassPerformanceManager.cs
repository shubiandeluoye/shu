using UnityEngine;
using Core.ObjectPool;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Manages performance optimizations for glass breaking system
    /// </summary>
    [DefaultExecutionOrder(-100)]  // 确保在其他脚本之前执行
    public class GlassPerformanceManager : MonoBehaviour
    {
        [System.Serializable]
        public class PlatformSettings
        {
            public int maxFragments = 50;
            public float cullingDistance = 10f;
            public bool useSimplifiedPhysics = false;
            public bool useLOD = false;
            public float physicsUpdateRate = 0.02f;
            [Range(0, 1)]
            public float particleDensity = 1f;
        }

        [Header("平台设置")]
        public PlatformSettings pcSettings;
        public PlatformSettings mobileSettings;

        [Header("Platform Settings")]
        [SerializeField]
        private bool isMobilePlatform;
        
        [Header("LOD Settings")]
        [Tooltip("Distance for full detail")]
        public float closeDistance = 3f;
        [Tooltip("Distance for medium detail")]
        public float mediumDistance = 6f;
        [Tooltip("Distance for minimum detail")]
        public float farDistance = 8f;
        
        [Header("Mobile Optimization")]
        [Tooltip("Physics update interval for distant fragments")]
        public float physicsUpdateInterval = 0.1f;
        [Tooltip("Maximum fragments for mobile")]
        public int mobileMaxFragments = 35;
        
        [Header("References")]
        [SerializeField]
        private Camera mainCamera;
        private GlassPoolConfiguration poolConfig;
        
        private void Awake()
        {
            poolConfig = GetComponent<GlassPoolConfiguration>();
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            // Detect platform
            #if UNITY_ANDROID || UNITY_IOS
            isMobilePlatform = true;
            #else
            isMobilePlatform = false;
            #endif
            
            ApplyPlatformSettings();
        }
        
        private void ApplyPlatformSettings()
        {
            PlatformSettings settings = GetCurrentPlatformSettings();
            ApplySettings(settings);
        }

        private PlatformSettings GetCurrentPlatformSettings()
        {
            #if UNITY_ANDROID || UNITY_IOS
                return mobileSettings;
            #else
                return pcSettings;
            #endif
        }

        private void ApplySettings(PlatformSettings settings)
        {
            // 应用到所有玻璃控制器
            var glassControllers = FindObjectsOfType<GlassBreakingController>();
            foreach (var controller in glassControllers)
            {
                if (controller != null)
                {
                    controller.maxFragments = settings.maxFragments;
                    controller.cullingDistance = settings.cullingDistance;
                }
            }

            // 设置物理更新
            if (settings.useSimplifiedPhysics)
            {
                Physics.defaultSolverIterations = 4;
                Physics.defaultSolverVelocityIterations = 1;
                Time.fixedDeltaTime = settings.physicsUpdateRate;
            }

            // 设置LOD
            if (settings.useLOD)
            {
                QualitySettings.lodBias = 0.5f;
            }

            // 设置粒子系统密度
            var particleSystems = FindObjectsOfType<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                var main = ps.main;
                main.maxParticles = Mathf.RoundToInt(main.maxParticles * settings.particleDensity);
            }
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateSettings(pcSettings);
            ValidateSettings(mobileSettings);
        }

        private void ValidateSettings(PlatformSettings settings)
        {
            if (settings != null)
            {
                settings.maxFragments = Mathf.Max(1, settings.maxFragments);
                settings.cullingDistance = Mathf.Max(1f, settings.cullingDistance);
                settings.physicsUpdateRate = Mathf.Clamp(settings.physicsUpdateRate, 0.01f, 0.05f);
            }
        }
        #endif

        public void UpdatePerformanceSettings()
        {
            ApplyPlatformSettings();
        }
        
        /// <summary>
        /// Gets the appropriate LOD level based on distance
        /// </summary>
        public LODLevel GetLODLevel(Vector3 position)
        {
            if (mainCamera == null) return LODLevel.Full;
            
            float distance = Vector3.Distance(mainCamera.transform.position, position);
            
            if (distance <= closeDistance)
                return LODLevel.Full;
            else if (distance <= mediumDistance)
                return LODLevel.Medium;
            else if (distance <= farDistance)
                return LODLevel.Minimal;
            else
                return LODLevel.Culled;
        }
        
        /// <summary>
        /// Updates fragment physics based on LOD level
        /// </summary>
        public void UpdateFragmentPhysics(GlassFragment fragment, LODLevel level)
        {
            if (fragment == null) return;
            
            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb == null) return;
            
            switch (level)
            {
                case LODLevel.Full:
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    break;
                    
                case LODLevel.Medium:
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    break;
                    
                case LODLevel.Minimal:
                    rb.isKinematic = true;
                    break;
                    
                case LODLevel.Culled:
                    // Return to pool if too far
                    var poolObject = fragment.GetComponent<PoolObject>();
                    if (poolObject != null)
                    {
                        poolObject.RecycleNow();
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Gets the maximum fragment count based on platform
        /// </summary>
        public int GetMaxFragments()
        {
            return isMobilePlatform ? mobileMaxFragments : 50;
        }
    }
    
    /// <summary>
    /// Level of Detail settings for fragments
    /// </summary>
    public enum LODLevel
    {
        Full,    // Full physics and effects
        Medium,  // Simplified physics
        Minimal, // Static display only
        Culled   // Disabled/recycled
    }
}
