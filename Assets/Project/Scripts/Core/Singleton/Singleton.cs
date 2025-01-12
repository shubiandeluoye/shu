using UnityEngine;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine.Profiling;

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
                        _instance.InitializeInstance();
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

    private void HandleDuplicateInstance()
    {
        Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 发现重复实例");
        LogGameObjectDetails(_instance.gameObject, "现有实例");
        LogGameObjectDetails(gameObject, "新实例");
        
        var oldInstance = _instance;
        Debug.Log($"[Singleton] 保存旧实例引用: {oldInstance.name}");
        
        _instance = this as T;
        MarkAsUnique();
        SetDontDestroyOnLoad();
        
        if (oldInstance != null)
        {
            var oldGameObject = oldInstance.gameObject;
            if (oldGameObject != null)
            {
                Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 准备销毁旧实例");
                LogGameObjectDetails(oldGameObject, "待销毁实例");
                DestroyImmediate(oldGameObject);
                Debug.Log($"[Singleton] [{DateTime.Now:HH:mm:ss.fff}] 旧实例已销毁");
            }
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
}