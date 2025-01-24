using UnityEngine;
using MapModule.Core.Data;
using MapModule.Core.Utils;

namespace MapModule.Unity.Configs
{
    [CreateAssetMenu(fileName = "ShapeConfig", menuName = "Map/Shape Config")]
    public class ShapeConfigSO : ScriptableObject
    {
        [Header("基础设置")]
        [SerializeField] private ShapeType type;
        [SerializeField] private float duration = 30f;
        [SerializeField] private bool enableRotation = true;
        [SerializeField] private bool enableShoot = true;

        [Header("圆形设置")]
        [SerializeField] private int bulletCapacity = 21;

        [Header("矩形设置")]
        [SerializeField] private Vector2Int gridSize = new(5, 8);

        [Header("三角形设置")]
        [SerializeField] private float rotationSpeed = 90f;

        [Header("梯形设置")]
        [SerializeField] private float topWidth = 1f;
        [SerializeField] private float bottomWidth = 2f;
        [SerializeField] private float bulletDelay = 0.35f;

        public ShapeConfig ToShapeConfig()
        {
            return new ShapeConfig
            {
                Type = type,
                Duration = duration,
                EnableRotation = enableRotation,
                EnableShoot = enableShoot,
                BulletCapacity = bulletCapacity,
                GridSize = new Vector2D(gridSize.x, gridSize.y),
                RotationSpeed = rotationSpeed,
                TopWidth = topWidth,
                BottomWidth = bottomWidth,
                BulletDelay = bulletDelay
            };
        }
    }
} 