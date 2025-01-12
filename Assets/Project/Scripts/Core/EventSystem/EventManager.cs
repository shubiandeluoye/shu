using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Event system implementation supporting type-safe event registration and parameter passing
/// </summary>
public class EventManager : Singleton<EventManager>
{
    // Dictionary to store event handlers with their corresponding event types
    private Dictionary<Type, List<Delegate>> eventHandlers = new Dictionary<Type, List<Delegate>>();

    // Register an event handler for a specific event type
    public void AddListener<T>(Action<T> handler)
    {
        Type eventType = typeof(T);

        if (!eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType] = new List<Delegate>();
        }

        if (!eventHandlers[eventType].Contains(handler))
        {
            eventHandlers[eventType].Add(handler);
        }
    }

    // Remove an event handler for a specific event type
    public void RemoveListener<T>(Action<T> handler)
    {
        Type eventType = typeof(T);

        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType].Remove(handler);

            if (eventHandlers[eventType].Count == 0)
            {
                eventHandlers.Remove(eventType);
            }
        }
    }

    // Trigger an event with optional parameters
    public void TriggerEvent<T>(T eventData)
    {
        Type eventType = typeof(T);

        if (eventHandlers.ContainsKey(eventType))
        {
            foreach (var handler in eventHandlers[eventType].ToArray())
            {
                try
                {
                    ((Action<T>)handler).Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventManager] Error triggering event {eventType}: {e}");
                }
            }
        }
    }

    // Clear all event handlers
    public void ClearAllListeners()
    {
        eventHandlers.Clear();
    }

    // Get the number of listeners for a specific event type
    public int GetListenerCount<T>()
    {
        Type eventType = typeof(T);
        return eventHandlers.ContainsKey(eventType) ? eventHandlers[eventType].Count : 0;
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        ClearAllListeners();
    }
}

// Example event data classes
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
