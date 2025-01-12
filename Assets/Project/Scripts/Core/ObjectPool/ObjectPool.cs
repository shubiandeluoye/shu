using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generic object pool implementation with auto-expansion and recycling capabilities
/// </summary>
public class ObjectPool : Singleton<ObjectPool>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField]
    private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary;
    private Dictionary<string, Transform> poolParents;

    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();
        poolParents = new Dictionary<string, Transform>();
    }

    private void Start()
    {
        foreach (Pool pool in pools)
        {
            CreatePool(pool.tag, pool.prefab, pool.size);
        }
    }

    public void CreatePool(string tag, GameObject prefab, int size)
    {
        GameObject poolParent = new GameObject($"Pool_{tag}");
        poolParent.transform.SetParent(transform);
        poolParents[tag] = poolParent.transform;

        Queue<GameObject> objectPool = new Queue<GameObject>();
        prefabDictionary[tag] = prefab;

        for (int i = 0; i < size; i++)
        {
            GameObject obj = CreateNewObject(tag, prefab);
            objectPool.Enqueue(obj);
        }

        poolDictionary[tag] = objectPool;
    }

    private GameObject CreateNewObject(string tag, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, poolParents[tag]);
        obj.SetActive(false);
        
        // Add PoolObject component to handle automatic recycling
        var poolObject = obj.AddComponent<PoolObject>();
        poolObject.Initialize(tag, this);
        
        return obj;
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];

        // Auto-expand pool if empty
        if (pool.Count == 0)
        {
            GameObject newObj = CreateNewObject(tag, prefabDictionary[tag]);
            pool.Enqueue(newObj);
            Debug.Log($"Pool {tag} auto-expanded");
        }

        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}

/// <summary>
/// Component added to pooled objects to handle automatic recycling
/// </summary>
public class PoolObject : MonoBehaviour
{
    private string poolTag;
    private ObjectPool pool;
    private float? autoRecycleTime;

    public void Initialize(string tag, ObjectPool objectPool, float? recycleTime = null)
    {
        poolTag = tag;
        pool = objectPool;
        autoRecycleTime = recycleTime;

        if (autoRecycleTime.HasValue)
        {
            StartCoroutine(AutoRecycle());
        }
    }

    private System.Collections.IEnumerator AutoRecycle()
    {
        yield return new WaitForSeconds(autoRecycleTime.Value);
        if (gameObject.activeInHierarchy)
        {
            pool.ReturnToPool(poolTag, gameObject);
        }
    }
}
