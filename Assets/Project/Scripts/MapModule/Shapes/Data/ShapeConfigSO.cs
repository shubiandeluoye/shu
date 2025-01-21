using UnityEngine;

namespace MapModule.Shapes
{
    [CreateAssetMenu(fileName = "ShapeConfig", menuName = "Map/ShapeConfig")]
    public class ShapeConfigSO : ScriptableObject
    {
        [Header("基础配置")]
        public ShapeType Type;
        public Vector2 Size = Vector2.one;
        public float Duration = 30f;

        [Header("圆形配置")]
        public int BulletCapacity = 21;

        [Header("矩形配置")]
        public Vector2Int GridSize = new Vector2Int(5, 8);

        [Header("三角形配置")]
        public float RotationSpeed = 90f;

        [Header("梯形配置")]
        public float TopWidth = 1.8f;
        public float BottomWidth = 2.5f;
        public float BulletDelay = 0.35f;

        private void OnValidate()
        {
            // 验证配置
            if (BulletCapacity <= 0) BulletCapacity = 1;
            if (RotationSpeed <= 0) RotationSpeed = 1;
            if (BulletDelay <= 0) BulletDelay = 0.1f;
            if (TopWidth <= 0) TopWidth = 0.1f;
            if (BottomWidth <= 0) BottomWidth = 0.1f;
        }

        public ShapeConfig ToRuntimeConfig()
        {
            return new ShapeConfig
            {
                Type = Type,
                Size = Size,
                Duration = Duration,
                BulletCapacity = BulletCapacity,
                GridSize = GridSize,
                RotationSpeed = RotationSpeed,
                TopWidth = TopWidth,
                BottomWidth = BottomWidth,
                BulletDelay = BulletDelay,
                EnableRotation = Type == ShapeType.Triangle || Type == ShapeType.Trapezoid,
                EnableShoot = Type == ShapeType.Trapezoid
            };
        }
    }
} 