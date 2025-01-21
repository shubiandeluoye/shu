using UnityEngine;

namespace CombatModule
{
    /// <summary>
    /// 伤害计算器
    /// 负责：
    /// 1. 基础伤害计算
    /// 2. 暴击判定
    /// 3. 伤害修正
    /// </summary>
    public class DamageCalculator
    {
        private readonly DamageConfig config;

        public DamageCalculator(DamageConfig config)
        {
            this.config = config;
        }

        public int CalculateDamage(DamageData damageData)
        {
            float finalDamage = damageData.BaseDamage;

            // 应用暴击
            if (ShouldCrit(damageData.CritChance))
            {
                finalDamage *= config.CritDamageMultiplier;
            }

            // 应用伤害类型修正
            finalDamage *= GetDamageTypeModifier(damageData.DamageType);

            // 应用随机波动
            finalDamage *= Random.Range(1f - config.DamageVariance, 1f + config.DamageVariance);

            return Mathf.RoundToInt(finalDamage);
        }

        private bool ShouldCrit(float critChance)
        {
            return Random.value < critChance;
        }

        private float GetDamageTypeModifier(DamageType type)
        {
            switch (type)
            {
                case DamageType.Normal:
                    return 1.0f;
                case DamageType.Skill:
                    return config.SkillDamageMultiplier;
                case DamageType.Status:
                    return config.StatusDamageMultiplier;
                default:
                    return 1.0f;
            }
        }
    }
} 