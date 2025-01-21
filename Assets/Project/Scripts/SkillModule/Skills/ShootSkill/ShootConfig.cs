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
        [Header("子弹配置")]
        public float BulletSpeed = 10f;
        public float BulletDamage = 10f;
        public float BulletLifetime = 3f;
        public float BulletRadius = 0.2f;
        public Color BulletColor = Color.red;

        [Header("发射配置")]
        public float[] ShootAngles = new float[] { 0 };  // 发射角度数组
        public int BulletsPerShot = 1;                   // 每次发射的子弹数量
        public float SpreadAngle = 0f;                   // 散射角度
        public bool UseRandomSpread = false;             // 是否使用随机散射

        [Header("特效配置")]
        public GameObject ShootEffectPrefab;             // 发射特效
        public Color ShootEffectColor = Color.yellow;
        public float ShootEffectDuration = 0.2f;

        [Header("音效配置")]
        public AudioClip ShootSound;          // 添加射击音效
        public AudioClip BulletImpactSound;   // 添加子弹碰撞音效

        public ShootConfig()
        {
            Type = SkillType.Shoot;  // 现在可以使用 SkillType
        }

        protected override void ValidateSkillType()
        {
            if (Type != SkillType.Shoot)
            {
                Debug.LogWarning($"[ShootConfig] {SkillName} 的技能类型必须是 Shoot");
                Type = SkillType.Shoot;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

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

            // 验证发射配置
            if (BulletsPerShot <= 0)
            {
                BulletsPerShot = 1;
                Debug.LogWarning($"[ShootConfig] {SkillName} 的每次发射子弹数量必须大于0");
            }

            if (SpreadAngle < 0)
            {
                SpreadAngle = 0;
                Debug.LogWarning($"[ShootConfig] {SkillName} 的散射角度不能为负数");
            }
        }
    }
} 