// 引用核心技能管理器
// @SkillManager: Assets/Project/Scripts/SkillModule/Core/SkillManager.cs
using UnityEngine;
using Core.Singleton;
using Core.EventSystem;
using SkillModule.Core;    // 引用 SkillManager.cs
using SkillModule.Types;

using SkillModule.Effects;
using SkillModule.Events;
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
        [Header("技能配置")]
        [SerializeField] private SkillConfig[] skillConfigs;  // 添加这个数组字段

        private SkillModule.Core.SkillManager skillManager;
        private Dictionary<int, BaseEffect> activeEffects;
        private EventManager eventManager;

        #region 射击点管理
        private Dictionary<GameObject, Transform> shootPoints = new Dictionary<GameObject, Transform>();

        /// <summary>
        /// 注册射击点
        /// </summary>
        public void RegisterShootPoint(GameObject owner, Transform shootPoint)
        {
            if (owner != null && shootPoint != null)
            {
                shootPoints[owner] = shootPoint;
                Debug.Log($"注册射击点: {owner.name}, 位置: {shootPoint.position}");
            }
        }

        /// <summary>
        /// 获取射击点位置
        /// </summary>
        public Vector3 GetShootPosition(GameObject owner)
        {
            if (shootPoints.TryGetValue(owner, out var point))
            {
                return point.position;
            }
            return owner.transform.position; // 如果没有注册射击点，返回物体位置
        }

        /// <summary>
        /// 获取射击方向
        /// </summary>
        public Vector3 GetShootDirection(GameObject owner)
        {
            if (shootPoints.TryGetValue(owner, out var point))
            {
                return point.forward;
            }
            return owner.transform.forward; // 如果没有注册射击点，返回物体朝向
        }
        #endregion

        protected override void OnAwake()
        {
            base.OnAwake();
            Initialize();

            // 注册所有配置的技能
            if (skillConfigs != null)
            {
                foreach (var config in skillConfigs)
                {
                    if (config != null)
                    {
                        RegisterSkill(config);
                    }
                }
            }
        }

        private void Initialize()
        {
            skillManager = GetComponent<SkillModule.Core.SkillManager>();
            activeEffects = new Dictionary<int, BaseEffect>();
            eventManager = EventManager.Instance;

            RegisterEventListeners();
        }

        private void RegisterEventListeners()
        {
            eventManager.AddListener<SkillStartEvent>(OnSkillStart);
            eventManager.AddListener<SkillEndEvent>(OnSkillEnd);
            eventManager.AddListener<SkillCooldownEvent>(OnSkillCooldown);
        }

        #region 公共接口

        /// <summary>
        /// 使用技能
        /// </summary>
        public bool UseSkill(int skillId, Vector3 position, Vector3 direction)
        {
            return skillManager.UseSkill(skillId, position, direction);
        }

        /// <summary>
        /// 注册技能
        /// </summary>
        public void RegisterSkill(Core.SkillConfig config)
        {
            skillManager.RegisterSkill(config);
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
            var config = skillManager.GetSkillConfig(evt.SkillId);
            if (config != null)
            {
                switch (config.Type)
                {
                    case SkillType.Shoot:
                        HandleShootSkill(config as ShootConfig, evt.Position, evt.Direction);
                        break;
                    case SkillType.Box:
                        HandleBoxSkill(config as BoxConfig, evt.Position, evt.Direction);
                        break;
                    case SkillType.Barrier:
                        HandleBarrierSkill(config as BarrierConfig, evt.Position, evt.Direction);
                        break;
                    case SkillType.Heal:
                        HandleHealSkill(config as Skills.HealConfig, evt.Position, evt.Direction);
                        break;
                }
            }
        }

        private void OnSkillEnd(SkillEndEvent evt)
        {
            // 处理技能结束事件
            // 可以在这里清理相关效果
        }

        private void OnSkillCooldown(SkillCooldownEvent evt)
        {
            // 处理技能冷却事件
            // 可以在这里更新UI等
        }

        #endregion

        #region 技能效果处理

        private void HandleShootSkill(ShootConfig config, Vector3 position, Vector3 direction)
        {
            if (config == null) return;

            // 直接使用传入的position和direction
            foreach (float angle in config.ShootAngles)
            {
                Vector3 rotatedDirection = RotateVector(direction, angle);
                var projectile = Instantiate(config.SkillPrefab, position, Quaternion.identity);
                CreateProjectile(
                    projectile,
                    rotatedDirection,
                    config.BulletSpeed,
                    config.BulletDamage,
                    config.BulletLifetime
                );
            }
        }

        private void HandleBoxSkill(BoxConfig config, Vector3 position, Vector3 direction)
        {
            if (config == null) return;

            var box = Instantiate(config.SkillPrefab, position, Quaternion.identity);
            var boxBehaviour = box.GetComponent<BoxBehaviour>();
            if (boxBehaviour != null)
            {
                boxBehaviour.Initialize(config.BoxDurability, config.CanBePushed, config.PushForce);
            }
        }

        private void HandleBarrierSkill(BarrierConfig config, Vector3 position, Vector3 direction)
        {
            if (config == null) return;

            var barrier = Instantiate(config.SkillPrefab, position, Quaternion.identity);
            var barrierBehaviour = barrier.GetComponent<BarrierBehaviour>();
            if (barrierBehaviour != null)
            {
                barrierBehaviour.Initialize(
                    config.BarrierHealth,
                    config.BlocksProjectiles,
                    config.BlocksPlayers,
                    config.DamageReduction,
                    config.Duration
                );
            }
        }

        private void HandleHealSkill(Skills.HealConfig config, Vector3 position, Vector3 direction)
        {
            if (config == null) return;

            // 使用 GameObject.FindObjectOfType 来获取 PlayerSystemManager 实例
            var player = GameObject.FindObjectOfType<PlayerSystemManager>();
            if (player != null)
            {
                // 创建回血效果
                var healEffect = CreateHealEffect(
                    config.SkillPrefab,
                    player.transform.position,
                    config.HealAmount,
                    config.BuffDuration
                );

                // 应用回血效果
                player.ApplyHeal(config.HealAmount);
            }
        }

        /// <summary>
        /// 创建回血效果
        /// </summary>
        public HealEffect CreateHealEffect(GameObject effectPrefab, Vector3 position, int healAmount, float duration)  // 修改参数类型为 int
        {
            var effect = new HealEffect(effectPrefab, position, healAmount, duration);
            RegisterEffect(effect);
            return effect;
        }

        private Vector3 RotateVector(Vector3 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector3(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos,
                0
            );
        }

        #endregion

        protected override void Update()
        {
            if (skillManager == null) return;  // 添加空检查
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
            if (skillManager == null) return;  // 添加空检查
            base.OnDestroy();
            // 清理所有效果
            foreach (var effect in activeEffects.Values)
            {
                effect.End();
            }
            activeEffects.Clear();

            // 取消事件监听
            eventManager.RemoveListener<SkillStartEvent>(OnSkillStart);
            eventManager.RemoveListener<SkillEndEvent>(OnSkillEnd);
            eventManager.RemoveListener<SkillCooldownEvent>(OnSkillCooldown);
        }
    }
} 