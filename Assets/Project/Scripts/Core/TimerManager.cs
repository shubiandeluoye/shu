using UnityEngine;
using System;
using System.Collections.Generic;

namespace Core.Timer
{
    public class TimerManager : MonoBehaviour
    {
        private static TimerManager instance;
        public static TimerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("TimerManager");
                    instance = go.AddComponent<TimerManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private class Timer
        {
            public int Id;
            public float RemainingTime;
            public Action Callback;
            public bool IsActive;
        }

        private List<Timer> activeTimers = new List<Timer>();
        private int nextTimerId = 1;

        public int AddTimer(float duration, Action callback)
        {
            var timer = new Timer
            {
                Id = nextTimerId++,
                RemainingTime = duration,
                Callback = callback,
                IsActive = true
            };
            
            activeTimers.Add(timer);
            return timer.Id;
        }

        public void RemoveTimer(int timerId)
        {
            for (int i = activeTimers.Count - 1; i >= 0; i--)
            {
                if (activeTimers[i].Id == timerId)
                {
                    activeTimers[i].IsActive = false;
                    break;
                }
            }
        }

        private void Update()
        {
            for (int i = activeTimers.Count - 1; i >= 0; i--)
            {
                var timer = activeTimers[i];
                if (!timer.IsActive)
                {
                    activeTimers.RemoveAt(i);
                    continue;
                }

                timer.RemainingTime -= Time.deltaTime;
                if (timer.RemainingTime <= 0)
                {
                    timer.Callback?.Invoke();
                    timer.IsActive = false;
                }
            }
        }
    }
} 