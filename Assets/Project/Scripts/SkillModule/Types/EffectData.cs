namespace SkillModule.Types
{
    public struct EffectData
    {
        public int EffectId { get; set; }
        public EffectType Type { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float Duration { get; set; }
        public float[] Parameters { get; set; }
    }

    // 不同类型效果的具体数据结构
    public struct ProjectileEffectData
    {
        public int EffectId { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public Vector3 Direction { get; set; }
    }

    public struct AreaEffectData
    {
        public int EffectId { get; set; }
        public float Radius { get; set; }
        public float Duration { get; set; }
        public Vector3 Position { get; set; }
    }

    public struct BarrierEffectData
    {
        public int EffectId { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Health { get; set; }
        public Vector3 Position { get; set; }
    }
} 