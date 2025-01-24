using System;
using SkillModule.Core;
using SkillModule.Events;

namespace SkillModule.Skills
{
    public class ShootSkill : BaseSkill
    {
        private ShootConfig shootConfig;

        public ShootSkill(ShootConfig config) : base(config)
        {
            shootConfig = config;
        }

        protected override void OnSkillStart()
        {
            SpawnProjectile();
        }

        private void SpawnProjectile()
        {
            // 发布发射投射物事件
            eventSystem.Publish("ProjectileSpawned", new ProjectileData
            {
                SkillId = SkillId,
                Damage = shootConfig.Damage,
                Speed = shootConfig.Speed,
                Direction = shootConfig.Direction
            });
        }
    }

    public class ShootConfig : SkillConfig
    {
        public float Damage { get; set; }
        public float Speed { get; set; }
        public Vector3 Direction { get; set; }
    }

    public struct ProjectileData
    {
        public int SkillId { get; set; }
        public float Damage { get; set; }
        public float Speed { get; set; }
        public Vector3 Direction { get; set; }
    }
} 