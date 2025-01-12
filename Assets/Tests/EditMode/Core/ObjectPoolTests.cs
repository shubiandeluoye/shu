using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

[TestFixture]
public class ObjectPoolTests
{
    private ObjectPool _objectPool;
    private GameObject _testPrefab;
    private const string TEST_TAG = "TestObject";

    [SetUp]
    public void Setup()
    {
        var poolObject = new GameObject("ObjectPool");
        _objectPool = poolObject.AddComponent<ObjectPool>();
        _testPrefab = new GameObject("TestPrefab");
    }

    [Test]
    public void CreatePool_WhenInitialized_CreatesCorrectNumberOfObjects()
    {
        // Arrange
        int poolSize = 5;

        // Act
        _objectPool.CreatePool(TEST_TAG, _testPrefab, poolSize);

        // Assert
        var pooledObjects = GameObject.FindGameObjectsWithTag(TEST_TAG);
        Assert.That(pooledObjects.Length, Is.EqualTo(poolSize));
    }

    [Test]
    public void SpawnFromPool_WhenPoolEmpty_AutoExpandsUnderPerformanceLimit()
    {
        // Arrange
        _objectPool.CreatePool(TEST_TAG, _testPrefab, 1);
        var firstObject = _objectPool.SpawnFromPool(TEST_TAG, Vector3.zero, Quaternion.identity);
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();
        var newObject = _objectPool.SpawnFromPool(TEST_TAG, Vector3.zero, Quaternion.identity);
        stopwatch.Stop();

        // Assert
        Assert.That(newObject, Is.Not.Null);
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(16), "Pool expansion took longer than 16ms (1 frame)");
    }

    [Test]
    public void ReturnToPool_WhenObjectReturned_DeactivatesAndEnqueues()
    {
        // Arrange
        _objectPool.CreatePool(TEST_TAG, _testPrefab, 1);
        var spawnedObject = _objectPool.SpawnFromPool(TEST_TAG, Vector3.zero, Quaternion.identity);

        // Act
        _objectPool.ReturnToPool(TEST_TAG, spawnedObject);

        // Assert
        Assert.That(spawnedObject.activeInHierarchy, Is.False);
        // Verify object is available for reuse
        var reusedObject = _objectPool.SpawnFromPool(TEST_TAG, Vector3.zero, Quaternion.identity);
        Assert.That(reusedObject, Is.EqualTo(spawnedObject));
    }

    [Test]
    public void SpawnFromPool_WithInvalidTag_ReturnsNullAndLogsWarning()
    {
        // Arrange
        LogAssert.Expect(LogType.Warning, "Pool with tag InvalidTag doesn't exist.");

        // Act
        var result = _objectPool.SpawnFromPool("InvalidTag", Vector3.zero, Quaternion.identity);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void ReturnToPool_WithInvalidTag_LogsWarning()
    {
        // Arrange
        var obj = new GameObject();
        LogAssert.Expect(LogType.Warning, "Pool with tag InvalidTag doesn't exist.");

        // Act
        _objectPool.ReturnToPool("InvalidTag", obj);
    }

    [Test]
    public void PoolObject_WithAutoRecycle_ReturnsToPoolAfterDelay()
    {
        // Arrange
        _objectPool.CreatePool(TEST_TAG, _testPrefab, 1);
        var spawnedObject = _objectPool.SpawnFromPool(TEST_TAG, Vector3.zero, Quaternion.identity);
        var poolObject = spawnedObject.GetComponent<PoolObject>();

        // Act
        poolObject.Initialize(TEST_TAG, _objectPool, 0.1f);

        // Assert
        Assert.That(spawnedObject.activeInHierarchy, Is.True);
        // Wait for auto-recycle
        yield return new WaitForSeconds(0.2f);
        Assert.That(spawnedObject.activeInHierarchy, Is.False);
    }

    [TearDown]
    public void Cleanup()
    {
        if (_objectPool != null)
            Object.DestroyImmediate(_objectPool.gameObject);
        if (_testPrefab != null)
            Object.DestroyImmediate(_testPrefab);
    }
}
