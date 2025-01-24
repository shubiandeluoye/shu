using UnityEngine;
using PlayerModule.Data;
using SkillModule.Utils;

namespace PlayerModule.Systems
{
    /// <summary>
    /// 生命系统
    /// 职责：
    /// 1. 生命值状态管理
    /// 2. 无敌时间管理
    /// 3. 生命值变化通知
    /// </summary>
    public class HealthSystem : IPlayerSystem
    {
        private readonly HealthConfig config;
        private int currentHealth;
        private float lastDamageTime;
        private bool isInvincible;
        private IPlayerManager manager;

        public int CurrentHealth => currentHealth;
        public bool IsAlive => currentHealth > 0;

        public HealthSystem(HealthConfig config)
        {
            this.config = config;
        }

        public void Initialize(IPlayerManager manager)
        {
            this.manager = manager;
            currentHealth = config.MaxHealth;
            lastDamageTime = -config.InvincibilityTime;
            isInvincible = false;
        }

        /// <summary>
        /// 修改生命值（由技能系统调用）
        /// </summary>
        public void ModifyHealth(int amount, ModifyHealthType modifyType)
        {
            // 检查是否可以修改血量
            if (!IsAlive || (amount < 0 && isInvincible)) return;

            int oldHealth = currentHealth;
            
            // 应用生命值变化
            currentHealth = System.Math.Clamp(currentHealth + amount, 0, config.MaxHealth);

            // 如果是伤害，设置无敌时间
            if (amount < 0)
            {
                lastDamageTime = SkillUtils.GetCurrentTime();
                isInvincible = true;
            }

            // 发送生命值变化事件
            if (currentHealth != oldHealth)
            {
                manager.PublishEvent("PlayerHealthChanged", new PlayerHealthChangedEvent 
                { 
                    CurrentHealth = currentHealth,
                    PreviousHealth = oldHealth,
                    ChangeAmount = amount,
                    ModifyType = modifyType
                });

                // 检查死亡
                if (!IsAlive)
                {
                    OnDeath();
                }
            }
        }

        public void Update(float deltaTime)
        {
            // 更新无敌状态
            if (isInvincible && SkillUtils.GetCurrentTime() >= lastDamageTime + config.InvincibilityTime)
            {
                isInvincible = false;
            }
        }

        private void OnDeath()
        {
            manager.PublishEvent("PlayerDeath", new PlayerDeathEvent 
            { 
                DeathPosition = Vector3.zero,
                DeathReason = DeathReason.HealthDepleted
            });
        }

        public bool IsInvincible() => isInvincible;

        public void Dispose()
        {
            // 清理资源
        }
    }
} 