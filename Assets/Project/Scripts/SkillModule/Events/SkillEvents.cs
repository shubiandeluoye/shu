using UnityEngine;
using SkillModule.Core;
using SkillModule.Types;
using System;
using System.Collections.Generic;

namespace SkillModule.Events
{
    /// <summary>
    /// 技能开始事件
    /// </summary>
    public class SkillStartEvent
    {
        public int SkillId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public object Caster { get; set; }
        public float StartTime { get; set; }
    }

    /// <summary>
    /// 技能结束事件
    /// </summary>
    public class SkillEndEvent
    {
        public int SkillId { get; set; }
        public bool WasSuccessful { get; set; }
        public string EndReason { get; set; }
        public float Duration { get; set; }
    }

    /// <summary>
    /// 技能冷却事件
    /// </summary>
    public class SkillCooldownEvent
    {
        public int SkillId { get; set; }
        public float CooldownDuration { get; set; }
        public float RemainingCooldown { get; set; }
        public bool IsReady { get; set; }
    }

    /// <summary>
    /// 技能打断事件
    /// </summary>
    public class SkillInterruptEvent
    {
        public int SkillId { get; set; }
        public string InterruptReason { get; set; }
        public GameObject Interrupter { get; set; }
    }

    /// <summary>
    /// 技能状态变更事件
    /// </summary>
    public class SkillStateChangeEvent
    {
        public int SkillId { get; set; }
        public SkillState OldState { get; set; }
        public SkillState NewState { get; set; }
        public float StateChangeTime { get; set; }
    }

    /// <summary>
    /// 技能效果事件
    /// </summary>
    public struct SkillEffectEvent
    {
        public int SkillId;
        public int EffectId;
        public SkillEffectType Type;
        public GameObject Target;
        public float[] Parameters;
    }

    public struct HealEvent
    {
        public GameObject Source;
        public GameObject Target;
        public float Amount;
        public bool IsPercentage;
        public bool CanOverheal;
    }

    public struct EffectEvent
    {
        public GameObject Target;
        public int EffectId;
        public float Duration;
        public float Strength;
    }

    /// <summary>
    /// 子弹数据
    /// </summary>
    public struct BulletData
    {
        public int SkillId;           
        public float Damage;          
        public float Speed;           
        public Vector3 Direction;     
        public GameObject Source;     
    }

    /// <summary>
    /// 子弹类型切换事件
    /// </summary>
    public struct BulletTypeSwitchEvent
    {
        public int SkillId;           
        public BulletData BulletData; 
    }

    public class SkillEventSystem
    {
        private static SkillEventSystem instance;
        public static SkillEventSystem Instance => instance ??= new SkillEventSystem();

        private Dictionary<string, List<Action<object>>> eventHandlers = new Dictionary<string, List<Action<object>>>();

        public void Subscribe(string eventName, Action<object> handler)
        {
            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = new List<Action<object>>();
            }
            eventHandlers[eventName].Add(handler);
        }

        public void Unsubscribe(string eventName, Action<object> handler)
        {
            if (eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName].Remove(handler);
            }
        }

        public void Publish(string eventName, object eventData)
        {
            if (eventHandlers.ContainsKey(eventName))
            {
                foreach (var handler in eventHandlers[eventName])
                {
                    handler.Invoke(eventData);
                }
            }
        }

        // 预定义的事件名称
        public static class EventNames
        {
            public const string SkillStart = "SkillStart";
            public const string SkillEnd = "SkillEnd";
            public const string SkillCooldown = "SkillCooldown";
            public const string SkillStateChange = "SkillStateChange";
            public const string EffectCreated = "EffectCreated";
            public const string EffectRemoved = "EffectRemoved";
            public const string EffectStateChanged = "EffectStateChanged";
        }
    }
} 