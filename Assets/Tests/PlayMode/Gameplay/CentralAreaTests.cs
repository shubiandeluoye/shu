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
    public IEnumerator TestCentralArea_StateTransitions()
    {
        // Test initial state
        Assert.That(centralArea.CurrentState, Is.EqualTo(CentralAreaState.Collecting));

        // Simulate bullet collection
        for (int i = 0; i < 21; i++)
        {
            var bullet = ObjectPool.Instance.SpawnFromPool("Bullet", 
                centralArea.transform.position + Vector3.right, 
                Quaternion.identity);
            yield return new WaitForFixedUpdate();
        }

        // Should transition to charging state
        Assert.That(centralArea.CurrentState, Is.EqualTo(CentralAreaState.Charging));
        
        // Wait for charging to complete (5-8 seconds)
        yield return new WaitForSeconds(8f);
        
        // Should have transitioned through firing and back to collecting
        Assert.That(centralArea.CurrentState, Is.EqualTo(CentralAreaState.Collecting));
    }

    [UnityTest]
    public IEnumerator TestCentralArea_ChargingTimer()
    {
        // Arrange
        float minChargeTime = 5f;
        float maxChargeTime = 8f;
        float startTime = Time.time;

        // Collect bullets to trigger charging
        for (int i = 0; i < 21; i++)
        {
            ObjectPool.Instance.SpawnFromPool("Bullet", 
                centralArea.transform.position + Vector3.right, 
                Quaternion.identity);
            yield return new WaitForFixedUpdate();
        }

        // Wait for state to change back to collecting
        yield return new WaitUntil(() => centralArea.CurrentState == CentralAreaState.Collecting);

        // Verify charging duration
        float chargeDuration = Time.time - startTime;
        Assert.That(chargeDuration, Is.GreaterThanOrEqualTo(minChargeTime));
        Assert.That(chargeDuration, Is.LessThanOrEqualTo(maxChargeTime + 0.1f));
    }

    [UnityTest]
    public IEnumerator TestCentralArea_AutoExplodeTimeout()
    {
        // Arrange
        float timeoutDuration = 30f;
        bool stateChanged = false;
        
        EventManager.Instance.AddListener<CentralAreaStateChangedEvent>(evt => 
        {
            if (evt.NewState == CentralAreaState.Firing)
                stateChanged = true;
        });

        // Act - wait for timeout
        yield return new WaitForSeconds(timeoutDuration + 0.1f);

        // Assert
        Assert.That(stateChanged, Is.True, "Central area should auto-explode after 30 seconds");
        Assert.That(centralArea.CurrentState, Is.EqualTo(CentralAreaState.Collecting), 
            "Should return to collecting state after firing");
    }

    [UnityTest]
    public IEnumerator TestCentralArea_BulletCollection()
    {
        // Arrange
        int requiredBullets = 21;
        int collectedCount = 0;
        
        EventManager.Instance.AddListener<CentralAreaStateChangedEvent>(evt => 
        {
            collectedCount = evt.CollectedCount;
        });

        // Act - collect bullets
        for (int i = 0; i < requiredBullets; i++)
        {
            var bullet = ObjectPool.Instance.SpawnFromPool("Bullet", 
                centralArea.transform.position + Vector3.right, 
                Quaternion.identity);
            yield return new WaitForFixedUpdate();
        }

        // Assert
        Assert.That(collectedCount, Is.EqualTo(requiredBullets));
        Assert.That(centralArea.CurrentState, Is.EqualTo(CentralAreaState.Charging));
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
