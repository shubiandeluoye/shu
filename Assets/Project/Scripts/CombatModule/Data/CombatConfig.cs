using UnityEngine;

namespace CombatModule
{
    [System.Serializable]
    public class CombatConfig
    {
        public DamageConfig DamageConfig;
        public StatusEffectConfig StatusEffectConfig;
    }

    [System.Serializable]
    public class DamageConfig
    {
        public float CritDamageMultiplier = 2f;
        public float SkillDamageMultiplier = 1.5f;
        public float StatusDamageMultiplier = 0.8f;
        public float DamageVariance = 0.1f;  // 伤害浮动范围 ±10%
    }

    [System.Serializable]
    public class StatusEffectConfig
    {
        public float DefaultDuration = 5f;
        public float DefaultTickInterval = 1f;
        public int DefaultMaxStacks = 3;
    }
} 