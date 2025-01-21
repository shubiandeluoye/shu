using UnityEngine;
using System.Collections.Generic;

namespace SkillModule.Effects
{
    /// <summary>
    /// 效果控制器
    /// 负责管理和更新所有活动的效果
    /// </summary>
    public class EffectController : MonoBehaviour
    {
        private Dictionary<int, BaseEffect> activeEffects = new Dictionary<int, BaseEffect>();
        private List<int> effectsToRemove = new List<int>();

        private void Update()
        {
            // 更新所有活动效果
            foreach (var effect in activeEffects.Values)
            {
                if (effect.IsActive)
                {
                    effect.Update();
                }
                else
                {
                    effectsToRemove.Add(effect.EffectId);
                }
            }

            // 移除已结束的效果
            foreach (var effectId in effectsToRemove)
            {
                RemoveEffect(effectId);
            }
            effectsToRemove.Clear();
        }

        public void AddEffect(BaseEffect effect)
        {
            if (effect == null) return;

            if (!activeEffects.ContainsKey(effect.EffectId))
            {
                activeEffects.Add(effect.EffectId, effect);
                effect.Start();
            }
        }

        public void RemoveEffect(int effectId)
        {
            if (activeEffects.TryGetValue(effectId, out var effect))
            {
                effect.End();
                activeEffects.Remove(effectId);
            }
        }

        public void RemoveAllEffects()
        {
            foreach (var effect in activeEffects.Values)
            {
                effect.End();
            }
            activeEffects.Clear();
        }

        public BaseEffect GetEffect(int effectId)
        {
            activeEffects.TryGetValue(effectId, out var effect);
            return effect;
        }

        public void InterruptEffect(int effectId, string reason = null)
        {
            if (activeEffects.TryGetValue(effectId, out var effect))
            {
                effect.Interrupt(reason);
                effectsToRemove.Add(effectId);
            }
        }

        public bool HasEffect(int effectId)
        {
            return activeEffects.ContainsKey(effectId);
        }

        public int GetActiveEffectCount()
        {
            return activeEffects.Count;
        }

        private void OnDestroy()
        {
            RemoveAllEffects();
        }
    }
} 