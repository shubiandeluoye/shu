using UnityEngine;
using System.Collections.Generic;
using Core.Singleton;

namespace CombatModule
{
    /// <summary>
    /// 战斗系统总管理器
    /// 负责：
    /// 1. 伤害计算
    /// 2. 状态效果管理
    /// 3. 战斗事件分发
    /// </summary>
    public class CombatSystemManager : Singleton<CombatSystemManager>, IStatusEffectHandler
    {
        [Header("战斗配置")]
        [SerializeField] private CombatConfig combatConfig;

        private DamageCalculator damageCalculator;
        private StatusEffectSystem statusEffectSystem;
        private Dictionary<GameObject, System.Action<int>> damageCallbacks;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            damageCalculator = new DamageCalculator(combatConfig.DamageConfig);
            statusEffectSystem = new StatusEffectSystem(this);
            damageCallbacks = new Dictionary<GameObject, System.Action<int>>();
        }

        #region 公共接口
        /// <summary>
        /// 注册伤害回调
        /// </summary>
        public void RegisterDamageCallback(GameObject target, System.Action<int> callback)
        {
            damageCallbacks[target] = callback;
        }

        /// <summary>
        /// 取消注册伤害回调
        /// </summary>
        public void UnregisterDamageCallback(GameObject target)
        {
            if (damageCallbacks.ContainsKey(target))
            {
                damageCallbacks.Remove(target);
            }
        }

        /// <summary>
        /// 处理伤害
        /// </summary>
        public void HandleDamage(GameObject attacker, GameObject target, DamageData damageData)
        {
            if (!IsValidDamageRequest(attacker, target)) return;

            // 计算最终伤害
            int finalDamage = damageCalculator.CalculateDamage(damageData);

            // 通过回调通知目标
            if (damageCallbacks.TryGetValue(target, out var callback))
            {
                callback.Invoke(finalDamage);
            }
        }

        /// <summary>
        /// 添加状态效果
        /// </summary>
        public void AddStatusEffect(GameObject target, StatusEffectData effectData)
        {
            statusEffectSystem.AddEffect(target, effectData);
        }

        /// <summary>
        /// 移除状态效果
        /// </summary>
        public void RemoveStatusEffect(GameObject target, StatusEffectType effectType)
        {
            statusEffectSystem.RemoveEffect(target, effectType);
        }
        #endregion

        #region IStatusEffectHandler Implementation
        public void OnEffectApplied(GameObject target, StatusEffectData effect)
        {
            // 状态效果应用时的处理
            Debug.Log($"Status effect {effect.Type} applied to {target.name}");
        }

        public void OnEffectRemoved(GameObject target, StatusEffectType effectType)
        {
            // 状态效果移除时的处理
            Debug.Log($"Status effect {effectType} removed from {target.name}");
        }

        public void OnEffectStacked(GameObject target, StatusEffectType effectType, int newStackCount)
        {
            // 状态效果叠加时的处理
            Debug.Log($"Status effect {effectType} stacked on {target.name}, new stack: {newStackCount}");
        }

        public void OnEffectTick(GameObject target, DamageData damageData)
        {
            // 处理持续伤害
            HandleDamage(target, target, damageData);
        }
        #endregion

        private bool IsValidDamageRequest(GameObject attacker, GameObject target)
        {
            return attacker != null && target != null && attacker != target;
        }

        protected override void Update()
        {
            base.Update();
            // 更新状态效果
            statusEffectSystem?.Update();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 清理资源
            damageCallbacks.Clear();
            statusEffectSystem?.Dispose();
        }
    }
} 