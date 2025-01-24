namespace MapModule.Core.Data
{
    public static class MapEvents
    {
        // 系统事件
        public const string SystemInitialized = "MapSystemInitialized";
        public const string AreaStateChanged = "AreaStateChanged";
        public const string ShapeChanged = "ShapeChanged";

        // 形状事件
        public const string ShapeHit = "ShapeHit";
        public const string ShapeDestroyed = "ShapeDestroyed";
        public const string BulletCollected = "BulletCollected";
        public const string GridCellDestroyed = "GridCellDestroyed";
    }

    public struct ShapeHitEvent
    {
        public ShapeType Type { get; set; }
        public Vector3D HitPoint { get; set; }
        public Vector3D Position { get; set; }
        public int SkillId { get; set; }
    }

    public struct BulletCollectedEvent
    {
        public Vector3D Position { get; set; }
        public int CurrentCount { get; set; }
        public int MaxCount { get; set; }
    }

    public struct AreaStateChangedEvent
    {
        public Vector2D CentralAreaSize { get; set; }
        public Vector2D ActiveAreaSize { get; set; }
        public bool IsActive { get; set; }
    }
} 