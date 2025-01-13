using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ShootingTests : InputTestBase
{
    private PlayerController player;
    private GameObject playerObject;
    private BulletController bullet;
    private GameObject bulletObject;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create player
        playerObject = new GameObject("TestPlayer");
        player = playerObject.AddComponent<PlayerController>();

        // Create bullet prefab
        bulletObject = new GameObject("TestBullet");
        bullet = bulletObject.AddComponent<BulletController>();
        bulletObject.AddComponent<BulletPrefabSetup>();

        // Initialize object pool
        var poolInitializer = new GameObject("PoolInitializer").AddComponent<BulletPoolInitializer>();

        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator TestShooting_Angles()
    {
        // Test straight shot (0 degrees)
        player.SimulateShoot(0f);
        yield return new WaitForFixedUpdate();
        var straightBullet = GameObject.FindGameObjectWithTag("Bullet");
        Assert.That(straightBullet.GetComponent<BulletController>().GetDirection().normalized, 
            Is.EqualTo(Vector2.right).Using(Vector2Comparer.Instance));

        // Test 30 degree shot
        player.SimulateShoot(30f);
        yield return new WaitForFixedUpdate();
        var angle30Bullet = GameObject.FindGameObjectsWithTag("Bullet")[1];
        var angle30Dir = angle30Bullet.GetComponent<BulletController>().GetDirection().normalized;
        Assert.That(Vector2.Angle(angle30Dir, Vector2.right), Is.EqualTo(30f).Within(0.1f));

        // Test 45 degree shot
        player.SimulateShoot(45f);
        yield return new WaitForFixedUpdate();
        var angle45Bullet = GameObject.FindGameObjectsWithTag("Bullet")[2];
        var angle45Dir = angle45Bullet.GetComponent<BulletController>().GetDirection().normalized;
        Assert.That(Vector2.Angle(angle45Dir, Vector2.right), Is.EqualTo(45f).Within(0.1f));
    }

    [UnityTest]
    public IEnumerator TestShooting_BulletSpeed()
    {
        // Arrange
        float expectedSpeed = 8f;
        Vector3 startPos = Vector3.zero;

        // Act
        player.SimulateShoot(0f);
        yield return new WaitForFixedUpdate();
        var bullet = GameObject.FindGameObjectWithTag("Bullet");
        var initialPos = bullet.transform.position;
        
        yield return new WaitForFixedUpdate();
        
        // Assert
        var displacement = (bullet.transform.position - initialPos).magnitude;
        var actualSpeed = displacement / Time.fixedDeltaTime;
        Assert.That(actualSpeed, Is.EqualTo(expectedSpeed).Within(0.1f));
    }

    [UnityTest]
    public IEnumerator TestShooting_BounceCount()
    {
        // Arrange
        var walls = new GameObject[2];
        walls[0] = new GameObject("Wall1") { tag = "Wall" };
        walls[0].AddComponent<BoxCollider2D>();
        walls[0].transform.position = new Vector3(2f, 0f, 0f);
        
        walls[1] = new GameObject("Wall2") { tag = "Wall" };
        walls[1].AddComponent<BoxCollider2D>();
        walls[1].transform.position = new Vector3(-2f, 0f, 0f);

        // Act
        player.SimulateShoot(0f);
        yield return new WaitForSeconds(1f); // Wait for bounces

        // Assert
        var bullet = GameObject.FindGameObjectWithTag("Bullet");
        var bulletController = bullet.GetComponent<BulletController>();
        Assert.That(bulletController.GetBounceCount(), Is.LessThanOrEqualTo(3), "Bullet should not bounce more than 3 times");
        
        // Cleanup
        foreach (var wall in walls)
        {
            Object.DestroyImmediate(wall);
        }
    }

    [UnityTest]
    public IEnumerator Shoot_WhenBulletBounces_CountsReflections()
    {
        // Arrange
        var wall = new GameObject("Wall");
        var wallCollider = wall.AddComponent<BoxCollider2D>();
        wall.transform.position = new Vector3(2f, 0f, 0f);

        // Act
        player.SimulateShoot(0f);
        yield return new WaitForSeconds(0.5f); // Wait for collision

        // Assert
        var spawnedBullet = GameObject.FindGameObjectWithTag("Bullet");
        var bulletController = spawnedBullet.GetComponent<BulletController>();
        Assert.That(bulletController.GetBounceCount(), Is.GreaterThan(0));
    }

    [UnityTest]
    public IEnumerator Shoot_WhenBulletHitsPlayer_DealsDamage()
    {
        // Arrange
        var targetObject = new GameObject("Target");
        var target = targetObject.AddComponent<PlayerController>();
        targetObject.transform.position = new Vector3(2f, 0f, 0f);
        float initialHealth = target.Health;

        // Act
        player.SimulateShoot(0f);
        yield return new WaitForSeconds(0.5f); // Wait for collision

        // Assert
        Assert.That(target.Health, Is.LessThan(initialHealth));
    }

    [UnityTest]
    public IEnumerator TestObjectPool_Creation()
    {
        // Arrange
        int expectedInitialSize = 20;
        
        // Act
        yield return new WaitForFixedUpdate();
        var pooledBullets = GameObject.FindGameObjectsWithTag("Bullet");
        
        // Assert
        Assert.That(pooledBullets.Length, Is.EqualTo(expectedInitialSize), 
            "Initial pool size should be 20");
    }

    [UnityTest]
    public IEnumerator TestObjectPool_Recycle()
    {
        // Arrange
        player.SimulateShoot(0f);
        yield return new WaitForFixedUpdate();
        var bullet = GameObject.FindGameObjectWithTag("Bullet");
        var initialPosition = bullet.transform.position;
        
        // Act
        ObjectPool.Instance.ReturnToPool("Bullet", bullet);
        yield return new WaitForFixedUpdate();
        
        // Assert
        Assert.That(bullet.activeInHierarchy, Is.False, "Bullet should be deactivated when recycled");
        
        // Test reuse
        player.SimulateShoot(0f);
        yield return new WaitForFixedUpdate();
        var reusedBullet = GameObject.FindGameObjectWithTag("Bullet");
        Assert.That(reusedBullet, Is.EqualTo(bullet), "Pool should reuse recycled bullet");
    }

    [UnityTest]
    public IEnumerator TestObjectPool_MaxCount()
    {
        // Arrange
        int maxPoolSize = 50;
        var bullets = new List<GameObject>();
        
        // Act
        for (int i = 0; i < maxPoolSize + 10; i++)
        {
            player.SimulateShoot(Random.Range(0f, 360f));
            yield return new WaitForFixedUpdate();
            bullets.Add(GameObject.FindGameObjectsWithTag("Bullet").Last());
        }
        
        // Assert
        var activeBullets = GameObject.FindGameObjectsWithTag("Bullet");
        Assert.That(activeBullets.Length, Is.LessThanOrEqualTo(maxPoolSize), 
            "Active bullets should not exceed max pool size");
    }

    [UnityTest]
    public IEnumerator Bullet_AfterLifetime_IsRecycled()
    {
        // Arrange
        player.SimulateShoot(0f);
        var spawnedBullet = GameObject.FindGameObjectWithTag("Bullet");
        
        // Act
        yield return new WaitForSeconds(10f); // Maximum lifetime

        // Assert
        Assert.That(spawnedBullet.activeInHierarchy, Is.False);
    }

    public class Vector2Comparer : IEqualityComparer<Vector2>
    {
        public static readonly Vector2Comparer Instance = new Vector2Comparer();
        private const float Epsilon = 0.001f;

        public bool Equals(Vector2 x, Vector2 y)
        {
            return Vector2.Distance(x, y) < Epsilon;
        }

        public int GetHashCode(Vector2 obj)
        {
            return obj.GetHashCode();
        }
    }

    [TearDown]
    public void Cleanup()
    {
        if (playerObject != null)
            Object.DestroyImmediate(playerObject);
        if (bulletObject != null)
            Object.DestroyImmediate(bulletObject);
    }
}
