using UnityEngine;
using Core.EventSystem;
using PlayerModule.Data;
using SkillModule.Types;
using SkillModule.Events;
using SkillModule.Core;  // 添加这行，引入 SkillContext 和 SkillConfig
using SkillModule;      // 添加这行，引入 SkillSystemManager

namespace PlayerModule.Systems
{
    /// <summary>
    /// 射击系统
    /// 负责：
    /// 1. 管理射击角度
    /// 2. 处理子弹类型切换
    /// 3. 发射子弹
    /// </summary>
    public class ShootingSystem
    {
        private readonly ShootingConfig config;
        private Transform shootPoint;
        private float lastShootTime;
        private BulletType currentBulletType;
        private float currentAngle;
        private GameObject owner;  // 添加所有者引用

        public BulletType CurrentBulletType => currentBulletType;

        public ShootingSystem(ShootingConfig config, GameObject owner)  // 修改构造函数
        {
            this.config = config;
            this.owner = owner;
            currentBulletType = BulletType.Small;
        }

        public void Initialize(Transform shootPoint)
        {
            this.shootPoint = shootPoint;
            lastShootTime = -config.ShootCooldown;
            currentAngle = 30f; // 默认30度
        }

        public void Shoot(float angle)
        {
            if (Time.time < lastShootTime + config.ShootCooldown) return;

            Vector3 shootPosition = shootPoint.position + (Vector3.right * config.BulletSpawnOffset);
            Vector3 shootDirection = Quaternion.Euler(0, 0, angle) * Vector3.right;

            // 创建技能上下文
            var context = new SkillContext(owner)
                .WithPosition(shootPosition)
                .WithDirection(shootDirection)
                .WithParameters(GetBulletDamage(), GetBulletSpeed());

            // 使用技能系统的接口
            if (SkillSystemManager.Instance.UseSkill((int)currentBulletType, shootPosition, shootDirection))
            {
                lastShootTime = Time.time;
            }
        }

        public void SwitchBulletType(BulletType newType)
        {
            if (currentBulletType == newType) return;
            currentBulletType = newType;
            
            // 创建技能配置
            var config = ScriptableObject.CreateInstance<SkillConfig>();
            config.SkillId = (int)newType;
            config.Type = SkillType.Shoot;  // 现在可以正确引用 SkillType
            config.SkillName = $"Bullet_{newType}";
            config.Cooldown = this.config.ShootCooldown;

            // 注册技能
            SkillSystemManager.Instance.RegisterSkill(config);
        }

        public void ToggleAngle()
        {
            currentAngle = currentAngle == 30f ? 45f : 30f;
        }

        private float GetBulletSpeed()
        {
            switch (currentBulletType)
            {
                case BulletType.Small:
                    return 10f;
                case BulletType.Medium:
                    return 8f;
                case BulletType.Large:
                    return 5f;
                default:
                    return 10f;
            }
        }

        private int GetBulletDamage()
        {
            switch (currentBulletType)
            {
                case BulletType.Small:
                    return 1;
                case BulletType.Medium:
                    return 5;
                case BulletType.Large:
                    return 20;
                default:
                    return 1;
            }
        }

        private void HandleShoot(SkillTriggerData triggerData)
        {
            // 使用完全限定名称解决歧义
            EventManager.Instance.TriggerEvent(new PlayerModule.Data.PlayerShootEvent 
            { 
                SkillId = triggerData.SkillId,
                Position = triggerData.Position,
                Direction = triggerData.Direction,
                Parameters = triggerData.Parameters
            });
        }

        private void OnBulletTypeSwitch(BulletTypeSwitchEvent evt)
        {
            // 使用技能ID作为子弹类型
            var triggerData = new SkillTriggerData
            {
                SkillId = evt.SkillId,
                TriggerType = TriggerType.Custom,
                Parameters = new float[] { evt.BulletData.Damage, evt.BulletData.Speed }
            };
            // ... 处理子弹类型切换
        }
    }
} 