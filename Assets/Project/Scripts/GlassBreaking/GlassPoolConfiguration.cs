using UnityEngine;
using Core.ObjectPool;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Configures object pools for glass fragments with platform-specific settings
    /// </summary>
    public class GlassPoolConfiguration : MonoBehaviour
    {
        [Header("Fragment Prefabs")]
        public GameObject largeFragmentPrefab;
        public GameObject mediumFragmentPrefab;
        public GameObject smallFragmentPrefab;
        public GameObject particleSystemPrefab;

        [Header("Pool Configuration")]
        [SerializeField]
        private bool isMobilePlatform;
        
        [Tooltip("Pool size for PC platform")]
        public int pcPoolSize = 50;
        [Tooltip("Pool size for mobile platform")]
        public int mobilePoolSize = 35;

        [Header("Fragment Distribution")]
        [Range(0, 1)]
        public float largeFragmentRatio = 0.3f;
        [Range(0, 1)]
        public float mediumFragmentRatio = 0.5f;
        [Range(0, 1)]
        public float smallFragmentRatio = 0.2f;

        private ObjectPool objectPool;

        private void Awake()
        {
            objectPool = GetComponent<ObjectPool>();
            if (objectPool == null)
            {
                Debug.LogError("[GlassPoolConfiguration] ObjectPool component is missing!");
                return;
            }

            // Detect platform
            #if UNITY_ANDROID || UNITY_IOS
            isMobilePlatform = true;
            #else
            isMobilePlatform = false;
            #endif

            InitializePools();
        }

        private void InitializePools()
        {
            int totalPoolSize = isMobilePlatform ? mobilePoolSize : pcPoolSize;

            // Calculate sizes for each fragment pool
            int largePoolSize = Mathf.RoundToInt(totalPoolSize * largeFragmentRatio);
            int mediumPoolSize = Mathf.RoundToInt(totalPoolSize * mediumFragmentRatio);
            int smallPoolSize = Mathf.RoundToInt(totalPoolSize * smallFragmentRatio);
            int particlePoolSize = 5; // Fixed small number for particle systems

            // Create pools
            if (largeFragmentPrefab != null)
            {
                objectPool.CreatePool("LargeGlassFragments", largeFragmentPrefab, largePoolSize);
            }

            if (mediumFragmentPrefab != null)
            {
                objectPool.CreatePool("MediumGlassFragments", mediumFragmentPrefab, mediumPoolSize);
            }

            if (smallFragmentPrefab != null)
            {
                objectPool.CreatePool("SmallGlassFragments", smallFragmentPrefab, smallPoolSize);
            }

            if (particleSystemPrefab != null)
            {
                objectPool.CreatePool("GlassParticleSystems", particleSystemPrefab, particlePoolSize);
            }

            Debug.Log($"[GlassPoolConfiguration] Initialized pools for {(isMobilePlatform ? "Mobile" : "PC")} platform");
            Debug.Log($"Large fragments: {largePoolSize}, Medium: {mediumPoolSize}, Small: {smallPoolSize}, Particles: {particlePoolSize}");
        }

        /// <summary>
        /// Gets the appropriate pool ID based on fragment size
        /// </summary>
        public string GetPoolIdForSize(FragmentSize size)
        {
            switch (size)
            {
                case FragmentSize.Large:
                    return "LargeGlassFragments";
                case FragmentSize.Medium:
                    return "MediumGlassFragments";
                case FragmentSize.Small:
                    return "SmallGlassFragments";
                case FragmentSize.Particle:
                    return "GlassParticleSystems";
                default:
                    return "MediumGlassFragments";
            }
        }
    }

    /// <summary>
    /// Enum defining fragment size categories
    /// </summary>
    public enum FragmentSize
    {
        Large,   // >5cm
        Medium,  // 2-5cm
        Small,   // <2cm
        Particle // Particle system
    }
}
