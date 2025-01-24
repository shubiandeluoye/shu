using UnityEngine;
using System.Collections.Generic;
using MapModule.Core.Data;

namespace MapModule.Unity.Utils
{
    public class MapResourceManager : MonoBehaviour
    {
        private static MapResourceManager instance;
        public static MapResourceManager Instance => instance;

        [SerializeField] private MapConfigSO mapConfig;
        [SerializeField] private ShapeTypeSO[] shapeTypes;
        [SerializeField] private ShapeConfigSO[] shapeConfigs;

        private Dictionary<ShapeType, ShapeTypeSO> typeConfigs;
        private Dictionary<ShapeType, ShapeConfigSO> shapeConfigMap;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                InitializeConfigs();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeConfigs()
        {
            typeConfigs = new Dictionary<ShapeType, ShapeTypeSO>();
            shapeConfigMap = new Dictionary<ShapeType, ShapeConfigSO>();

            foreach (var config in shapeTypes)
            {
                typeConfigs[config.ToShapeTypeConfig().Type] = config;
            }

            foreach (var config in shapeConfigs)
            {
                shapeConfigMap[config.ToShapeConfig().Type] = config;
            }
        }

        public MapConfig GetMapConfig() => mapConfig.ToMapConfig();
        public ShapeTypeSO GetShapeType(ShapeType type) => typeConfigs.GetValueOrDefault(type);
        public ShapeConfigSO GetShapeConfig(ShapeType type) => shapeConfigMap.GetValueOrDefault(type);
    }
} 