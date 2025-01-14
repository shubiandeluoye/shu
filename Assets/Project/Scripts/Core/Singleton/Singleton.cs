using UnityEngine;
using System;
using System.Threading;

namespace Core.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = FindObjectOfType<T>();
                            if (_instance == null)
                            {
                                GameObject singletonObject = new GameObject();
                                _instance = singletonObject.AddComponent<T>();
                                singletonObject.name = $"[{typeof(T).Name}]";
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            InitializeInstance();
        }

        private void InitializeInstance()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (_instance != null)
            {
                DestroyImmediate(_instance.gameObject);
            }
            _instance = null;
        }

        public static void ResetInstance()
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    DestroyImmediate(_instance.gameObject);
                }
                _instance = null;
            }
        }
    }
}