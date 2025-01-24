using UnityEngine;
using SkillModule.Types;
using SkillModule.Core;

namespace SkillModule.Skills
{
    /// <summary>
    /// 盒子技能配置
    /// 负责：
    /// 1. 盒子属性配置
    /// 2. 放置规则设置
    /// 3. 盒子外观定义
    /// 4. 生命周期管理
    /// </summary>
    [CreateAssetMenu(fileName = "BoxConfig", menuName = "Skills/Box Skill Config")]
    public class BoxConfig : SkillConfig
    {
        [Header("盒子配置")]
        public float BoxSize { get; set; } = 1f;
        public float BoxDurability { get; set; } = 100f;
        public Color BoxColor = Color.white;
        public PhysicsMaterial2D BoxMaterial;
        public bool UsePhysicsMaterial = true;
        public float Mass { get; set; } = 1f;
        public float Drag { get; set; } = 0.5f;

        [Header("放置配置")]
        public float PlaceDistance { get; set; } = 2f;
        public bool CheckGroundCollision { get; set; } = true;
        public LayerMask PlacementMask;
        public bool CanBePushed { get; set; } = true;
        public float PushForce { get; set; } = 5f;

        [Header("特效配置")]
        public GameObject PlaceEffectPrefab;
        public Color PlaceEffectColor = Color.white;
        public float PlaceEffectDuration = 0.5f;

        [Header("音效配置")]
        public AudioClip CreateSound;
        public AudioClip ImpactSound;
        
        public GameObject CreateEffectPrefab;
        public float CreateEffectDuration = 0.5f;

        public BoxConfig()
        {
            Type = SkillType.Box;
        }

        protected override void ValidateSkillType()
        {
            if (Type != SkillType.Box)
            {
                Debug.LogWarning($"[BoxConfig] {SkillName} 的技能类型必须是 Box");
                Type = SkillType.Box;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            // 验证盒子配置
            if (BoxSize <= 0)
            {
                BoxSize = 1f;
                Debug.LogWarning($"[BoxConfig] {SkillName} 的盒子大小必须大于0");
            }

            if (BoxDurability <= 0)
            {
                BoxDurability = 1f;
                Debug.LogWarning($"[BoxConfig] {SkillName} 的盒子耐久度必须大于0");
            }

            if (PlaceDistance < 0)
            {
                PlaceDistance = 0;
                Debug.LogWarning($"[BoxConfig] {SkillName} 的放置距离不能为负数");
            }

            if (PushForce < 0)
            {
                PushForce = 0;
                Debug.LogWarning($"[BoxConfig] {SkillName} 的推力不能为负数");
            }
        }

        public override bool IsValid()
        {
            if (!base.IsValid()) return false;

            // 验证盒子配置
            if (BoxSize <= 0) return false;
            if (BoxDurability <= 0) return false;
            if (PlaceDistance < 0) return false;
            if (PushForce < 0) return false;

            return true;
        }
    }
} 