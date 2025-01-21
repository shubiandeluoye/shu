using UnityEngine;

namespace MapModule.Shapes
{
    public enum ShapeType
    {
        None = 0,    // 添加 None 作为默认值
        Circle,
        Rectangle,
        Triangle,
        Trapezoid
    }

    [System.Serializable]
    public class ShapeConfig
    {
        public ShapeType Type;          // 形状类型
        public Vector2 Size;            // 基础大小
        public float Duration;          // 持续时间（如果有）
        
        // 圆形特有
        public int BulletCapacity;      // 子弹容量
        
        // 矩形特有
        public Vector2Int GridSize;     // 网格大小
        
        // 三角形特有
        public float RotationSpeed;     // 旋转速度
        
        // 梯形特有
        public float TopWidth;          // 上底宽度
        public float BottomWidth;       // 下底宽度
        public float BulletDelay;       // 子弹延迟
        
        // 新增：通用配置
        public bool EnableRotation = true;
        public bool EnableShoot = true;
    }
} 