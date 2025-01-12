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

        private void Awake()
        {
            InitializeDictionaries();
        }

        private void InitializeDictionaries()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            prefabDictionary = new Dictionary<string, GameObject>();
            poolParents = new Dictionary<string, Transform>();
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

        public void CreatePool(string poolId, GameObject prefab, int size)
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

            for (int i = 0; i < size; i++)
            {
                GameObject obj = CreateNewObject(poolId, prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary[poolId] = objectPool;
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
                GameObject newObj = CreateNewObject(poolId, prefabDictionary[poolId]);
                pool.Enqueue(newObj);
            }

            GameObject objectToSpawn = pool.Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

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
