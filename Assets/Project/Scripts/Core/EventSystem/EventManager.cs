using UnityEngine;
using System;
using System.Collections.Generic;
using Core.Singleton;

namespace Core.EventSystem
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<Type, List<Delegate>> eventDictionary = new Dictionary<Type, List<Delegate>>();
        private readonly object _eventLock = new object();

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
            }
        }

        public void RemoveListener<T>(Action<T> listener)
        {
            lock (_eventLock)
            {
                Type eventType = typeof(T);
                if (eventDictionary.ContainsKey(eventType))
                {
                    eventDictionary[eventType].Remove(listener);
                }
            }
        }

        public void TriggerEvent<T>(T eventData)
        {
            List<Delegate> listeners = null;

            lock (_eventLock)
            {
                Type eventType = typeof(T);
                if (eventDictionary.ContainsKey(eventType))
                {
                    listeners = new List<Delegate>(eventDictionary[eventType]);
                }
            }

            if (listeners == null || listeners.Count == 0)
            {
                return;
            }

            foreach (var listener in listeners)
            {
                var handler = (Action<T>)listener;
                handler.Invoke(eventData);
            }
        }

        public void StartListening(string eventName, System.Action listener)
        {
            if (!eventDictionary.ContainsKey(typeof(System.Action)))
            {
                eventDictionary[typeof(System.Action)] = new List<Delegate>();
            }
            eventDictionary[typeof(System.Action)].Add(listener);
        }

        public bool HasListeners(string eventName)
        {
            return eventDictionary.ContainsKey(typeof(System.Action)) && 
                   eventDictionary[typeof(System.Action)].Count > 0;
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            lock (_eventLock)
            {
                eventDictionary.Clear();
            }
        }

        public void ClearAllListeners()
        {
            lock (_eventLock)
            {
                eventDictionary.Clear();
            }
        }
    }

    public class GameStateChangedEvent
    {
        public GameState NewState { get; private set; }

        public GameStateChangedEvent(GameState newState)
        {
            NewState = newState;
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
}
