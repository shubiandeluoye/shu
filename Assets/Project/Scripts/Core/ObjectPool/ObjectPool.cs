using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Core.ObjectPool
{
    public class ObjectPool : MonoBehaviour
    {
        private static ObjectPool instance;
        public static ObjectPool Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("ObjectPool");
                    instance = go.AddComponent<ObjectPool>();
                }
                return instance;
            }
        }

        private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<GameObject, float> lifetimes = new Dictionary<GameObject, float>();

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        public GameObject GetObject(GameObject prefab, float lifetime = 0)
        {
            string key = prefab.name;
            if (!pools.ContainsKey(key))
            {
                pools[key] = new Queue<GameObject>();
            }

            GameObject obj;
            if (pools[key].Count == 0)
            {
                obj = Instantiate(prefab, transform);
                obj.name = prefab.name;
            }
            else
            {
                obj = pools[key].Dequeue();
            }
            
            obj.SetActive(true);
            
            if (lifetime > 0)
            {
                lifetimes[obj] = Time.time + lifetime;
                StartCoroutine(AutoReturn(obj, lifetime));
            }
            
            return obj;
        }

        public void ReturnObject(GameObject obj)
        {
            string key = obj.name.Replace("(Clone)", "").Trim();
            if (pools.ContainsKey(key))
            {
                obj.SetActive(false);
                pools[key].Enqueue(obj);
                lifetimes.Remove(obj);
            }
        }

        private IEnumerator AutoReturn(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (obj != null && obj.activeInHierarchy)
            {
                ReturnObject(obj);
            }
        }

        private void Update()
        {
            var time = Time.time;
            foreach (var kvp in lifetimes.ToArray())
            {
                if (time >= kvp.Value)
                {
                    ReturnObject(kvp.Key);
                }
            }
        }
    }
}
