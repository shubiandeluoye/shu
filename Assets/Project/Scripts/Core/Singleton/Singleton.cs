using UnityEngine;

/// <summary>
/// Generic Singleton base class for MonoBehaviour components
/// Implements thread-safe initialization and scene persistence
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object _lock = new object();
    private static T _instance;
    private static bool _quitting = false;

    public static T Instance
    {
        get
        {
            if (_quitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (_instance == null)
                    {
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"[Singleton] {typeof(T)}";

                        DontDestroyOnLoad(singletonObject);

                        Debug.Log($"[Singleton] Created instance of {typeof(T)}");
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[Singleton] Multiple instances of {typeof(T)} detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnApplicationQuit()
    {
        _quitting = true;
    }
}
