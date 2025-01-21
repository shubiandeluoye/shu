using UnityEngine;
using SkillModule.Events;
using SkillModule.Types;

namespace SkillModule.Effects
{
    public class HealEffect : BaseEffect
    {
        private GameObject effectPrefab;
        private GameObject effectInstance;
        private int healAmount;
        private HealEffectConfig config;

        public HealEffect(GameObject prefab, Vector3 position, int amount, float duration) 
            : base(duration)
        {
            effectPrefab = prefab;
            healAmount = amount;
            
            if (effectPrefab != null)
            {
                effectInstance = GameObject.Instantiate(effectPrefab, position, Quaternion.identity);
            }
        }

        public void Initialize(HealEffectConfig config)
        {
            this.config = config;
            if (effectInstance != null)
            {
                var renderer = effectInstance.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = config.Color;
                }
                effectInstance.transform.localScale = Vector3.one * config.Scale;
            }
        }

        protected override void OnEffectStart()
        {
            // 触发治疗效果事件
            eventManager.TriggerEvent(new SkillEffectEvent
            {
                EffectId = effectId,
                Type = (SkillEffectType)EffectType.Heal,
                Parameters = new float[] { healAmount }
            });
        }

        protected override void OnEffectUpdate()
        {
            if (effectInstance != null)
            {
                // 更新特效（例如：缩放、颜色渐变等）
                float normalizedTime = elapsedTime / duration;
                float scale = Mathf.Lerp(config.Scale, config.Scale * 0.5f, normalizedTime);
                effectInstance.transform.localScale = Vector3.one * scale;

                var renderer = effectInstance.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = config.Color;
                    color.a = 1 - normalizedTime;
                    renderer.color = color;
                }
            }
        }

        protected override void OnEffectEnd()
        {
            if (effectInstance != null)
            {
                GameObject.Destroy(effectInstance);
            }
        }

        protected override void OnEffectInterrupt(string reason)
        {
            OnEffectEnd();
        }
    }

    public struct HealEffectConfig
    {
        public Color Color;
        public float Scale;
        public float Duration;
    }
} 