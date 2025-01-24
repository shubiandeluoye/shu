using SkillModule.Types;
using SkillModule.Events;

namespace SkillModule.Core
{
    /// <summary>
    /// 被动技能基类
    /// 负责：
    /// 1. 技能基础属性
    /// 2. 触发条件判断
    /// 3. 冷却管理
    /// </summary>
    public abstract class PassiveSkillBase
    {
        protected SkillData skillData;
        protected float lastTriggerTime;
        protected bool isEnabled = true;
        protected SkillEventSystem eventSystem => SkillEventSystem.Instance;

        public int SkillId => skillData.SkillId;
        public string SkillName => skillData.SkillName;
        public float Cooldown => skillData.Cooldown;

        protected PassiveSkillBase(SkillData data)
        {
            skillData = data;
            lastTriggerTime = -Cooldown;
            Initialize();
        }

        public virtual void Initialize()
        {
            RegisterEvents();
        }

        protected virtual void RegisterEvents()
        {
            // 子类实现具体的事件注册
        }

        protected virtual void UnregisterEvents()
        {
            // 子类实现具体的事件注销
        }

        protected virtual bool CanTrigger(SkillContext context)
        {
            if (!isEnabled) return false;
            if (GetCurrentTime() - lastTriggerTime < Cooldown) return false;
            
            return ValidateParameters(context);
        }

        protected virtual bool ValidateParameters(SkillContext context)
        {
            // 子类重写以实现具体的参数验证
            return true;
        }

        protected abstract void ExecuteSkill(SkillContext context);

        public virtual void Enable() => isEnabled = true;
        public virtual void Disable() => isEnabled = false;
        public virtual bool IsReady() => GetCurrentTime() - lastTriggerTime >= Cooldown;
        
        protected virtual float GetCurrentTime()
        {
            return System.DateTime.Now.Ticks / 10000000f; // 转换为秒
        }

        public virtual void Trigger(SkillContext context)
        {
            if (!CanTrigger(context)) return;

            ExecuteSkill(context);
            lastTriggerTime = GetCurrentTime();

            // 触发技能事件
            eventSystem.Publish(SkillEventSystem.EventNames.SkillStart, new SkillEventData 
            { 
                SkillId = SkillId,
                State = SkillState.Casting,
                TimeStamp = lastTriggerTime
            });
        }
    }
} 