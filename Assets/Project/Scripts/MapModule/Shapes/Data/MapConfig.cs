using UnityEngine;
using MapModule.Shapes;

namespace MapModule.Shapes
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Map/MapConfig")]
    public class MapConfig : ScriptableObject
    {
        [Header("形状预制体")]
        public GameObject[] ShapePrefabs;

        [Header("时间配置")]
        public float ShapeChangeInterval = 40f;
        public float TransitionDuration = 1f;

        [Header("区域配置")]
        public Vector2 CentralAreaSize = new Vector2(3, 10);
        public Vector2 ActiveAreaSize = new Vector2(3, 7);
        public float VerticalFloatRange = 1f;

        [Header("形状配置")]
        public ShapeTypeConfig[] ShapeConfigs;

        [System.Serializable]
        public class ShapeTypeConfig
        {
            public ShapeType Type;
            public Vector2 Size;
            public float SpawnChance = 1f;
            public bool Enabled = true;
        }

        public Vector2 GetShapeSize(ShapeType type)
        {
            foreach (var config in ShapeConfigs)
            {
                if (config.Type == type && config.Enabled)
                {
                    return config.Size;
                }
            }
            return Vector2.one; // 默认大小
        }

        public bool IsShapeEnabled(ShapeType type)
        {
            foreach (var config in ShapeConfigs)
            {
                if (config.Type == type)
                {
                    return config.Enabled;
                }
            }
            return false;
        }
    }
} 