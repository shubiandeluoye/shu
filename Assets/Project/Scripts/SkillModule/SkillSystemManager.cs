// 引用核心技能管理器
// @SkillManager: Assets/Project/Scripts/SkillModule/Core/SkillManager.cs
using UnityEngine;
using Core.Singleton;
using SkillModule.Events;  // 改用技能模块的事件系统
using SkillModule.Core;
using SkillModule.Types;
using SkillModule.Effects;
using System.Collections.Generic;
using SkillModule.Skills;
using PlayerModule;  // 添加 PlayerModule 引用

namespace SkillModule
{
    /// <summary>
    /// 技能系统总管理器
    /// 负责：
    /// 1. 整合所有子系统
    /// 2. 提供对外接口
    /// 3. 处理系统间通信
    /// 4. 管理技能生命周期
    /// 依赖：
    /// - SkillModule.Core.SkillManager
    /// </summary>
    public class SkillSystemManager : Singleton<SkillSystemManager>
    {
        private SkillEventManager eventManager => SkillEventManager.Instance;  // 使用技能模块的事件管理器
        private Dictionary<int, BaseEffect> activeEffects;
        private Dictionary<int, BaseSkill> skills = new Dictionary<int, BaseSkill>();
        private SkillEventSystem eventSystem => SkillEventSystem.Instance;

        protected override void OnAwake()
        {
            base.OnAwake();
            RegisterEventListeners();
            Initialize();
        }

        private void RegisterEventListeners()
        {
            // 技能事件
            eventManager.AddListener<SkillStartEvent>(OnSkillStart);
            eventManager.AddListener<SkillEndEvent>(OnSkillEnd);
            eventManager.AddListener<SkillCooldownEvent>(OnSkillCooldown);
            eventManager.AddListener<SkillInterruptEvent>(OnSkillInterrupt);
            
            // 效果事件
            eventManager.AddListener<AddEffectEvent>(OnEffectAdded);
            eventManager.AddListener<RemoveEffectEvent>(OnEffectRemoved);
            eventManager.AddListener<EffectStateChangeEvent>(OnEffectStateChanged);
            
            // 伤害和治疗事件
            eventManager.AddListener<DamageEvent>(OnDamageDealt);
            eventManager.AddListener<HealingEvent>(OnHealingApplied);
            eventManager.AddListener<ShieldEvent>(OnShieldChanged);
            eventManager.AddListener<ProjectileEvent>(OnProjectileSpawned);
            eventManager.AddListener<AreaEffectEvent>(OnAreaEffectCreated);
        }

        private void Initialize()
        {
            // 订阅相关事件
            eventSystem.Subscribe("SkillUsed", OnSkillUsed);
        }

        #region 公共接口

        /// <summary>
        /// 使用技能
        /// </summary>
        public bool UseSkill(int skillId, Vector3 position, Vector3 direction)
        {
            Debug.Log($"[{System.DateTime.Now:HH:mm:ss.fff}][SkillSystemManager] Trying to use skill {skillId} at position {position}, skillManager is: {(eventManager == null ? "null" : "not null")}");
            return eventManager.UseSkill(skillId, position, direction);
        }

        /// <summary>
        /// 注册技能
        /// </summary>
        public void RegisterSkill(Core.SkillConfig config)
        {
            eventManager.RegisterSkill(config);
        }

        /// <summary>
        /// 创建投射物效果
        /// </summary>
        public ProjectileEffect CreateProjectile(GameObject projectile, Vector3 direction, float speed, float damage, float duration)
        {
            var effect = new ProjectileEffect(projectile, direction, speed, damage, duration);
            RegisterEffect(effect);
            return effect;
        }

        /// <summary>
        /// 创建区域效果
        /// </summary>
        public AreaEffect CreateAreaEffect(GameObject area, Vector2 size, float duration, AreaEffectConfig config)
        {
            var effect = new AreaEffect(area, area.transform.position, size, duration, config);
            RegisterEffect(effect);
            return effect;
        }

        /// <summary>
        /// 创建Buff效果
        /// </summary>
        public AreaEffect CreateBuff(GameObject target, float duration, AreaEffectConfig config)
        {
            var effect = new AreaEffect(
                config.SkillPrefab,
                target.transform.position,
                new Vector2(1, 1),
                duration,
                config
            );
            RegisterEffect(effect);
            return effect;
        }

        #endregion

        #region 效果管理

        private void RegisterEffect(BaseEffect effect)
        {
            int effectId = effect.GetHashCode();
            activeEffects[effectId] = effect;
            effect.Start();
        }

