using UnityEngine;

namespace CombatModule
{
    public class DamageData
    {
        public float BaseDamage;
        public DamageType DamageType;
        public float CritChance;
        public bool HasKnockback;
        public Vector2 KnockbackDirection;
    }

    public enum DamageType
    {
        Normal,     // 普通伤害
        Skill,      // 技能伤害
        Status      // 状态伤害
    }

    public class StatusEffectData
    {
        public StatusEffectType Type;
        public float Value;        // 基础值
        public float SlowAmount;   // 减速效果的减速量
        public float Duration;
        public int MaxStacks;
        public bool HasTickEffect;
        public float TickInterval;
    }

    public enum StatusEffectType
    {
        Stun,       // 眩晕
        Slow,       // 减速
        Dot         // 持续伤害
    }

    public class ActiveStatusEffect
    {
        public StatusEffectData Data;
        public float RemainingTime;
        public int StackCount;
        public float TimeSinceLastTick;
    }
} 