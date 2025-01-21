using UnityEngine;
using Core.EventSystem;

namespace SkillModule.Effects
{
    /// <summary>
    /// 效果基类
    /// 负责：
    /// 1. 效果生命周期管理
    /// 2. 效果参数配置
    /// 3. 效果状态控制
    /// </summary>
    public abstract class BaseEffect
    {
        protected int effectId;
        protected float duration;
        protected float elapsedTime;
        protected bool isActive;
        protected GameObject target;
        protected EventManager eventManager;

        public int EffectId => effectId;
        public bool IsActive => isActive;
        public float RemainingTime => Mathf.Max(0, duration - elapsedTime);
        public float Progress => Mathf.Clamp01(elapsedTime / duration);

        protected BaseEffect(float duration)
        {
            this.duration = duration;
            this.effectId = GenerateEffectId();
            this.eventManager = EventManager.Instance;
            this.isActive = true;
        }

        public virtual void Start()
        {
            isActive = true;
            elapsedTime = 0;
            OnEffectStart();
        }

        public virtual void Update()
        {
            if (!isActive) return;

            elapsedTime += Time.deltaTime;
            OnEffectUpdate();

            if (elapsedTime >= duration)
            {
                End();
            }
        }

        public virtual void End()
        {
            if (!isActive) return;
            
            isActive = false;
            OnEffectEnd();
        }

        public virtual void Interrupt(string reason = null)
        {
            if (!isActive) return;

            isActive = false;
            OnEffectInterrupt(reason);
        }

        protected abstract void OnEffectStart();
        protected abstract void OnEffectUpdate();
        protected abstract void OnEffectEnd();
        protected virtual void OnEffectInterrupt(string reason) { }

        private static int effectIdCounter = 0;
        private static int GenerateEffectId()
        {
            return ++effectIdCounter;
        }
    }
} 