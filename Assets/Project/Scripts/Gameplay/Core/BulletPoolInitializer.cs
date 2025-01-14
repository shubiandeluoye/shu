using UnityEngine;
using Core.ObjectPool;

namespace Gameplay.Core
{
    public class BulletPoolInitializer : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int initialPoolSize = 20;

        private void Start()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            // 预先创建对象
            for (int i = 0; i < initialPoolSize; i++)
            {
                var obj = ObjectPool.Instance.GetObject(bulletPrefab);
                ObjectPool.Instance.ReturnObject(obj);
            }
        }
    }
}
