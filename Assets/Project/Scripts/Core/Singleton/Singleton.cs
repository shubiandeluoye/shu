using UnityEngine;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine.Profiling;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private bool _isMarkedUnique;

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
                                singletonObject.name = typeof(T).ToString() + " (Singleton)";
                            }
                            (_instance as Singleton<T>)?.InitializeInstance();
                        }
                    }
                }
                return _instance;
            }
        }

        private void InitializeInstance()
        {
            var methodStartTime = DateTime.Now;
            Debug.Log($"\n[Singleton] [{methodStartTime:HH:mm:ss.fff}] {gameObject.name} 开始初始化");
            Debug.Log($"[Singleton] 线程信息: ID={Thread.CurrentThread.ManagedThreadId}, 是否主线程={Thread.CurrentThread.IsBackground}");
            Debug.Log($"[Singleton] 当前锁状态: {System.Threading.Monitor.IsEntered(_lock)}");
            
            lock (_lock)
            {
                var lockAcquireTime = DateTime.Now;
                Debug.Log($"[Singleton] [{lockAcquireTime:HH:mm:ss.fff}] 获取锁, 耗时: {(lockAcquireTime - methodStartTime).TotalMilliseconds:F2}ms");
                
                try
                {
                    LogCurrentState("进入锁区域后");
                    
                    if (_instance == null)
                    {
                        HandleNullInstance();
                    }
                    else if (_instance != this)
                    {
                        HandleDuplicateInstance();
                    }
                    else
                    {
                        Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 当前对象已经是实例: {gameObject.name}");
                    }
                    
                    LogCurrentState("初始化完成后");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 初始化过程中发生异常:");
                    Debug.LogError($"异常类型: {ex.GetType().FullName}");
                    Debug.LogError($"异常消息: {ex.Message}");
                    Debug.LogError($"堆栈跟踪:\n{ex.StackTrace}");
                    throw;
                }
                finally
                {
                    var endTime = DateTime.Now;
                    Debug.Log($"[Singleton] [{endTime:HH:mm:ss.fff}] 退出锁区域");
                    Debug.Log($"[Singleton] 总耗时: {(endTime - methodStartTime).TotalMilliseconds:F2}ms");
                }
            }
        }

        private void HandleNullInstance()
        {
            Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 没有现有实例，设置当前对象");
            LogGameObjectDetails(gameObject, "新实例");
            
            _instance = this as T;
            MarkAsUnique();
            SetDontDestroyOnLoad();
            
            Debug.Log($"[Singleton] 首次实例化完成: {gameObject.name}, IsUnique: {_isMarkedUnique}");
        }

        protected virtual void HandleDuplicateInstance()
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log($"[Singleton] 发现重复实例，保留第一个实例");
                DestroyImmediate(gameObject);
            }
        }

        private void LogCurrentState(string context)
        {
            Debug.Log($"\n[Singleton] --- 当前状态 [{context}] ---");
            Debug.Log($"当前时间: {DateTime.Now:HH:mm:ss.fff}");
            Debug.Log($"_instance: {(_instance != null ? _instance.name : "null")}");
            Debug.Log($"this: {gameObject.name}");
            Debug.Log($"IsMarkedUnique: {_isMarkedUnique}");
            if (_instance != null)
            {
                LogGameObjectDetails(_instance.gameObject, "当前实例");
            }
        }

        private void LogGameObjectDetails(GameObject go, string context)
        {
            Debug.Log($"\n[Singleton] --- GameObject详情 [{context}] ---");
            Debug.Log($"名称: {go.name}");
            Debug.Log($"实例ID: {go.GetInstanceID()}");
            Debug.Log($"是否激活: {go.activeInHierarchy}");
            Debug.Log($"层级: {go.layer}");
            Debug.Log($"位置: {go.transform.position}");
            Debug.Log($"是否是预制体实例: {PrefabUtility.IsPartOfPrefabInstance(go)}");
            Debug.Log($"完整路径: {GetGameObjectPath(go)}");
            Debug.Log($"组件数量: {go.GetComponents<Component>().Length}");
        }

        private string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }

        private void MarkAsUnique()
        {
            _isMarkedUnique = true;
        }

        private void SetDontDestroyOnLoad()
        {
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnApplicationQuit()
        {
            // 基类的默认实现
        }

        protected virtual void Awake()
        {
            // 基类的默认实现
        }

        protected virtual void OnAwake()
        {
            // 子类可以重写这个方法来添加自己的初始化逻辑
        }

        #region 异步操作管理
        private readonly Dictionary<string, CancellationTokenSource> _operationTokens = new Dictionary<string, CancellationTokenSource>();

        protected async Task ExecuteAsyncOperation(Func<CancellationToken, Task> operation, string operationKey = null)
        {
            LogCurrentState($"开始异步操作: {operationKey ?? "未命名"}");
            
            if (string.IsNullOrEmpty(operationKey))
            {
                operationKey = Guid.NewGuid().ToString();
            }

            try
            {
                CancelOperation(operationKey); // 如果存在相同key的操作，先取消
                var cts = new CancellationTokenSource();
                _operationTokens[operationKey] = cts;

                await operation(cts.Token);
                
                LogCurrentState($"异步操作完成: {operationKey}");
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 操作被取消: {operationKey}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 异步操作异常: {operationKey}");
                Debug.LogError($"异常类型: {ex.GetType().FullName}");
                Debug.LogError($"异常消息: {ex.Message}");
                Debug.LogError($"堆栈跟踪:\n{ex.StackTrace}");
                throw;
            }
            finally
            {
                if (_operationTokens.ContainsKey(operationKey))
                {
                    _operationTokens.Remove(operationKey);
                }
            }
        }

        protected void CancelOperation(string operationKey)
        {
            if (_operationTokens.TryGetValue(operationKey, out var cts))
            {
                Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 取消操作: {operationKey}");
                cts.Cancel();
                _operationTokens.Remove(operationKey);
            }
        }

        protected void CancelAllOperations()
        {
            Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 取消所有异步操作");
            foreach (var cts in _operationTokens.Values)
            {
                cts.Cancel();
            }
            _operationTokens.Clear();
        }
        #endregion

        #region 标志位管理
        private int _flags;

        protected void SetFlag(string flagName)
        {
            _flags |= 1 << flagName.GetHashCode() % 32;
        }

        protected void ClearFlag(string flagName)
        {
            _flags &= ~(1 << flagName.GetHashCode() % 32);
        }

        protected bool HasFlag(string flagName)
        {
            return (_flags & (1 << flagName.GetHashCode() % 32)) != 0;
        }
        #endregion

        #region 性能监控扩展
        private float _lastPerformanceLogTime;
        private int _updateCount;
        private const float PERFORMANCE_LOG_INTERVAL = 5f;

        protected void StartPerformanceMonitoring()
        {
            _lastPerformanceLogTime = Time.realtimeSinceStartup;
            _updateCount = 0;
            SetFlag("PerformanceMonitoring");
        }

        protected virtual void StopPerformanceMonitoring()
        {
            ClearFlag("PerformanceMonitoring");
        }

        protected virtual void Update()
        {
            if (!HasFlag("PerformanceMonitoring")) return;

            _updateCount++;
            float currentTime = Time.realtimeSinceStartup;
            
            if (currentTime - _lastPerformanceLogTime >= PERFORMANCE_LOG_INTERVAL)
            {
                float fps = _updateCount / (currentTime - _lastPerformanceLogTime);
                LogPerformanceMetrics(fps);
                
                _updateCount = 0;
                _lastPerformanceLogTime = currentTime;
            }
        }

        private void LogPerformanceMetrics(float fps)
        {
            Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] {typeof(T).Name} 性能指标:");
            Debug.Log($"FPS: {fps:F2}");
            Debug.Log($"内存使用: {Profiler.GetTotalAllocatedMemoryLong() / 1048576}MB");
            Debug.Log($"活动异步操作: {_operationTokens.Count}");
        }
        #endregion

        #region 线程安全工具
        protected void ExecuteOnMainThread(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
            {
                action();
            }
            else
            {
                // 使用你现有的锁机制
                lock (_lock)
                {
                    Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 将操作转移到主线程执行");
                    action();
                }
            }
        }

        protected bool IsMainThread()
        {
            return Thread.CurrentThread.ManagedThreadId == 1;
        }
        #endregion

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }

    /// <summary>
    /// MonoBehaviour 单例基类
    /// 用于需要挂载到 GameObject 上的单例组件
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;

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
                                singletonObject.name = typeof(T).ToString() + " (MonoSingleton)";
                                
                                // 直接在这里调用初始化方法
                                var singleton = _instance as MonoSingleton<T>;
                                if (singleton != null)
                                {
                                    singleton.MarkAsUnique();
                                    singleton.SetDontDestroyOnLoad();
                                }
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        private void MarkAsUnique()
        {
            // _isMarkedUnique = true;  // 这个字段从未被使用
        }

        private void SetDontDestroyOnLoad()
        {
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Awake()
        {
            // 子类可以重写这个方法
        }

        protected virtual void OnDestroy()
        {
            _applicationIsQuitting = true;
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}