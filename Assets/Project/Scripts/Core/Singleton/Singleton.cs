using UnityEngine;
using System.Collections;
using System;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core
{
    /// <summary>
    /// 单例基类
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object _lock = new object();
        private static T _instance;
        private static bool _isInitializing = false;  // 添加初始化标志

        /// <summary>
        /// 获取单例实例
        /// </summary>
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
                                var go = new GameObject($"[{typeof(T).Name}]");
                                _isInitializing = true;  // 标记正在初始化
                                _instance = go.AddComponent<T>();
                                _isInitializing = false;  // 初始化完成
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取当前实例（不创建新实例）
        /// </summary>
        public static T CurrentInstance => _instance;

        protected virtual void Awake()
        {
            Debug.Log($"[Singleton] {Time.time} Awake called on {gameObject.name}, IsInitializing: {_isInitializing}");
            
            lock (_lock)  // 使用同一个锁确保线程安全
            {
                if (_isInitializing)
                {
                    // 如果是通过Instance属性创建的，直接返回
                    Debug.Log($"[Singleton] {Time.time} {gameObject.name} is being initialized through Instance property");
                    return;
                }

                if (_instance == null)
                {
                    // 如果没有实例，将自己设置为实例
                    Debug.Log($"[Singleton] {Time.time} Setting {gameObject.name} as first instance");
                    _instance = this as T;
                }
                else if (_instance != this)
                {
                    // 如果已经有实例，且不是自己，则销毁自己
                    Debug.Log($"[Singleton] {Time.time} Destroying duplicate {gameObject.name}, current instance: {_instance.name}");
                    #if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        DestroyImmediate(gameObject);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                    #else
                    Destroy(gameObject);
                    #endif
                }
            }
        }

        /// <summary>
        /// 对象销毁时的处理
        /// </summary>
        protected virtual void OnDestroy()
        {
            lock (_lock)
            {
                if (_instance == this)
                {
                    Debug.Log($"[Singleton] {Time.time} Clearing instance reference from {gameObject.name}");
                    _instance = null;
                }
            }
        }

        /// <summary>
        /// 重置单例实例
        /// </summary>
        public static void ResetInstance()
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    Debug.Log($"[Singleton] {Time.time} Resetting instance: {_instance.name}");
                    #if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        DestroyImmediate(_instance.gameObject);
                    }
                    else
                    {
                        Destroy(_instance.gameObject);
                    }
                    #endif
                    _instance = null;
                }
                _isInitializing = false;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            Debug.Log($"[Singleton] {Time.time} OnApplicationQuit called on {gameObject.name}");
            lock (_lock)
            {
                if (_instance == this)
                {
                    _instance = null;
                }
            }
        }
    }
}
