using UnityEngine;
using MapModule.Core.Data;
using MapModule.Core.Utils;

namespace MapModule.Unity.Configs
{
    [CreateAssetMenu(fileName = "ShapeType", menuName = "Map/Shape Type")]
    public class ShapeTypeSO : ScriptableObject
    {
        [Header("基础设置")]
        [SerializeField] private ShapeType type;
        [SerializeField] private Vector2 size = Vector2.one;
        [SerializeField] private float spawnChance = 1f;
        [SerializeField] private bool enabled = true;

        [Header("视觉设置")]
        [SerializeField] private Sprite sprite;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private ParticleSystem hitEffectPrefab;
        [SerializeField] private ParticleSystem destroyEffectPrefab;

        public ShapeTypeConfig ToShapeTypeConfig()
        {
            return new ShapeTypeConfig
            {
                Type = type,
                Size = new Vector2D(size.x, size.y),
                SpawnChance = spawnChance,
                Enabled = enabled
            };
        }

        public Sprite GetSprite() => sprite;
        public Color GetColor() => color;
        public ParticleSystem GetHitEffect() => hitEffectPrefab;
        public ParticleSystem GetDestroyEffect() => destroyEffectPrefab;
    }
} 