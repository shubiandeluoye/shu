using MapModule.Core.Utils;

namespace MapModule.Core.Data
{
    public class ShapeState
    {
        public ShapeType Type { get; set; }
        public Vector3D Position { get; set; }
        public bool IsActive { get; set; }
        
        // 圆形特有
        public int CurrentBulletCount { get; set; }
        
        // 矩形特有
        public bool[,] GridState { get; set; }
        
        // 三角形和梯形共有
        public float CurrentRotation { get; set; }
        public bool IsRotating { get; set; }
        
        // 梯形特有
        public float LastBulletTime { get; set; }
        
        // 矩形和梯形共有
        public float RemainingTime { get; set; }
    }
} 