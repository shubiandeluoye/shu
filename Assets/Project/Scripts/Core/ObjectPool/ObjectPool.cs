using UnityEngine;
using System.Collections.Generic;
using Core;

namespace Core.ObjectPool
{
    /// <summary>
    /// 通用对象池实现
    /// 支持自动扩展和对象回收功能
    /// </summary>
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
            Debug.Log("对象池Awake被调用");
            InitializeDictionaries();
        }

        private void InitializeDictionaries()
        {
            Debug.Log("初始化字典...");
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            prefabDictionary = new Dictionary<string, GameObject>();
            poolParents = new Dictionary<string, Transform>();
            Debug.Log("字典初始化完成");
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

        /// <summary>
        /// 创建一个新的对象池
        /// </summary>
        /// <param name="poolId">对象池ID</param>
        /// <param name="prefab">预制体</param>
        /// <param name="size">初始大小</param>
        public void CreatePool(string poolId, GameObject prefab, int size)
        {
            Debug.Log($"开始创建对象池: {poolId}, 大小: {size}");
            
            // 确保字典已初始化
            if (poolParents == null)
            {
                Debug.Log("字典未初始化，正在初始化...");
                InitializeDictionaries();
            }

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
            Debug.Log($"对象池 {poolId} 创建完成，大小: {size}");
        }

        /// <summary>
        /// 创建新的对象实例
        /// </summary>
        private GameObject CreateNewObject(string poolId, GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, poolParents[poolId]);
            obj.SetActive(false);
            
            // 添加PoolObject组件用于自动回收
            var poolObject = obj.AddComponent<PoolObject>();
            poolObject.Initialize(poolId, this);
            
            return obj;
        }

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <param name="poolId">对象池ID</param>
        /// <param name="position">生成位置</param>
        /// <param name="rotation">生成旋转</param>
        public GameObject SpawnFromPool(string poolId, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(poolId))
            {
                Debug.LogWarning($"[对象池] 标签为 {poolId} 的对象池不存在。");
                return null;
            }

            Queue<GameObject> pool = poolDictionary[poolId];

            // 如果池为空则自动扩展
            if (pool.Count == 0)
            {
                GameObject newObj = CreateNewObject(poolId, prefabDictionary[poolId]);
                pool.Enqueue(newObj);
                Debug.Log($"[对象池] {poolId} 已自动扩展");
            }

            GameObject objectToSpawn = pool.Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            return objectToSpawn;
        }

        /// <summary>
        /// 将对象返回到对象池
        /// </summary>
        public void ReturnToPool(string poolId, GameObject objectToReturn)
        {
            if (!poolDictionary.ContainsKey(poolId))
            {
                Debug.LogWarning($"[对象池] 标签为 {poolId} 的对象池不存在。");
                return;
            }

            objectToReturn.SetActive(false);
            poolDictionary[poolId].Enqueue(objectToReturn);
        }
    }

    /// <summary>
    /// 池化对象组件
    /// 用于处理对象的自动回收
    /// </summary>
    public class PoolObject : MonoBehaviour
    {
        private string poolId;
        private ObjectPool pool;
        private float? autoRecycleTime;

        /// <summary>
        /// 初始化池化对象
        /// </summary>
        /// <param name="poolId">对象池ID</param>
        /// <param name="objectPool">对象池引用</param>
        /// <param name="recycleTime">自动回收时间（可选）</param>
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

        // 添加直接回收方法，用于测试
        public void RecycleNow()
        {
            if (gameObject.activeInHierarchy && pool != null)
            {
                pool.ReturnToPool(poolId, gameObject);
            }
        }
    }
}
