using UnityEngine;
using MapModule.Shapes;
using Core.ObjectPool;

namespace MapModule.Shapes
{
    public class ShapeFactory
    {
        private MapConfig mapConfig;
        private const string POOL_PREFIX = "Shape_";

        public ShapeFactory(MapConfig config)
        {
            mapConfig = config;
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var prefab in mapConfig.ShapePrefabs)
            {
                var shape = prefab.GetComponent<BaseShape>();
                if (shape != null)
                {
                    string poolId = POOL_PREFIX + shape.GetShapeType();
                    ObjectPool.Instance.CreatePool(poolId, prefab, 5, 20);  // 初始5个，最大20个
                }
            }
        }

        public BaseShape CreateShape(ShapeType type)
        {
            string poolId = POOL_PREFIX + type;
            var obj = ObjectPool.Instance.SpawnFromPool(poolId, Vector3.zero, Quaternion.identity);
            return obj?.GetComponent<BaseShape>();
        }

        public void RecycleShape(BaseShape shape)
        {
            if (shape == null) return;
            string poolId = POOL_PREFIX + shape.GetShapeType();
            ObjectPool.Instance.ReturnToPool(poolId, shape.gameObject);
        }
    }
} 