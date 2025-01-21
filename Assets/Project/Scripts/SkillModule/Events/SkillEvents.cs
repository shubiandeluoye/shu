using UnityEngine;
using SkillModule.Core;
using SkillModule.Types;

namespace SkillModule.Events
{
    /// <summary>
    /// 技能开始事件
    /// </summary>
    public struct SkillStartEvent
    {
        public int SkillId;
        public Vector3 Position;
        public Vector3 Direction;
    }

    /// <summary>
    /// 技能结束事件
    /// </summary>
    public struct SkillEndEvent
    {
        public int SkillId;
        public bool WasSuccessful;
        public string FailReason;
    }

    /// <summary>
    /// 技能冷却事件
    /// </summary>
    public struct SkillCooldownEvent
    {
        public int SkillId;
        public float CooldownTime;
        
    }

    /// <summary>
    /// 技能打断事件
    /// </summary>
    public struct SkillInterruptEvent
    {
        public int SkillId;
        public string Reason;
        public GameObject Source;
    }

    /// <summary>
    /// 技能状态变更事件
    /// </summary>
    public struct SkillStateChangeEvent
    {
        public int SkillId;
        public SkillState State;
        public string Reason;
    }

    /// <summary>
    /// 技能效果事件
    /// </summary>
    public struct SkillEffectEvent
    {
        public int SkillId;
        public int EffectId;
        public SkillEffectType Type;
        public GameObject Target;
        public float[] Parameters;
    }

    public struct DamageEvent
    {
        public GameObject Source;
        public GameObject Target;
        public float Damage;
        public Vector3 Direction;
        public Vector3 Position;
    }

    public struct HealEvent
    {
        public GameObject Source;
        public GameObject Target;
        public float Amount;
        public bool IsPercentage;
        public bool CanOverheal;
    }

    public struct EffectEvent
    {
        public GameObject Target;
        public int EffectId;
        public float Duration;
        public float Strength;
    }

    /// <summary>
    /// 子弹数据
    /// </summary>
    public struct BulletData
    {
        public int SkillId;           // 关联的技能ID
        public float Damage;          // 伤害值
        public float Speed;           // 速度
        public Vector3 Direction;     // 方向
        public GameObject Source;     // 发射源
    }

    /// <summary>
    /// 子弹类型切换事件
    /// </summary>
    public struct BulletTypeSwitchEvent
    {
        public int SkillId;           // 新的技能ID
        public BulletData BulletData; // 子弹数据
    }
} 