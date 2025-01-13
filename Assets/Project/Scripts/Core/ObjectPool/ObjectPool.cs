using UnityEngine;
using System.Collections.Generic;
using Core;

namespace Core.ObjectPool
{
    public class ObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string poolId;
            public GameObject prefab;
            public int size;
        }

        [SerializeField]
        private List<Pool> pools = new List<Pool>();
        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<string, GameObject> prefabDictionary;
        private Dictionary<string, Transform> poolParents;
        private Dictionary<string, int> maxPoolSizes;
        private Dictionary<string, int> activeObjectCounts;

        private void Awake()
        {
            InitializeDictionaries();
        }

        private void InitializeDictionaries()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            prefabDictionary = new Dictionary<string, GameObject>();
            poolParents = new Dictionary<string, Transform>();
            maxPoolSizes = new Dictionary<string, int>();
            activeObjectCounts = new Dictionary<string, int>();
        }

        private void Start()
        {
            if (pools != null)
            {
                foreach (Pool pool in pools)
                {
                    if (pool != null && pool.prefab != null)
                    {
                        CreatePool(pool.poolId, pool.prefab, pool.size);
                    }
                }
            }
        }

        public void CreatePool(string poolId, GameObject prefab, int initialSize, int maxSize = -1)
        {
            if (prefab == null)
            {
                Debug.LogError($"[ObjectPool] Prefab for pool {poolId} is null");
                return;
            }

            GameObject poolParent = new GameObject($"Pool_{poolId}");
            poolParent.transform.SetParent(transform);
            poolParents[poolId] = poolParent.transform;

            Queue<GameObject> objectPool = new Queue<GameObject>();
            prefabDictionary[poolId] = prefab;

            maxPoolSizes[poolId] = maxSize;
            activeObjectCounts[poolId] = 0;

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = CreateNewObject(poolId, prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary[poolId] = objectPool;
            Debug.Log($"[ObjectPool] Created pool {poolId} with initial size {initialSize}, max size {maxSize}");
        }

        private GameObject CreateNewObject(string poolId, GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, poolParents[poolId]);
            obj.SetActive(false);
            
            var poolObject = obj.AddComponent<PoolObject>();
            poolObject.Initialize(poolId, this);
            
            return obj;
        }

        public GameObject SpawnFromPool(string poolId, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(poolId))
            {
                Debug.LogWarning($"[ObjectPool] Pool {poolId} does not exist");
                return null;
            }

            Queue<GameObject> pool = poolDictionary[poolId];

            if (pool.Count == 0)
            {
                int maxSize = maxPoolSizes.ContainsKey(poolId) ? maxPoolSizes[poolId] : -1;
                int currentActive = activeObjectCounts.ContainsKey(poolId) ? activeObjectCounts[poolId] : 0;
                
                if (maxSize == -1 || currentActive < maxSize)
                {
                    GameObject newObj = CreateNewObject(poolId, prefabDictionary[poolId]);
                    pool.Enqueue(newObj);
                    Debug.Log($"[ObjectPool] Pool {poolId} auto-expanded, active objects: {currentActive + 1}");
                }
                else
                {
                    Debug.LogWarning($"[ObjectPool] Pool {poolId} reached max size {maxSize}, waiting for objects to be returned");
                    return null;
                }
            }

            GameObject objectToSpawn = pool.Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            
            if (activeObjectCounts.ContainsKey(poolId))
                activeObjectCounts[poolId]++;

            return objectToSpawn;
        }

        public void ReturnToPool(string poolId, GameObject objectToReturn)
        {
            if (!poolDictionary.ContainsKey(poolId))
            {
                Debug.LogWarning($"[ObjectPool] Pool {poolId} does not exist");
                return;
            }

            objectToReturn.SetActive(false);
            poolDictionary[poolId].Enqueue(objectToReturn);
            
            if (activeObjectCounts.ContainsKey(poolId))
                activeObjectCounts[poolId] = Mathf.Max(0, activeObjectCounts[poolId] - 1);
        }
    }

    public class PoolObject : MonoBehaviour
    {
        private string poolId;
        private ObjectPool pool;
        private float? autoRecycleTime;

        public void Initialize(string poolId, ObjectPool objectPool, float? recycleTime = null)
        {
            this.poolId = poolId;
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
            RecycleNow();
        }

        public void RecycleNow()
        {
            if (gameObject.activeInHierarchy && pool != null)
            {
                pool.ReturnToPool(poolId, gameObject);
            }
        }
    }
}
