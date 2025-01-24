namespace MapModule.Core.Data
{
    public enum ShapeType
    {
        None = 0,
        Circle,
        Rectangle,
        Triangle,
        Trapezoid
    }

    public class ShapeConfig
    {
        public ShapeType Type { get; set; }
        public Vector2D Size { get; set; }
        public float Duration { get; set; }
        
        // 圆形特有
        public int BulletCapacity { get; set; }
        
        // 矩形特有
        public Vector2D GridSize { get; set; }
        
        // 三角形特有
        public float RotationSpeed { get; set; }
        
        // 梯形特有
        public float TopWidth { get; set; }
        public float BottomWidth { get; set; }
        public float BulletDelay { get; set; }
        
        public bool EnableRotation { get; set; } = true;
        public bool EnableShoot { get; set; } = true;
    }
} 