using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Diagnostics;
using Core.ObjectPool;
using Debug = UnityEngine.Debug;

namespace Tests.EditMode.Core
{
    [TestFixture]
    public class ObjectPoolTests
    {
        private ObjectPool _objectPool;
        private GameObject _testPrefab;
        private const string TEST_POOL_ID = "TestPool";

        [SetUp]
        public void Setup()
        {
            Debug.Log("开始设置测试环境...");
            
            var poolGO = new GameObject("ObjectPool");
            _objectPool = poolGO.AddComponent<ObjectPool>();
            Debug.Log($"对象池组件已创建: {_objectPool != null}");
            
            _testPrefab = new GameObject("TestPrefab");
            Debug.Log($"测试预制体已创建: {_testPrefab != null}");
            
            // 确保Awake被调用
            _objectPool.gameObject.SetActive(true);
        }

        [UnityTest]
        public IEnumerator CreatePool_WhenInitialized_CreatesCorrectNumberOfObjects()
        {
            Debug.Log("开始测试创建对象池...");
            
            // 等待一帧
            yield return null;
            
            // Arrange
            int poolSize = 5;
            Debug.Log($"计划创建池大小: {poolSize}");

            // Act
            Debug.Log("开始创建对象池...");
            _objectPool.CreatePool(TEST_POOL_ID, _testPrefab, poolSize);
            
            // 等待一帧
            yield return null;

            // Assert
            var poolParent = _objectPool.transform.Find($"Pool_{TEST_POOL_ID}");
            Debug.Log($"池父物体是否创建: {poolParent != null}");
            
            Assert.That(poolParent, Is.Not.Null, "对象池父物体应该被创建");
            
            Debug.Log($"实际创建的对象数量: {poolParent.childCount}");
            Assert.That(poolParent.childCount, Is.EqualTo(poolSize), 
                $"期望创建 {poolSize} 个对象，实际创建了 {poolParent.childCount} 个");
            
            // 检查每个对象的状态
            for (int i = 0; i < poolParent.childCount; i++)
            {
                yield return null; // 每个对象检查之间等待一帧
                var obj = poolParent.GetChild(i).gameObject;
                Debug.Log($"对象 {i}: 激活状态={obj.activeInHierarchy}, " +
                         $"PoolObject组件存在={obj.GetComponent<PoolObject>() != null}");
                
                Assert.That(obj.activeInHierarchy, Is.False, $"对象 {i} 应该处于非激活状态");
                Assert.That(obj.GetComponent<PoolObject>(), Is.Not.Null, $"对象 {i} 应该有PoolObject组件");
            }
        }

        [UnityTest]
        public IEnumerator SpawnFromPool_ShowsCompleteLifecycle()
        {
            Debug.Log("开始测试对象生命周期...");
            
            // 确保预制体和对象池都存在
            Assert.That(_testPrefab, Is.Not.Null, "测试预制体不应为空");
            Assert.That(_objectPool, Is.Not.Null, "对象池不应为空");
            
            yield return null;
            
            // 1. 创建池
            int initialSize = 2;
            Debug.Log($"创建大小为 {initialSize} 的对象池");
            _objectPool.CreatePool(TEST_POOL_ID, _testPrefab, initialSize);
            yield return null;

            // 验证池是否创建成功
            var poolParent = _objectPool.transform.Find($"Pool_{TEST_POOL_ID}");
            Assert.That(poolParent, Is.Not.Null, "对象池父物体应该被创建");
            Assert.That(poolParent.childCount, Is.EqualTo(initialSize), "初始对象数量不正确");

            // 2. 从池中获取对象
            var obj1 = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.zero, Quaternion.identity);
            Debug.Log($"第一个对象已获取: {obj1 != null}, 激活状态: {obj1?.activeInHierarchy}");
            yield return null;

            // 3. 获取第二个对象
            var obj2 = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.one, Quaternion.identity);
            Debug.Log($"第二个对象已获取: {obj2 != null}, 激活状态: {obj2?.activeInHierarchy}");
            yield return null;

            // 4. 尝试获取第三个对象（应该触发自动扩展）
            var obj3 = _objectPool.SpawnFromPool(TEST_POOL_ID, -Vector3.one, Quaternion.identity);
            Debug.Log($"第三个对象已获取（自动扩展）: {obj3 != null}, 激活状态: {obj3?.activeInHierarchy}");
            yield return null;

            // 5. 返回对象到池中
            Debug.Log("开始返回对象到池中...");
            _objectPool.ReturnToPool(TEST_POOL_ID, obj1);
            yield return null;
            Debug.Log($"对象1返回后的激活状态: {obj1.activeInHierarchy}");

