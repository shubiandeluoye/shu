using MapModule.Core.Utils;

namespace MapModule.Core.Data
{
    public enum ShapeActionType
    {
        None,
        Shoot,
        Rotate,
        Disappear
    }

    public struct ShapeChangedEvent
    {
        public ShapeType Type { get; set; }
        public Vector3D Position { get; set; }
        public float Rotation { get; set; }
        public float RotationDirection { get; set; }
        public ShapeActionType ActionType { get; set; }
        public object ActionData { get; set; }
    }

    public struct ShapeDestroyedEvent
    {
        public ShapeType Type { get; set; }
        public Vector3D Position { get; set; }
    }
} 