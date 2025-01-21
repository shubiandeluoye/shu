using SkillModule.Effects;

namespace SkillModule.Events
{
    public struct AddEffectEvent
    {
        public BaseEffect Effect;
    }

    public struct RemoveEffectEvent
    {
        public int EffectId;
    }

    public struct EffectStateChangeEvent
    {
        public int EffectId;
        public bool IsActive;
        public string Reason;
    }
} 