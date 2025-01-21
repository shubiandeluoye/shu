using UnityEngine;

namespace CombatModule
{
    public struct BulletHitEvent
    {
        public GameObject Bullet;
        public GameObject Target;
        public DamageData DamageData;
    }

    public struct SkillHitEvent
    {
        public GameObject Attacker;
        public GameObject Target;
        public DamageData DamageData;
        public StatusEffectData StatusEffect;
    }

    public struct StatusEffectEvent
    {
        public StatusEffectEventType Type;
        public GameObject Target;
        public StatusEffectData EffectData;
    }

    public enum StatusEffectEventType
    {
        Add,
        Remove,
        Stack
    }

    public struct StatusEffectStackEvent
    {
        public GameObject Target;
        public StatusEffectType EffectType;
        public int NewStackCount;
    }

    public struct StatusEffectRemoveEvent
    {
        public GameObject Target;
        public StatusEffectType EffectType;
    }

    public struct DotTickEvent
    {
        public GameObject Target;
        public DamageData DamageData;
    }
} 