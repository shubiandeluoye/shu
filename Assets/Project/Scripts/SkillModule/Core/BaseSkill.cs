using UnityEngine;
using Core.EventSystem;
using Core.Timer;
using SkillModule.Events;
using SkillModule.Types;

namespace SkillModule.Core
{
    /// <summary>
    /// 技能基类
    /// 负责：
    /// 1. 基础技能属性
    /// 2. 冷却管理
    /// 3. 技能状态控制
    /// 4. 事件分发
    /// </summary>
    public abstract class BaseSkill
    {
        protected SkillConfig config;
        protected EventManager eventManager;
        protected bool isReady = true;
        protected float lastUseTime;
        protected bool isActive;
        protected float cooldownEndTime;
        protected int timerId;

        public int SkillId => config.SkillId;
        public string SkillName => config.SkillName;
        public float Cooldown => config.Cooldown;
        public float Duration => config.Duration;
        public bool IsActive => isActive;

        protected BaseSkill(SkillConfig config)
        {
            this.config = config;
            eventManager = EventManager.Instance;
            lastUseTime = -config.Cooldown;
            isActive = false;
            cooldownEndTime = 0;
        }

        public virtual void Initialize()
        {
            isReady = true;
        }

        public virtual bool Execute(Vector3 position, Vector3 direction)
        {
            if (!CanExecuteSkill()) return false;

            ExecuteSkillInternal(position, direction);
            return true;
        }

        protected virtual bool CanExecuteSkill()
        {
            if (!isReady) return false;
            if (Time.time - lastUseTime < config.Cooldown) return false;
            return true;
        }

        protected virtual void ExecuteSkillInternal(Vector3 position, Vector3 direction)
        {
            // 触发技能开始事件
            TriggerSkillStartEvent(position, direction);

            // 开始冷却
            StartCooldown();

            // 如果有持续时间，设置技能结束
            if (config.Duration > 0)
            {
                TimerManager.Instance.AddTimer(config.Duration, () =>
                {
                    OnSkillEnd();
                });
            }
        }

        protected virtual void StartCooldown()
        {
            lastUseTime = Time.time;
            isReady = false;

            // 触发冷却开始事件
            TriggerCooldownEvent();

            // 设置冷却结束
            TimerManager.Instance.AddTimer(config.Cooldown, () =>
            {
                isReady = true;
            });
        }

        protected virtual void OnSkillEnd()
        {
            TriggerSkillEndEvent();
        }

        public virtual float GetCooldownProgress()
        {
            float timeSinceLastUse = Time.time - lastUseTime;
            return Mathf.Clamp01(timeSinceLastUse / config.Cooldown);
        }

        public virtual string GetSkillDescription()
        {
            return config.Description;
        }

        public virtual void Enable()
        {
            isReady = true;
        }

        public virtual void Disable()
        {
            isReady = false;
        }

        protected void TriggerSkillStartEvent(Vector3 position, Vector3 direction)
        {
            eventManager.TriggerEvent(new SkillStartEvent 
            { 
                SkillId = config.SkillId,
                Position = position,
                Direction = direction
            });
        }

        protected void TriggerSkillEndEvent(bool wasSuccessful = true, string failReason = "")
        {
            eventManager.TriggerEvent(new SkillEndEvent
            {
                SkillId = config.SkillId,
                WasSuccessful = wasSuccessful,
                FailReason = failReason
            });
        }

        protected void TriggerCooldownEvent()
        {
            UpdateCooldown();
        }

        protected void UpdateCooldown()
        {
            if (!isReady)
            {
                float remainingTime = cooldownEndTime - Time.time;
                eventManager.TriggerEvent(new SkillCooldownEvent
                {
                    SkillId = config.SkillId,
                    CooldownTime = remainingTime
                });
            }
        }

        public virtual bool CanUse()
        {
            return !isActive && Time.time >= cooldownEndTime;
        }

        public virtual void Use(Vector3 position, Vector3 direction)
        {
            if (!CanUse()) return;

            isActive = true;
            cooldownEndTime = Time.time + config.Cooldown;

            // 如果有持续时间，启动计时器
            if (config.Duration > 0)
            {
                timerId = TimerManager.Instance.AddTimer(config.Duration, OnDurationEnd);
            }

            // 触发技能开始事件
            TriggerStartEvent(position, direction);

            // 执行具体技能逻辑
            OnUse(position, direction);
        }

        public virtual void Cancel()
        {
            if (!isActive || !config.CanCancel) return;

            // 如果有计时器，取消它
            if (timerId != 0)
            {
                TimerManager.Instance.RemoveTimer(timerId);
                timerId = 0;
            }

            isActive = false;
            OnCancel();
            TriggerEndEvent();
        }

        protected virtual void OnUse(Vector3 position, Vector3 direction)
        {
            // 子类实现具体技能逻辑
        }

        protected virtual void OnCancel()
        {
            // 子类实现取消逻辑
        }

        protected virtual void OnDurationEnd()
        {
            isActive = false;
            timerId = 0;
            TriggerEndEvent();
        }

        protected virtual void TriggerStartEvent(Vector3 position, Vector3 direction)
        {
            EventManager.Instance.TriggerEvent(new SkillStartEvent
            {
                SkillId = config.SkillId,
                Position = position,
                Direction = direction
            });
        }

        protected virtual void TriggerEndEvent()
        {
            EventManager.Instance.TriggerEvent(new SkillEndEvent
            {
                SkillId = config.SkillId
            });
        }
    }
} 