using UnityEngine;
using SkillModule.Types;
using SkillModule.Core;

namespace SkillModule.Skills
{
    /// <summary>
    /// 屏障技能配置
    /// 负责：
    /// 1. 屏障属性配置
    /// 2. 屏障形状定义
    /// 3. 防御参数设置
    /// 4. 视觉效果配置
    /// </summary>
    [CreateAssetMenu(fileName = "BarrierConfig", menuName = "Skills/Barrier Skill Config")]
    public class BarrierConfig : SkillConfig
    {
        [Header("屏障配置")]
        public float BarrierWidth = 3f;
        public float BarrierHeight = 2f;
        public float BarrierHealth = 100f;
        public float DamageReduction = 0.5f;
        public Color BarrierColor = new Color(0.3f, 0.8f, 1f, 0.6f);
        public Color DamagedColor = new Color(1f, 0.3f, 0.3f, 0.6f);

        [Header("交互配置")]
        public bool BlocksProjectiles = true;
        public bool BlocksPlayers = true;
        public LayerMask BlockedLayers;
        public float KnockbackForce = 5f;

        [Header("视觉效果")]
        public bool PulseEffect = true;
        public float PulseSpeed = 2f;
        public float PulseIntensity = 0.1f;
        public GameObject ImpactEffectPrefab;
        public float ImpactEffectDuration = 0.3f;

        [Header("音效配置")]
        public AudioClip CreateSound;
        public AudioClip BlockSound;
        public AudioClip BreakSound;
        [Range(0f, 1f)]
        public new float SoundVolume = 0.5f;

        public BarrierConfig()
        {
            Type = SkillType.Barrier;
        }

        protected override void ValidateSkillType()
        {
            if (Type != SkillType.Barrier)
            {
                Debug.LogWarning($"[BarrierConfig] {SkillName} 的技能类型必须是 Barrier");
                Type = SkillType.Barrier;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            // 验证尺寸
            if (BarrierWidth <= 0)
            {
                BarrierWidth = 1f;
                Debug.LogWarning($"[BarrierConfig] {SkillName} 的屏障宽度必须大于0");
            }

            if (BarrierHeight <= 0)
            {
                BarrierHeight = 1f;
                Debug.LogWarning($"[BarrierConfig] {SkillName} 的屏障高度必须大于0");
            }

            // 验证生命值
            if (BarrierHealth <= 0)
            {
                BarrierHealth = 1f;
                Debug.LogWarning($"[BarrierConfig] {SkillName} 的屏障生命值必须大于0");
            }

            // 验证伤害减免
            DamageReduction = Mathf.Clamp01(DamageReduction);

            // 验证击退力
            if (KnockbackForce < 0)
            {
                KnockbackForce = 0;
                Debug.LogWarning($"[BarrierConfig] {SkillName} 的击退力不能为负数");
            }

            // 验证脉冲效果
            if (PulseSpeed <= 0)
            {
                PulseSpeed = 1f;
                Debug.LogWarning($"[BarrierConfig] {SkillName} 的脉冲速度必须大于0");
            }
        }
    }
} 