using SkillModule.Core;
using SkillModule.Events;

namespace SkillModule.Skills
{
    public class BarrierSkill : BaseSkill
    {
        private BarrierConfig barrierConfig;

        public BarrierSkill(BarrierConfig config) : base(config)
        {
            barrierConfig = config;
        }

        protected override void OnSkillStart()
        {
            // 发布创建屏障事件
            eventSystem.Publish("BarrierCreated", new BarrierSkillData
            {
                SkillId = SkillId,
                Position = skillContext.Position,
                Direction = skillContext.Direction,
                Width = barrierConfig.BarrierWidth,
                Height = barrierConfig.BarrierHeight,
                Health = barrierConfig.BarrierHealth,
                BlocksProjectiles = barrierConfig.BlocksProjectiles,
                BlocksPlayers = barrierConfig.BlocksPlayers,
                DamageReduction = barrierConfig.DamageReduction,
                Duration = barrierConfig.Duration
            });
        }
    }

    public class BarrierConfig : SkillConfig
    {
        public float BarrierWidth { get; set; }
        public float BarrierHeight { get; set; }
        public float BarrierHealth { get; set; }
        public bool BlocksProjectiles { get; set; }
        public bool BlocksPlayers { get; set; }
        public float DamageReduction { get; set; }
    }

    public struct BarrierSkillData
    {
        public int SkillId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Health { get; set; }
        public bool BlocksProjectiles { get; set; }
        public bool BlocksPlayers { get; set; }
        public float DamageReduction { get; set; }
        public float Duration { get; set; }
    }
} 