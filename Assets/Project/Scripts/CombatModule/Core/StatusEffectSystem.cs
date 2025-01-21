using UnityEngine;
using System.Collections.Generic;

namespace CombatModule
{
    public interface IStatusEffectHandler
    {
        void OnEffectApplied(GameObject target, StatusEffectData effect);
        void OnEffectRemoved(GameObject target, StatusEffectType effectType);
        void OnEffectStacked(GameObject target, StatusEffectType effectType, int newStackCount);
        void OnEffectTick(GameObject target, DamageData damageData);
    }

    /// <summary>
    /// 状态效果系统
    /// 负责：
    /// 1. 管理所有状态效果
    /// 2. 更新状态效果
    /// 3. 处理状态效果的叠加和移除
    /// </summary>
    public class StatusEffectSystem
    {
        private Dictionary<GameObject, List<StatusEffectData>> activeEffects;
        private IStatusEffectHandler effectHandler;

        public StatusEffectSystem(IStatusEffectHandler handler)
        {
            activeEffects = new Dictionary<GameObject, List<StatusEffectData>>();
            effectHandler = handler;
        }

        public void AddEffect(GameObject target, StatusEffectData effectData)
        {
            if (!activeEffects.ContainsKey(target))
            {
                activeEffects[target] = new List<StatusEffectData>();
            }

            // 检查是否已存在同类效果
            var existingEffect = activeEffects[target].Find(e => e.Type == effectData.Type);
            if (existingEffect != null)
            {
                // 刷新持续时间
                existingEffect.Duration = effectData.Duration;
                effectHandler?.OnEffectStacked(target, effectData.Type, 1);
            }
            else
            {
                // 添加新效果
                activeEffects[target].Add(effectData);
                effectHandler?.OnEffectApplied(target, effectData);
            }
        }

        public void RemoveEffect(GameObject target, StatusEffectType effectType)
        {
            if (!activeEffects.ContainsKey(target)) return;

            var effect = activeEffects[target].Find(e => e.Type == effectType);
            if (effect != null)
            {
                activeEffects[target].Remove(effect);
                effectHandler?.OnEffectRemoved(target, effectType);
            }
        }

        public void Update()
        {
            var targets = new List<GameObject>(activeEffects.Keys);
            foreach (var target in targets)
            {
                if (target == null)
                {
                    activeEffects.Remove(target);
                    continue;
                }

                UpdateTargetEffects(target);
            }
        }

        private void UpdateTargetEffects(GameObject target)
        {
            var effects = activeEffects[target];
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                var effect = effects[i];
                effect.Duration -= Time.deltaTime;

                // 处理持续伤害效果
                if (effect.HasTickEffect && effect.Type == StatusEffectType.Dot)
                {
                    var damageData = new DamageData
                    {
                        BaseDamage = effect.Value,
                        DamageType = DamageType.Status,
                        HasKnockback = false
                    };
                    effectHandler?.OnEffectTick(target, damageData);
                }

                // 移除过期效果
                if (effect.Duration <= 0)
                {
                    effects.RemoveAt(i);
                    effectHandler?.OnEffectRemoved(target, effect.Type);
                }
            }
        }

        public void Dispose()
        {
            activeEffects.Clear();
        }

        private void HandleStatusEffect(GameObject target, StatusEffectData effectData)
        {
            switch (effectData.Type)
            {
                case StatusEffectType.Stun:
                    // 直接添加眩晕效果
                    AddEffect(target, effectData);
                    break;
                case StatusEffectType.Slow:
                    // 直接添加减速效果
                    AddEffect(target, effectData);
                    break;
                case StatusEffectType.Dot:
                    // 直接添加持续伤害效果
                    AddEffect(target, effectData);
                    break;
            }
        }
    }
} 