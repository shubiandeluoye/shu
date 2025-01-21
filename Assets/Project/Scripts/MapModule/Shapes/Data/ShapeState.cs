using UnityEngine;

namespace MapModule.Shapes
{
    public class ShapeState
    {
        public ShapeType Type { get; set; }
        public Vector3 Position { get; set; }
        public bool IsActive { get; set; }
        public int ShapeId { get; set; }
        public float Rotation { get; set; }
        public Vector3 Scale { get; set; }
        
        // 圆形状态
        public int CurrentBulletCount { get; set; }
        
        // 矩形状态
        public bool[,] GridState { get; set; }
        
        // 三角形状态
        public float CurrentRotation { get; set; }
        public bool IsRotating { get; set; }
        
        // 梯形状态
        public float LastBulletTime { get; set; }
        public float RemainingTime { get; set; }
    }
} 