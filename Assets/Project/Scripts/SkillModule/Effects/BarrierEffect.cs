using UnityEngine;
using SkillModule.Events;
using SkillModule.Types;

namespace SkillModule.Effects
{
    public class BarrierEffect : BaseEffect
    {
        private GameObject barrierInstance;
        private float health;
        private float maxHealth;
        private BarrierEffectConfig config;
        private bool isBreaking;
        private Vector3 originalScale;  // 添加原始大小记录

        public BarrierEffect(GameObject barrierPrefab, Vector3 position, float duration, BarrierEffectConfig config) 
            : base(duration)
        {
            this.config = config;
            this.maxHealth = config.Health;
            this.health = maxHealth;

            if (barrierPrefab != null)
            {
                barrierInstance = GameObject.Instantiate(barrierPrefab, position, Quaternion.identity);
                SetupBarrier();
                originalScale = barrierInstance.transform.localScale;  // 记录原始大小
            }
        }

        private void SetupBarrier()
        {
            if (barrierInstance == null) return;

            // 设置大小
            barrierInstance.transform.localScale = new Vector3(config.Width, config.Height, 1);

            // 设置颜色
            var renderer = barrierInstance.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = config.BarrierColor;
            }

            // 设置碰撞体
            var collider = barrierInstance.GetComponent<BoxCollider2D>();
            if (collider == null) collider = barrierInstance.AddComponent<BoxCollider2D>();
            
            // 设置刚体
            var rigidbody = barrierInstance.GetComponent<Rigidbody2D>();
            if (rigidbody == null) rigidbody = barrierInstance.AddComponent<Rigidbody2D>();
            rigidbody.isKinematic = true;
        }

        public void TakeDamage(float damage)
        {
            if (isBreaking) return;

            health -= damage;
            
            // 更新视觉效果
            UpdateBarrierVisuals();

            if (health <= 0)
            {
                Break();
            }
        }

        private void Break()
        {
            isBreaking = true;
            
            // 触发破碎事件
            eventManager.TriggerEvent(new SkillEffectEvent
            {
                EffectId = effectId,
                Type = (SkillEffectType)EffectType.Shield,
                Parameters = new float[] { 0 }
            });

            // 可以在这里添加破碎特效
            End();
        }

        protected override void OnEffectStart()
        {
            if (config.PulseEffect)
            {
                // 启动脉冲效果
                StartPulseEffect();
            }
        }

        protected override void OnEffectUpdate()
        {
            if (barrierInstance != null && !isBreaking)
            {
                UpdateBarrierVisuals();
                if (config.PulseEffect)
                {
                    UpdatePulseEffect();
                }
            }
        }

        private void UpdateBarrierVisuals()
        {
            if (barrierInstance == null) return;

            var renderer = barrierInstance.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                // 根据血量更新颜色
                float healthPercent = health / maxHealth;
                Color color = Color.Lerp(config.DamagedColor, config.BarrierColor, healthPercent);
                renderer.color = color;
            }
        }

        private void StartPulseEffect()
        {
            // 脉冲效果的初始化已经在构造函数中完成
            // 这里可以添加其他初始化逻辑
        }

        private void UpdatePulseEffect()
        {
            if (barrierInstance == null) return;

            // 实现脉冲效果的更新
            float pulse = (1 + Mathf.Sin(Time.time * config.PulseSpeed) * config.PulseIntensity);
            barrierInstance.transform.localScale = originalScale * pulse;  // 使用原始大小
        }

        protected override void OnEffectEnd()
        {
            if (barrierInstance != null)
            {
                // 恢复原始大小
                barrierInstance.transform.localScale = originalScale;
                GameObject.Destroy(barrierInstance);
            }
        }

        protected override void OnEffectInterrupt(string reason)
        {
            OnEffectEnd();
        }
    }

    public struct BarrierEffectConfig
    {
        public float Width;
        public float Height;
        public float Health;
        public Color BarrierColor;
        public Color DamagedColor;
        public bool PulseEffect;
        public float PulseSpeed;
        public float PulseIntensity;
    }
} 