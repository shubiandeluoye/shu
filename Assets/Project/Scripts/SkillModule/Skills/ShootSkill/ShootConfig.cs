using UnityEngine;
using SkillModule.Types;
using SkillModule.Core;

namespace SkillModule.Skills
{
    /// <summary>
    /// 射击技能配置
    /// 负责：
    /// 1. 射击参数配置
    /// 2. 子弹属性设置
    /// 3. 射击模式定义
    /// 4. 特效配置
    /// </summary>
    [CreateAssetMenu(fileName = "New Shoot Skill", menuName = "Skills/Shoot Skill")]
    public class ShootConfig : SkillConfig
    {
        [Header("子弹预制体")]
        public GameObject BulletPrefab;    // 只需要一个子弹预制体
        
        [Header("射击配置")]
        public float BulletSpeed { get; set; } = 12f;
        public float BulletDamage { get; set; } = 10f;
        
        [Header("射击角度")]
        public float[] ShootAngles { get; set; } = new float[] { 0 };

        [Header("子弹属性")]
        public float BulletLifetime { get; set; } = 3f;
        public float BulletRadius { get; set; } = 0.2f;
        public Color BulletColor = Color.red;
        public Color ShootEffectColor = Color.yellow;

        [Header("特效配置")]
        public GameObject ShootEffectPrefab;             // 发射特效
        public float ShootEffectDuration = 0.5f;         // 发射特效持续时间
        public AudioClip ShootSound;                     // 发射音效
        public AudioClip BulletImpactSound;              // 子弹击中音效

        [Header("射击模式")]
        public ShootPattern Pattern { get; set; } = ShootPattern.Straight;

        public ShootConfig()
        {
            Type = SkillType.Shoot;
        }

        public override bool IsValid()
        {
            if (!base.IsValid()) return false;

            // 验证子弹配置
            if (BulletSpeed <= 0) return false;
            if (BulletDamage < 0) return false;
            if (BulletLifetime <= 0) return false;
            if (BulletRadius <= 0) return false;

            return true;
        }

        protected override void ValidateSkillType()
        {
            if (Type != SkillType.Shoot)
            {
                Debug.LogError($"[ShootConfig] {SkillName} 的技能类型错误: {Type}，已自动修正为 Shoot");
                Type = SkillType.Shoot;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            Debug.Log($"[ShootConfig] 正在验证技能配置: {SkillName}");

            if (BulletPrefab == null)
            {
                Debug.LogError($"[ShootConfig] {SkillName} 缺少子弹预制体!");
            }

            // 验证子弹配置
            if (BulletSpeed <= 0)
            {
                BulletSpeed = 1f;
                Debug.LogWarning($"[ShootConfig] {SkillName} 的子弹速度必须大于0");
            }

            if (BulletDamage < 0)
            {
                BulletDamage = 0;
                Debug.LogWarning($"[ShootConfig] {SkillName} 的子弹伤害不能为负数");
            }

            if (BulletLifetime <= 0)
            {
                BulletLifetime = 1f;
                Debug.LogWarning($"[ShootConfig] {SkillName} 的子弹生命周期必须大于0");
            }

            if (BulletRadius <= 0)
            {
                BulletRadius = 0.1f;
                Debug.LogWarning($"[ShootConfig] {SkillName} 的子弹半径必须大于0");
            }

            Debug.Log($"[ShootConfig] {SkillName} 配置验证完成");
        }
    }

    public enum BulletType
    {
        Normal,
        Special
    }

    public enum ShootPattern
    {
        Straight,
        Spread30,
        Spread45
    }
} 