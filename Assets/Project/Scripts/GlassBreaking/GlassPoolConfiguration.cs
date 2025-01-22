using UnityEngine;
using Core.ObjectPool;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Configures object pools for glass fragments with platform-specific settings
    /// </summary>
    [CreateAssetMenu(fileName = "GlassPoolConfig", menuName = "Glass Breaking/Pool Configuration")]
    public class GlassPoolConfiguration : ScriptableObject
    {
        [System.Serializable]
        public class PoolConfig
        {
            public string poolId;
            public GameObject prefab;
            public int size = 20;
            public float autoRecycleTime = 5f;  // 自动回收时间
        }

        [Header("碎片池配置")]
        public PoolConfig[] fragmentPools;

        [Header("特效池配置")]
        public PoolConfig particlePool;

        public void InitializePools()
        {
            if (ObjectPool.Instance == null)
            {
                Debug.LogError("ObjectPool instance not found!");
                return;
            }

            // 初始化碎片池
            foreach (var config in fragmentPools)
            {
                if (config.prefab != null)
                {
                    ObjectPool.Instance.CreatePool(config.poolId, config.prefab, config.size);
                    // 预热对象池
                    ObjectPool.Instance.WarmupPool(config.poolId, config.size);
                }
            }

            // 初始化特效池
            if (particlePool.prefab != null)
            {
                ObjectPool.Instance.CreatePool(particlePool.poolId, particlePool.prefab, particlePool.size);
                ObjectPool.Instance.WarmupPool(particlePool.poolId, particlePool.size);
            }
        }

        public void ClearPools()
        {
            if (ObjectPool.Instance == null) return;

            // 清理碎片池
            foreach (var config in fragmentPools)
            {
                ObjectPool.Instance.TrimPool(config.poolId, 0);
            }

            // 清理特效池
            ObjectPool.Instance.TrimPool(particlePool.poolId, 0);
        }

        private void OnValidate()
        {
            // 验证配置
            foreach (var config in fragmentPools)
            {
                ValidateConfig(config);
            }
            ValidateConfig(particlePool);
        }

        private void ValidateConfig(PoolConfig config)
        {
            if (config.size < 0)
            {
                config.size = 0;
            }
            if (config.autoRecycleTime < 0)
            {
                config.autoRecycleTime = 0;
            }
            if (string.IsNullOrEmpty(config.poolId) && config.prefab != null)
            {
                config.poolId = config.prefab.name + "Pool";
            }
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
