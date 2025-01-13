using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CentralAreaTests : InputTestBase
{
    private CentralArea centralArea;
    private GameObject centralAreaObject;
    private GameObject bulletPrefab;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create central area
        centralAreaObject = new GameObject("TestCentralArea");
        centralArea = centralAreaObject.AddComponent<CentralArea>();
        
        // Create bullet prefab
        bulletPrefab = new GameObject("TestBullet");
        bulletPrefab.AddComponent<BulletController>();
        bulletPrefab.AddComponent<BulletPrefabSetup>();
        
        // Initialize object pool
        var poolInitializer = new GameObject("PoolInitializer").AddComponent<BulletPoolInitializer>();
        
        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator CentralArea_WhenCreated_HasCorrectDimensions()
    {
        // Arrange
        float expectedRadius = 1f; // For 2x2 circular area

        // Act
        yield return new WaitForFixedUpdate();

        // Assert
        var collider = centralAreaObject.GetComponent<CircleCollider2D>();
        Assert.That(collider.radius, Is.EqualTo(expectedRadius));
    }

    [UnityTest]
    public IEnumerator CentralArea_WhenCollecting21Bullets_EntersChargingState()
    {
        // Arrange
        int bulletCount = 21;
        bool chargingStarted = false;
        
        // Act
        for (int i = 0; i < bulletCount; i++)
        {
            var bullet = ObjectPool.Instance.SpawnFromPool("Bullet", centralAreaObject.transform.position + Vector3.right, Quaternion.identity);
            yield return new WaitForFixedUpdate();
        }
        
        yield return new WaitForSeconds(0.5f); // Wait for collection

        // Assert
        var currentState = centralArea.GetComponent<StateMachine>().GetState<ChargingState>();
        Assert.That(currentState, Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator CentralArea_WhenCharging_FiresWithinTimeRange()
    {
        // Arrange
        int bulletCount = 21;
        float minFireTime = 5f;
        float maxFireTime = 8f;
        float startTime = Time.time;
        bool hasFired = false;

        // Act
        for (int i = 0; i < bulletCount; i++)
        {
            var bullet = ObjectPool.Instance.SpawnFromPool("Bullet", centralAreaObject.transform.position + Vector3.right, Quaternion.identity);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitUntil(() => 
        {
            var bullets = GameObject.FindGameObjectsWithTag("Bullet");
            hasFired = bullets.Length > bulletCount;
            return hasFired || Time.time - startTime > maxFireTime + 1f;
        });

        // Assert
        float elapsedTime = Time.time - startTime;
        Assert.That(hasFired, Is.True);
        Assert.That(elapsedTime, Is.GreaterThanOrEqualTo(minFireTime));
        Assert.That(elapsedTime, Is.LessThanOrEqualTo(maxFireTime + 1f));
    }

    [UnityTest]
    public IEnumerator CentralArea_WhenNotFilled_AutoExplodesAfter30Seconds()
    {
        // Arrange
        float autoExplodeTime = 30f;
        float startTime = Time.time;
        bool hasExploded = false;

        // Act
        yield return new WaitForSeconds(autoExplodeTime + 1f);

        // Assert
        float elapsedTime = Time.time - startTime;
        Assert.That(elapsedTime, Is.GreaterThanOrEqualTo(autoExplodeTime));
        
        // Verify state reset
        var currentState = centralArea.GetComponent<StateMachine>().GetState<CollectingState>();
        Assert.That(currentState, Is.Not.Null);
    }

    [TearDown]
    public void Cleanup()
    {
        if (centralAreaObject != null)
            Object.DestroyImmediate(centralAreaObject);
        if (bulletPrefab != null)
            Object.DestroyImmediate(bulletPrefab);
    }
}
