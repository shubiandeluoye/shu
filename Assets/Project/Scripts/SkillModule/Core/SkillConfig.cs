using UnityEngine;
using SkillModule.Types;

namespace SkillModule.Core
{
    /// <summary>
    /// 技能配置基类
    /// 负责：
    /// 1. 基础技能属性配置
    /// 2. 技能预制体管理
    /// 3. 技能类型定义
    /// </summary>
    public abstract class SkillConfig : ScriptableObject
    {
        [Header("基础配置")]
        public int SkillId;
        public string SkillName;
        [TextArea(3, 5)]
        public string Description;
        public SkillType Type;

        [Header("时间参数")]
        public float Cooldown = 1f;
        public float Duration;
        public float CastTime;
        public bool CanCancel;

        [Header("目标配置")]
        public TargetType TargetType;
        public LayerMask TargetMask;
        public float Range;

        [Header("资源配置")]
        public GameObject SkillPrefab;
        public GameObject EffectPrefab;
        public Sprite SkillIcon;
        public AudioClip SkillSound;
        [Range(0, 1)]
        public float SoundVolume = 1f;

        protected virtual void OnValidate()
        {
            // 确保技能ID不为0
            if (SkillId == 0)
            {
                Debug.LogWarning($"[SkillConfig] {SkillName} 的技能ID为0，请设置一个有效值");
            }

            // 确保冷却时间大于0
            if (Cooldown <= 0)
            {
                Cooldown = 0.1f;
                Debug.LogWarning($"[SkillConfig] {SkillName} 的冷却时间必须大于0");
            }

            // 确保持续时间不为负数
            if (Duration < 0)
            {
                Duration = 0;
                Debug.LogWarning($"[SkillConfig] {SkillName} 的持续时间不能为负数");
            }

            // 确保施法时间不为负数
            if (CastTime < 0)
            {
                CastTime = 0;
                Debug.LogWarning($"[SkillConfig] {SkillName} 的施法时间不能为负数");
            }

            // 确保范围不为负数
            if (Range < 0)
            {
                Range = 0;
                Debug.LogWarning($"[SkillConfig] {SkillName} 的范围不能为负数");
            }

            ValidateSkillType();
        }

        protected virtual void ValidateSkillType()
        {
            // 子类重写以验证具体技能类型
        }

        public virtual bool IsValid()
        {
            return SkillId != 0 && !string.IsNullOrEmpty(SkillName);
        }
    }
} 