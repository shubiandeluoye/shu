using UnityEngine;
using Core.EventSystem;
using SkillModule.Types;
using SkillModule.Effects;
using SkillModule.Events;

namespace SkillModule.Core
{
    /// <summary>
    /// 被动技能基类
    /// 负责：
    /// 1. 技能基础属性
    /// 2. 触发条件判断
    /// 3. 冷却管理
    /// 4. 效果创建
    /// </summary>
    public abstract class PassiveSkillBase : ScriptableObject
    {
        [Header("基础配置")]
        public int SkillId;
        public string SkillName;
        public string Description;
        public TriggerType TriggerType;
        public float Cooldown;

        [Header("触发条件")]
        public float[] TriggerParameters;  // 触发参数（如：血量阈值、伤害阈值等）
        public bool RequireTarget;         // 是否需要目标
        public LayerMask TargetMask;      // 目标层级过滤

        protected EventManager eventManager;
        protected float lastTriggerTime;
        protected bool isEnabled = true;

        public virtual void Initialize()
        {
            eventManager = EventManager.Instance;
            lastTriggerTime = -Cooldown;
            RegisterEvents();
        }

        protected virtual void RegisterEvents()
        {
            eventManager.AddListener<SkillTriggerData>(OnSkillTrigger);
        }

        protected virtual void UnregisterEvents()
        {
            eventManager.RemoveListener<SkillTriggerData>(OnSkillTrigger);
        }

        protected virtual void OnSkillTrigger(SkillTriggerData data)
        {
            if (!CanTrigger(data)) return;
            
            ExecuteSkill(data);
            lastTriggerTime = Time.time;
            
            // 触发技能事件
            eventManager.TriggerEvent(new SkillStartEvent 
            { 
                SkillId = SkillId,
                Position = data.Position,
                Direction = data.Direction
            });
        }

        protected virtual bool CanTrigger(SkillTriggerData data)
        {
            if (!isEnabled) return false;
            if (data.TriggerType != TriggerType) return false;
            if (Time.time - lastTriggerTime < Cooldown) return false;
            if (RequireTarget && data.Target == null) return false;
            if (RequireTarget && !IsValidTarget(data.Target)) return false;
            
            return ValidateParameters(data);
        }

        protected virtual bool ValidateParameters(SkillTriggerData data)
        {
            // 子类重写以实现具体的参数验证
            return true;
        }

        protected virtual bool IsValidTarget(GameObject target)
        {
            return ((1 << target.layer) & TargetMask) != 0;
        }

        protected abstract void ExecuteSkill(SkillTriggerData data);

        public virtual void Enable() => isEnabled = true;
        public virtual void Disable() => isEnabled = false;
        public virtual bool IsReady() => Time.time - lastTriggerTime >= Cooldown;
        public virtual float GetCooldownProgress() => Mathf.Clamp01((Time.time - lastTriggerTime) / Cooldown);
    }
} 