            // 6. 重新使用返回的对象
            var reusedObj = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.zero, Quaternion.identity);
            yield return null;
            Debug.Log($"重用的对象是否为之前的对象1: {reusedObj == obj1}");
            
            // 验证
            Assert.That(obj1.activeInHierarchy, Is.True, "重用的对象应该被激活");
            Assert.That(reusedObj, Is.EqualTo(obj1), "应该重用之前返回的对象");
        }

        [Test]
        public void SpawnFromPool_WhenPoolEmpty_AutoExpandsUnderPerformanceLimit()
        {
            // Arrange
            _objectPool.CreatePool(TEST_POOL_ID, _testPrefab, 1);
            var firstObject = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.zero, Quaternion.identity);
            Assert.That(firstObject, Is.Not.Null, "First spawn should succeed");

            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            var newObject = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.zero, Quaternion.identity);
            stopwatch.Stop();

            // Assert
            Assert.That(newObject, Is.Not.Null, "Auto-expanded object should not be null");
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(16), "Pool expansion took longer than 16ms");
        }

        [Test]
        public void ReturnToPool_WhenObjectReturned_DeactivatesAndEnqueues()
        {
            // Arrange
            _objectPool.CreatePool(TEST_POOL_ID, _testPrefab, 1);
            var spawnedObject = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.zero, Quaternion.identity);
            Assert.That(spawnedObject, Is.Not.Null, "Initial spawn should succeed");

            // Act
            _objectPool.ReturnToPool(TEST_POOL_ID, spawnedObject);

            // Assert
            Assert.That(spawnedObject.activeInHierarchy, Is.False, "Object should be deactivated");
            var reusedObject = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.zero, Quaternion.identity);
            Assert.That(reusedObject, Is.EqualTo(spawnedObject), "Should reuse the returned object");
        }

        [Test]
        public void SpawnFromPool_WithInvalidTag_ReturnsNullAndLogsWarning()
        {
            // 确保字典已初始化
            _objectPool.CreatePool("DummyPool", _testPrefab, 1);
            
            LogAssert.Expect(LogType.Warning, $"[对象池] 标签为 InvalidTag 的对象池不存在。");

            // Act
            var result = _objectPool.SpawnFromPool("InvalidTag", Vector3.zero, Quaternion.identity);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ReturnToPool_WithInvalidTag_LogsWarning()
        {
            // 确保字典已初始化
            _objectPool.CreatePool("DummyPool", _testPrefab, 1);
            
            LogAssert.Expect(LogType.Warning, $"[对象池] 标签为 InvalidTag 的对象池不存在。");

            // Arrange
            var obj = new GameObject("TestObject");

            // Act
            _objectPool.ReturnToPool("InvalidTag", obj);

            // Cleanup
            Object.DestroyImmediate(obj);
        }

        [Test]
        public void PoolObject_AutoRecycle_Functionality()
        {
            Debug.Log("开始自动回收测试...");
            
            // Arrange
            _objectPool.CreatePool(TEST_POOL_ID, _testPrefab, 1);
            var spawnedObject = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.zero, Quaternion.identity);
            Assert.That(spawnedObject, Is.Not.Null, "生成的对象不应为空");

            var poolObject = spawnedObject.GetComponent<PoolObject>();
            Assert.That(poolObject, Is.Not.Null, "对象应该有PoolObject组件");
            
            // Act
            Debug.Log("初始化PoolObject并直接调用回收");
            poolObject.Initialize(TEST_POOL_ID, _objectPool);
            poolObject.RecycleNow();

            // Assert
            Debug.Log($"检查对象状态: 激活={spawnedObject.activeInHierarchy}");
            Assert.That(spawnedObject.activeInHierarchy, Is.False, "对象应该在回收后被禁用");
        }

        // 如果需要测试实际的延迟功能，应该在PlayMode测试中进行
        // [UnityTest]
        // public IEnumerator PoolObject_AutoRecycle_DelayedReturn()
        // {
        //     Debug.Log("开始自动回收延迟测试...");
        //     
        //     // Arrange
        //     _objectPool.CreatePool(TEST_POOL_ID, _testPrefab, 1);
        //     yield return null;
        //     
        //     var spawnedObject = _objectPool.SpawnFromPool(TEST_POOL_ID, Vector3.zero, Quaternion.identity);
        //     Assert.That(spawnedObject, Is.Not.Null, "生成的对象不应为空");
        //     yield return null;

        //     var poolObject = spawnedObject.GetComponent<PoolObject>();
        //     Assert.That(poolObject, Is.Not.Null, "对象应该有PoolObject组件");
        //     
        //     // Act
        //     Debug.Log("初始化PoolObject，设置自动回收时间为2秒");
        //     poolObject.Initialize(TEST_POOL_ID, _objectPool, 2f);
        //     Assert.That(spawnedObject.activeInHierarchy, Is.True, "对象初始应该处于激活状态");

        //     // 等待更多的帧
        //     Debug.Log("开始等待帧...");
        //     for(int i = 0; i < 200; i++) // 增加等待的帧数
        //     {
        //         yield return null;
        //         Debug.Log($"等待第 {i} 帧, 对象状态: {spawnedObject.activeInHierarchy}");
        //         if (!spawnedObject.activeInHierarchy)
        //         {
        //             Debug.Log($"对象在第 {i} 帧被禁用");
        //             break;
        //         }
        //     }

        //     // Assert
        //     Debug.Log($"最终检查对象状态: 激活={spawnedObject.activeInHierarchy}");
        //     Assert.That(spawnedObject.activeInHierarchy, Is.False, "对象应该在延迟后被禁用");
        // }

        [TearDown]
        public void Cleanup()
        {
            Debug.Log("清理测试环境...");
            if (_objectPool != null)
            {
                Object.DestroyImmediate(_objectPool.gameObject);
                Debug.Log("对象池已销毁");
            }
            if (_testPrefab != null)
            {
                Object.DestroyImmediate(_testPrefab);
                Debug.Log("测试预制体已销毁");
            }
        }
    }
}
