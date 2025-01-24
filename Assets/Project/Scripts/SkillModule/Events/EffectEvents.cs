using SkillModule.Types;

namespace SkillModule.Events
{
    public class AddEffectEvent
    {
        public EffectData Effect { get; set; }
        public object Target { get; set; }
        public float Duration { get; set; }
        public string Source { get; set; }
    }

    public class RemoveEffectEvent
    {
        public int EffectId { get; set; }
        public string Reason { get; set; }
        public bool WasExpired { get; set; }
    }

    public class EffectStateChangeEvent
    {
        public int EffectId { get; set; }
        public bool IsActive { get; set; }
        public string Reason { get; set; }
        public float RemainingDuration { get; set; }
    }

    public class EffectUpdateEvent
    {
        public int EffectId { get; set; }
        public float DeltaTime { get; set; }
        public object UpdateData { get; set; }
    }
} 