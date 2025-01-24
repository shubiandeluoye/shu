using SkillModule.Core;
using SkillModule.Events;

namespace SkillModule.Skills
{
    /// <summary>
    /// 回血技能
    /// 功能：
    /// 1. 恢复指定数值的生命值
    /// 2. 播放回血特效
    /// 3. 有冷却时间
    /// </summary>
    public class HealSkill : BaseSkill
    {
        private HealConfig healConfig;

        public HealSkill(HealConfig config) : base(config)
        {
            healConfig = config;
        }

        protected override void OnSkillStart()
        {
            // 发布治疗事件
            eventSystem.Publish("HealApplied", new HealSkillData
            {
                SkillId = SkillId,
                Position = skillContext.Position,
                HealAmount = healConfig.HealAmount,
                HealRadius = healConfig.HealRadius,
                IsOverTime = healConfig.IsOverTime,
                Duration = healConfig.Duration,
                BuffStrength = healConfig.BuffStrength,
                Source = skillContext.Source,
                Target = skillContext.Target
            });
        }
    }

    public class HealConfig : SkillConfig
    {
        public float HealAmount { get; set; }
        public float HealRadius { get; set; }
        public bool IsOverTime { get; set; }
        public float BuffStrength { get; set; }
    }

    public struct HealSkillData
    {
        public int SkillId { get; set; }
        public Vector3 Position { get; set; }
        public float HealAmount { get; set; }
        public float HealRadius { get; set; }
        public bool IsOverTime { get; set; }
        public float Duration { get; set; }
        public float BuffStrength { get; set; }
        public object Source { get; set; }
        public object Target { get; set; }
    }
} 