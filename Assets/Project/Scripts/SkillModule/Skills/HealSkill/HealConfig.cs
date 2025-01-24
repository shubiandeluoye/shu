using UnityEngine;
using SkillModule.Types;
using SkillModule.Core;

namespace SkillModule.Skills
{
    [CreateAssetMenu(fileName = "HealConfig", menuName = "Skills/Heal Skill Config")]
    public class HealConfig : SkillConfig
    {
        [Header("治疗配置")]
        public int HealAmount { get; set; } = 20;
        public bool IsPercentage { get; set; } = false;
        public float HealRadius { get; set; } = 0f;        // 0表示单体治疗，大于0表示范围治疗
        public LayerMask TargetLayers;
        public bool CanOverheal { get; set; } = false;     // 是否可以过量治疗

        [Header("治疗效果")]
        public Color HealColor = Color.green;
        public float EffectScale = 1f;
        public bool UseParticles = true;
        public int ParticleCount = 20;
        public float ParticleSpeed = 5f;
        public float ParticleLifetime = 1f;

        [Header("音效配置")]
        public AudioClip HealSound;
        public AudioClip FailSound;
        [Range(0f, 1f)]
        public new float SoundVolume = 0.5f;

        [Header("额外效果")]
        public bool ApplyBuff { get; set; } = false;
        public float BuffDuration { get; set; } = 5f;
        public float BuffStrength { get; set; } = 1.2f;
        public GameObject BuffEffectPrefab;

        public HealConfig()
        {
            Type = SkillType.Heal;
        }

        protected override void ValidateSkillType()
        {
            if (Type != SkillType.Heal)
            {
                Debug.LogWarning($"[HealConfig] {SkillName} 的技能类型必须是 Heal");
                Type = SkillType.Heal;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            // 验证治疗量
            if (HealAmount < 0)
            {
                HealAmount = 0;
                Debug.LogWarning($"[HealConfig] {SkillName} 的治疗量不能为负数");
            }

            // 验证治疗半径
            if (HealRadius < 0)
            {
                HealRadius = 0;
                Debug.LogWarning($"[HealConfig] {SkillName} 的治疗半径不能为负数");
            }

            // 验证粒子效果
            if (ParticleCount < 0)
            {
                ParticleCount = 0;
                Debug.LogWarning($"[HealConfig] {SkillName} 的粒子数量不能为负数");
            }

            if (ParticleSpeed <= 0)
            {
                ParticleSpeed = 1f;
                Debug.LogWarning($"[HealConfig] {SkillName} 的粒子速度必须大于0");
            }

            if (ParticleLifetime <= 0)
            {
                ParticleLifetime = 0.1f;
                Debug.LogWarning($"[HealConfig] {SkillName} 的粒子生命周期必须大于0");
            }

            // 验证Buff效果
            if (ApplyBuff)
            {
                if (BuffDuration <= 0)
                {
                    BuffDuration = 1f;
                    Debug.LogWarning($"[HealConfig] {SkillName} 的Buff持续时间必须大于0");
                }

                if (BuffStrength <= 0)
                {
                    BuffStrength = 1f;
                    Debug.LogWarning($"[HealConfig] {SkillName} 的Buff强度必须大于0");
                }
            }
        }

        public override bool IsValid()
        {
            if (!base.IsValid()) return false;

            // 验证治疗配置
            if (HealAmount < 0) return false;
            if (HealRadius < 0) return false;

            // 验证Buff效果
            if (ApplyBuff)
            {
                if (BuffDuration <= 0) return false;
                if (BuffStrength <= 0) return false;
            }

            return true;
        }
    }
} 