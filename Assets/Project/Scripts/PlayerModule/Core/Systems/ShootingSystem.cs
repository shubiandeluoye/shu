using PlayerModule.Data;
using SkillModule.Core;
using SkillModule.Utils;
using System.Collections.Generic;

namespace PlayerModule.Core.Systems
{
    public class ShootingSystem : IPlayerSystem
    {
        private readonly ShootingConfig config;
        private IPlayerManager manager;
        private float lastShootTime;
        private BulletType currentBulletType;
        private float currentAngle;
        private Dictionary<int, SkillConfig> skillSlots = new Dictionary<int, SkillConfig>();
        private int currentSlot = 0;

        public BulletType CurrentBulletType => currentBulletType;

        public ShootingSystem(ShootingConfig config)
        {
            this.config = config;
        }

        public void Initialize(IPlayerManager manager)
        {
            this.manager = manager;
            currentBulletType = BulletType.Small;
            currentAngle = 0f;
        }

        public void Shoot(Vector3 position, Vector3 direction)
        {
            if (!CanShoot()) return;

            if (skillSlots.TryGetValue(currentSlot, out var skillConfig))
            {
                manager.PublishEvent("PlayerShoot", new PlayerShootEvent
                {
                    SkillId = skillConfig.SkillId,
                    Position = position,
                    Direction = direction,
                    Parameters = new float[] { GetBulletDamage(), GetBulletSpeed() }
                });

                lastShootTime = SkillUtils.GetCurrentTime();
            }
        }

        public void SwitchBulletType(BulletType newType)
        {
            if (currentBulletType == newType) return;
            currentBulletType = newType;
            
            manager.PublishEvent("BulletTypeChanged", new BulletTypeSwitchEvent
            {
                SkillId = (int)newType,
                BulletData = new BulletData
                {
                    Damage = GetBulletDamage(),
                    Speed = GetBulletSpeed()
                }
            });
        }

        public void SetSkillSlot(int slotIndex, SkillConfig config)
        {
            if (config == null) return;
            skillSlots[slotIndex] = config;
        }

        public void SwitchSkillSlot(int slotIndex)
        {
            if (skillSlots.ContainsKey(slotIndex))
            {
                currentSlot = slotIndex;
            }
        }

        private bool CanShoot()
        {
            return SkillUtils.GetCurrentTime() >= lastShootTime + config.ShootCooldown;
        }

        private float GetBulletSpeed()
        {
            return currentBulletType switch
            {
                BulletType.Small => 10f,
                BulletType.Medium => 8f,
                BulletType.Large => 5f,
                _ => 10f
            };
        }

        private int GetBulletDamage()
        {
            return currentBulletType switch
            {
                BulletType.Small => 1,
                BulletType.Medium => 5,
                BulletType.Large => 20,
                _ => 1
            };
        }

        public void Dispose() { }
    }
} 