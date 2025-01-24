using UnityEngine;
using MapModule.Core.Data;
using MapModule.Core.Utils;

namespace MapModule.Unity.Configs
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Map/Map Config")]
    public class MapConfigSO : ScriptableObject
    {
        [Header("区域设置")]
        [SerializeField] private Vector2 centralAreaSize = new(3, 10);
        [SerializeField] private Vector2 activeAreaSize = new(3, 7);
        [SerializeField] private float verticalFloatRange = 1f;

        [Header("时间设置")]
        [SerializeField] private float shapeChangeInterval = 40f;
        [SerializeField] private float transitionDuration = 0.5f;

        [Header("形状配置")]
        [SerializeField] private ShapeTypeSO[] shapeConfigs;

        public MapConfig ToMapConfig()
        {
            return new MapConfig
            {
                CentralAreaSize = new Vector2D(centralAreaSize.x, centralAreaSize.y),
                ActiveAreaSize = new Vector2D(activeAreaSize.x, activeAreaSize.y),
                VerticalFloatRange = verticalFloatRange,
                ShapeChangeInterval = shapeChangeInterval,
                TransitionDuration = transitionDuration,
                ShapeConfigs = System.Array.ConvertAll(shapeConfigs, config => config.ToShapeTypeConfig())
            };
        }
    }
} 