        private void RemoveEffect(int effectId)
        {
            if (activeEffects.TryGetValue(effectId, out var effect))
            {
                effect.End();
                activeEffects.Remove(effectId);
            }
        }

        #endregion

        #region 事件处理

        private void OnSkillStart(SkillStartEvent evt)
        {
            Debug.Log($"[SkillSystem] Skill {evt.SkillId} started at {evt.Position}");
        }

        private void OnSkillEnd(SkillEndEvent evt)
        {
            Debug.Log($"[SkillSystem] Skill {evt.SkillId} ended. Success: {evt.WasSuccessful}");
        }

        private void OnSkillCooldown(SkillCooldownEvent evt)
        {
            Debug.Log($"[SkillSystem] Skill {evt.SkillId} cooldown: {evt.RemainingCooldown:F1}s");
        }

        private void OnSkillInterrupt(SkillInterruptEvent evt)
        {
            Debug.Log($"[SkillSystem] Skill {evt.SkillId} interrupted: {evt.InterruptReason}");
        }

        private void OnEffectAdded(AddEffectEvent evt)
        {
            Debug.Log($"[SkillSystem] Effect added to {evt.Target.name}, duration: {evt.Duration}s");
        }

        private void OnEffectRemoved(RemoveEffectEvent evt)
        {
            Debug.Log($"[SkillSystem] Effect {evt.EffectId} removed: {evt.Reason}");
        }

        private void OnEffectStateChanged(EffectStateChangeEvent evt)
        {
            Debug.Log($"[SkillSystem] Effect {evt.EffectId} state changed to {evt.IsActive}");
        }

        private void OnDamageDealt(DamageEvent evt)
        {
            Debug.Log($"[SkillSystem] {evt.Source.name} dealt {evt.Damage} damage to {evt.Target.name}");
        }

        private void OnHealingApplied(HealingEvent evt)
        {
            Debug.Log($"[SkillSystem] {evt.Source.name} healed {evt.Target.name} for {evt.Amount}");
        }

        private void OnShieldChanged(ShieldEvent evt)
        {
            Debug.Log($"[SkillSystem] Shield on {evt.Target.name}: {evt.ShieldAmount}");
        }

        private void OnProjectileSpawned(ProjectileEvent evt)
        {
            Debug.Log($"[SkillSystem] Projectile spawned: {evt.ProjectileType}");
        }

        private void OnAreaEffectCreated(AreaEffectEvent evt)
        {
            Debug.Log($"[SkillSystem] Area effect created at {evt.Position}, radius: {evt.Radius}");
        }

        private void OnSkillUsed(object eventData)
        {
            if (eventData is SkillEventData data)
            {
                // 处理技能使用事件
                // 可以在这里实现技能效果、动画等
            }
        }

        #endregion

        protected override void Update()
        {
            base.Update();
            // 更新所有活动效果
            var effectIds = new List<int>(activeEffects.Keys);
            foreach (var effectId in effectIds)
            {
                if (activeEffects.TryGetValue(effectId, out var effect))
                {
                    effect.Update();
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 清理所有效果
            foreach (var effect in activeEffects.Values)
            {
                effect.End();
            }
            activeEffects.Clear();

            UnregisterEventListeners();
        }

        private void UnregisterEventListeners()
        {
            if (eventManager == null) return;

            // 技能事件
            eventManager.RemoveListener<SkillStartEvent>(OnSkillStart);
            eventManager.RemoveListener<SkillEndEvent>(OnSkillEnd);
            eventManager.RemoveListener<SkillCooldownEvent>(OnSkillCooldown);
            eventManager.RemoveListener<SkillInterruptEvent>(OnSkillInterrupt);
            
            // 效果事件
            eventManager.RemoveListener<AddEffectEvent>(OnEffectAdded);
            eventManager.RemoveListener<RemoveEffectEvent>(OnEffectRemoved);
            eventManager.RemoveListener<EffectStateChangeEvent>(OnEffectStateChanged);
            
            // 伤害和治疗事件
            eventManager.RemoveListener<DamageEvent>(OnDamageDealt);
            eventManager.RemoveListener<HealingEvent>(OnHealingApplied);
            eventManager.RemoveListener<ShieldEvent>(OnShieldChanged);
            eventManager.RemoveListener<ProjectileEvent>(OnProjectileSpawned);
            eventManager.RemoveListener<AreaEffectEvent>(OnAreaEffectCreated);
        }

        public Dictionary<string, int> GetEventStatistics()
        {
            return eventManager.GetEventStatistics();
        }
    }
} 