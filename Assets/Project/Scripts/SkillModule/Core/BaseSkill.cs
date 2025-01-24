using SkillModule.Types;
using SkillModule.Events;

namespace SkillModule.Core
{
    public abstract class BaseSkill
    {
        protected SkillData skillData;
        protected SkillState currentState;
        protected float lastUseTime;
        protected float cooldownEndTime;

        protected SkillEventSystem eventSystem => SkillEventSystem.Instance;

        public int SkillId => skillData.SkillId;
        public string SkillName => skillData.SkillName;
        public float Cooldown => skillData.Cooldown;
        public float Duration => skillData.Duration;
        public bool IsActive => currentState == SkillState.Casting;

        protected BaseSkill(SkillData data)
        {
            skillData = data;
            currentState = SkillState.Ready;
            lastUseTime = 0;
            cooldownEndTime = 0;
        }

        public virtual bool CanUse()
        {
            return currentState == SkillState.Ready;
        }

        public virtual void Use()
        {
            if (!CanUse()) return;

            OnSkillStart();
            currentState = SkillState.Casting;
            lastUseTime = GetCurrentTime();

            // 发布技能使用事件
            eventSystem.Publish(SkillEventSystem.EventNames.SkillStart, new SkillEventData 
            { 
                SkillId = SkillId,
                State = currentState,
                TimeStamp = lastUseTime
            });
        }

        public virtual void Cancel()
        {
            if (currentState != SkillState.Casting || !skillData.CanCancel) return;
            
            currentState = SkillState.Ready;
            OnCancel();
            TriggerEndEvent();
        }

        protected abstract void OnSkillStart();
        protected virtual void OnCancel() { }

        protected virtual void TriggerEndEvent()
        {
            eventSystem.Publish(SkillEventSystem.EventNames.SkillEnd, new SkillEventData
            {
                SkillId = SkillId,
                State = currentState,
                TimeStamp = GetCurrentTime()
            });
        }

        protected virtual float GetCurrentTime()
        {
            return System.DateTime.Now.Ticks / 10000000f; // 转换为秒
        }

        protected float GetRemainingCooldown()
        {
            return System.Math.Max(cooldownEndTime - GetCurrentTime(), 0);
        }
    }
}