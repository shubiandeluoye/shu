using UnityEngine;

namespace SkillModule.Types
{
    /// <summary>
    /// 技能状态枚举
    /// </summary>
    public enum SkillState
    {
        Ready,      // 准备就绪
        Active,     // 技能激活中
        Cooldown,   // 冷却中
        Disabled    // 禁用状态
    }

    /// <summary>
    /// 技能触发数据
    /// </summary>
    public struct SkillTriggerData
    {
        public int SkillId;                // 技能ID
        public TriggerType TriggerType;    // 触发类型
        public GameObject Source;          // 来源对象
        public GameObject Target;          // 目标对象
        public float[] Parameters;         // 参数数组（伤害值、治疗量等）
        public Vector3 Position;           // 触发位置
        public Vector3 Direction;          // 触发方向
    }

    /// <summary>
    /// 触发类型枚举
    /// </summary>
    public enum TriggerType
    {
        None = 0,
        OnDamaged,        // 受伤时
        OnHealed,         // 治疗时
        OnKill,          // 击杀时
        OnDeath,         // 死亡时
        OnStateChanged,   // 状态改变时
        OnInterval,       // 定时触发
        OnHealthThreshold,// 生命值阈值
        OnBuffApplied,    // Buff应用时
        OnBuffRemoved,    // Buff移除时
        OnCollision,      // 碰撞时
        Custom = 1000     // 自定义触发器起始值
    }

    /// <summary>
    /// 技能效果类型
    /// </summary>
    public enum SkillEffectType
    {
        None = 0,
        Damage,          // 伤害
        Heal,           // 治疗
        Buff,           // 增益
        Debuff,         // 减益
        Spawn,          // 生成物体
        Area,           // 区域效果
        Movement,       // 移动效果
        Custom = 1000    // 自定义效果起始值
    }

    /// <summary>
    /// 基础技能数据
    /// </summary>
    [System.Serializable]
    public class SkillData
    {
        public int SkillId;
        public string SkillName;
        public string Description;
        public SkillType Type;
        public float Cooldown;
        public float Duration;
        public bool CanCancel;
        public GameObject SkillPrefab;
    }

    /// <summary>
    /// 治疗技能数据
    /// </summary>
    [System.Serializable]
    public class HealData : SkillData
    {
        public int HealAmount = 20;
        public new float Duration = 0.5f;
        public Color HealColor = Color.green;
        public float EffectScale = 1f;

        public HealData()
        {
            Type = SkillType.Heal;
        }
    }
} 