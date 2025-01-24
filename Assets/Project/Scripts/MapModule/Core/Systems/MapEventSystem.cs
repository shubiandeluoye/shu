using System;
using System.Collections.Generic;

namespace MapModule.Core.Systems
{
    public class MapEventSystem
    {
        private static MapEventSystem instance;
        public static MapEventSystem Instance => instance ??= new MapEventSystem();

        private readonly Dictionary<string, List<Action<object>>> eventHandlers = 
            new Dictionary<string, List<Action<object>>>();

        public void Subscribe(string eventName, Action<object> handler)
        {
            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = new List<Action<object>>();
            }
            eventHandlers[eventName].Add(handler);
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

        public void Clear()
        {
            eventHandlers.Clear();
        }
    }
} 