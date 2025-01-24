using System;
using System.Collections.Generic;

namespace PlayerModule.Core.Systems
{
    public class PlayerEventSystem
    {
        private static PlayerEventSystem instance;
        public static PlayerEventSystem Instance => instance ??= new PlayerEventSystem();

        private readonly Dictionary<string, List<Action<object>>> eventHandlers = new Dictionary<string, List<Action<object>>>();

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
                    try
                    {
                        handler.Invoke(eventData);
                    }
                    catch (Exception e)
                    {
                        // 在实际项目中应该使用日志系统
                        System.Diagnostics.Debug.WriteLine($"Error handling event {eventName}: {e.Message}");
                    }
                }
            }
        }

        public void Clear()
        {
            eventHandlers.Clear();
        }
    }
} 