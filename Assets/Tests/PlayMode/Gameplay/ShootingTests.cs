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
    public IEnumerator Shoot_WhenFiringStraight_BulletMovesAtZeroDegrees()
    {
        // Arrange
        float expectedAngle = 0f;

        // Act
        player.SimulateShoot(expectedAngle);
        yield return new WaitForFixedUpdate();

        // Assert
        var spawnedBullet = GameObject.FindGameObjectWithTag("Bullet");
        Assert.That(spawnedBullet, Is.Not.Null);
        var bulletController = spawnedBullet.GetComponent<BulletController>();
        Assert.That(bulletController.GetDirection().normalized, Is.EqualTo(Vector2.right).Using(Vector2Comparer.Instance));
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
    public IEnumerator Bullet_WhenSpawned_HasCorrectVisuals()
    {
        // Act
        player.SimulateShoot(0f);
        yield return new WaitForFixedUpdate();

        // Assert
        var spawnedBullet = GameObject.FindGameObjectWithTag("Bullet");
        var spriteRenderer = spawnedBullet.GetComponent<SpriteRenderer>();
        var expectedColor = new Color(1f, 0.5f, 0f); // Orange
        Assert.That(spriteRenderer.color, Is.EqualTo(expectedColor));
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
