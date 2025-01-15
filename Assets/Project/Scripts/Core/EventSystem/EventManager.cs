using UnityEngine;
using System;
using System.Collections.Generic;
using Core;
using Core.FSM;

namespace Core.EventSystem
{
    /// <summary>
    /// 事件系统实现
    /// 支持类型安全的事件注册和参数传递
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        /// <summary>
        /// 存储事件及其监听器的字典
        /// </summary>
        private Dictionary<Type, List<Delegate>> eventDictionary = new Dictionary<Type, List<Delegate>>();
        private readonly object _eventLock = new object();

        /// <summary>
        /// 为指定事件类型注册事件处理器
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="listener">事件处理方法</param>
        public void AddListener<T>(Action<T> listener)
        {
            lock (_eventLock)
            {
                Type eventType = typeof(T);
                if (!eventDictionary.ContainsKey(eventType))
                {
                    eventDictionary[eventType] = new List<Delegate>();
                }
                eventDictionary[eventType].Add(listener);
                Debug.Log($"[EventManager] 添加监听器 {eventType.Name}, 当前监听器数量: {eventDictionary[eventType].Count}");
            }
        }

        /// <summary>
        /// 移除指定事件类型的事件处理器
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="listener">要移除的事件处理方法</param>
        public void RemoveListener<T>(Action<T> listener)
        {
            lock (_eventLock)
            {
                Type eventType = typeof(T);
                if (eventDictionary.ContainsKey(eventType))
                {
                    eventDictionary[eventType].Remove(listener);
                    Debug.Log($"[EventManager] 移除监听器 {eventType.Name}, 剩余监听器数量: {eventDictionary[eventType].Count}");
                }
            }
        }

        /// <summary>
        /// 触发指定类型的事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="eventData">事件数据</param>
        public void TriggerEvent<T>(T eventData)
        {
            List<Exception> exceptions = new List<Exception>();
            List<Delegate> listeners = null;

            // 获取监听器列表的副本
            lock (_eventLock)
            {
                Type eventType = typeof(T);
                if (eventDictionary.ContainsKey(eventType))
                {
                    listeners = new List<Delegate>(eventDictionary[eventType]);
                }
            }

            // 如果没有监听器，直接返回
            if (listeners == null || listeners.Count == 0)
            {
                Debug.Log($"[EventManager] 没有找到事件 {typeof(T).Name} 的监听器");
                return;
            }

            Debug.Log($"[EventManager] 开始触发事件 {typeof(T).Name}, 监听器数量: {listeners.Count}");

            // 遍历并调用所有监听器
            foreach (var listener in listeners)
            {
                try
                {
                    var handler = (Action<T>)listener;
                    handler.Invoke(eventData);
                    Debug.Log($"[EventManager] 成功执行监听器 {listener.Method.Name}");
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Debug.LogError($"[事件系统] 触发事件 {typeof(T).Name} 时发生错误: {ex}");
                }
            }

            // 如果有异常，抛出聚合异常
            if (exceptions.Count > 0)
            {
                if (exceptions.Count == 1)
                {
                    Debug.LogError($"[EventManager] 事件处理过程中发生单个异常");
                    throw exceptions[0];
                }
                else
                {
                    Debug.LogError($"[EventManager] 事件处理过程中发生多个异常 ({exceptions.Count})");
                    throw new AggregateException($"事件 {typeof(T).Name} 处理过程中发生多个错误", exceptions);
                }
            }
        }

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="listener">监听器委托</param>
        public void StartListening(string eventName, System.Action listener)
        {
            Debug.Log($"[EventManager] 添加监听器: {eventName}");
            
            if (!eventDictionary.ContainsKey(typeof(System.Action)))
            {
                eventDictionary[typeof(System.Action)] = new List<Delegate>();
            }
            
            eventDictionary[typeof(System.Action)].Add(listener);
            Debug.Log($"[EventManager] 监听器已添加: {eventName}, 当前监听器数量: {eventDictionary[typeof(System.Action)].Count}");
        }

        /// <summary>
        /// 检查指定事件是否有监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <returns>如果有监听器则返回true</returns>
        public bool HasListeners(string eventName)
        {
            return eventDictionary.ContainsKey(typeof(System.Action)) && eventDictionary[typeof(System.Action)].Count > 0;
        }

        /// <summary>
        /// 在应用退出时执行清理
        /// </summary>
        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            lock (_eventLock)
            {
                Debug.Log("[EventManager] 应用退出，清理所有事件监听器");
                eventDictionary.Clear();
            }
        }

        /// <summary>
        /// 清理所有事件监听器
        /// </summary>
        public void ClearAllListeners()
        {
            lock (_eventLock)
            {
                Debug.Log("[EventManager] 清理所有事件监听器");
                eventDictionary.Clear();
            }
        }

        /// <summary>
        /// 获取指定事件类型的监听器数量
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>监听器数量</returns>
        public int GetListenerCount<T>()
        {
            lock (_eventLock)
            {
                Type eventType = typeof(T);
                if (eventDictionary.ContainsKey(eventType))
                {
                    return eventDictionary[eventType].Count;
                }
                return 0;
            }
        }

        /// <summary>
        /// 清理事件
        /// </summary>
        public void Cleanup()
        {
            eventDictionary.Clear();
        }
    }
}
