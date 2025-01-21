using UnityEngine;

namespace MapModule.Shapes
{
    public struct ShapeHitEvent
    {
        public ShapeType Type;
        public Vector3 HitPoint;
        public Vector3 Position;
        public int SkillId;
    }

    public struct BulletCollectedEvent
    {
        public Vector3 Position;
        public int CurrentCount;
        public int MaxCount;
    }

    public struct ShapeActionEvent
    {
        public ShapeType Type;
        public ShapeActionType ActionType;
        public Vector3 Position;
        public object ActionData;
    }

    public enum ShapeActionType
    {
        None,
        Shoot,
        Rotate,
        Explode,
        GridDestroy
    }

    public struct GridCellDisableEvent
    {
        public Vector3 Position;
        public Vector2 Size;
    }

    public struct ShapeStateChangedEvent
    {
        public ShapeState OldState;
        public ShapeState NewState;
    }

    public struct MapSystemInitializedEvent
    {
        public MapConfig Config;        // 地图配置
        public Vector3 CentralPosition; // 中心位置
        public Vector2 AreaSize;        // 区域大小
    }
} 