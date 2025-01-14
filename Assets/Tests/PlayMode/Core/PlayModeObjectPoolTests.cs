using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Core.ObjectPool;

namespace Tests.PlayMode
{
    public class PlayModeObjectPoolTests
    {
        private ObjectPool _objectPool;
        private GameObject _testPrefab;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            Debug.Log("开始设置PlayMode测试环境...");
            
            var poolGO = new GameObject("ObjectPool");
            _objectPool = poolGO.AddComponent<ObjectPool>();
            
            _testPrefab = new GameObject("TestPrefab");
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator ObjectPool_GetAndReturn_WorksCorrectly()
        {
            var obj1 = _objectPool.GetObject(_testPrefab);
            Assert.That(obj1, Is.Not.Null, "应该能获取对象");
            Assert.That(obj1.activeInHierarchy, Is.True, "对象应该是激活的");

            _objectPool.ReturnObject(obj1);
            Assert.That(obj1.activeInHierarchy, Is.False, "返回的对象应该是禁用的");

            var obj2 = _objectPool.GetObject(_testPrefab);
            Assert.That(obj2, Is.EqualTo(obj1), "应该重用同一个对象");

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            if (_objectPool != null)
                Object.Destroy(_objectPool.gameObject);
            if (_testPrefab != null)
                Object.Destroy(_testPrefab);
            yield return null;
        }
    }
} 