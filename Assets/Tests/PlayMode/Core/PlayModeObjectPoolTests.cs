using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Core.ObjectPool;

namespace Tests.PlayMode.Core
{
    public class PlayModeObjectPoolTests
    {
        private ObjectPool _objectPool;
        private GameObject _testPrefab;
        private const string TEST_POOL_ID = "TestPool";

        [UnitySetUp]
        public IEnumerator Setup()
        {
            Debug.Log("开始设置PlayMode测试环境...");
            
            var poolGO = new GameObject("ObjectPool");
            _objectPool = poolGO.AddComponent<ObjectPool>();
            Debug.Log($"对象池组件已创建: {_objectPool != null}");
            
            _testPrefab = new GameObject("TestPrefab");
            Debug.Log($"测试预制体已创建: {_testPrefab != null}");

            yield return null;
        }

        [UnityTest]
        public IEnumerator PoolObject_AutoRecycle_DelayedReturn()
        {
            Debug.Log("开始自动回收延迟测试...");
            
            // Arrange
            _objectPool.CreatePool(TEST_POOL_ID, _testPrefab, 1);
            yield return new WaitForSeconds(0.1f);
            
            var spawnedObject = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.zero, Quaternion.identity);
            Assert.That(spawnedObject, Is.Not.Null, "生成的对象不应为空");

            var poolObject = spawnedObject.GetComponent<PoolObject>();
            Assert.That(poolObject, Is.Not.Null, "对象应该有PoolObject组件");
            
            // Act
            Debug.Log($"初始化PoolObject，设置自动回收时间为0.5秒，当前状态: {spawnedObject.activeInHierarchy}");
            poolObject.Initialize(TEST_POOL_ID, _objectPool, 0.5f);
            Assert.That(spawnedObject.activeInHierarchy, Is.True, "对象初始应该处于激活状态");

            // 等待足够的时间让自动回收发生
            Debug.Log("等待自动回收...");
            yield return new WaitForSeconds(1f);

            // Assert
            Debug.Log($"最终检查对象状态: 激活={spawnedObject.activeInHierarchy}");
            Assert.That(spawnedObject.activeInHierarchy, Is.False, "对象应该在延迟后被禁用");
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            Debug.Log("清理PlayMode测试环境...");
            if (_objectPool != null)
            {
                Object.Destroy(_objectPool.gameObject);
                Debug.Log("对象池已销毁");
            }
            if (_testPrefab != null)
            {
                Object.Destroy(_testPrefab);
                Debug.Log("测试预制体已销毁");
            }
            yield return null;
        }
    }
} 