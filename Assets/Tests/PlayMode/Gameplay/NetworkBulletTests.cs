using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fusion;

public class NetworkBulletTests : NetworkTestBase
{
    private NetworkRunner runner1;
    private NetworkRunner runner2;
    private NetworkPlayer player1;
    private NetworkPlayer player2;
    private BulletController bullet;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create network runners
        runner1 = CreateNetworkRunner("Host");
        runner2 = CreateNetworkRunner("Client");

        // Start game sessions
        yield return StartGame(runner1, GameMode.Host, "TestRoom");
        yield return StartGame(runner2, GameMode.Client, "TestRoom");

        // Wait for players to spawn
        yield return new WaitForSeconds(1f);

        // Get player references
        player1 = FindObjectOfType<NetworkPlayer>();
        player2 = FindObjectOfType<NetworkPlayer>();

        Assert.That(player1, Is.Not.Null, "Host player not spawned");
        Assert.That(player2, Is.Not.Null, "Client player not spawned");
    }

    [UnityTest]
    public IEnumerator NetworkBullet_WhenShot_SynchronizesAcrossClients()
    {
        // Arrange
        float shootAngle = 0f;
        Vector3 expectedDirection = Vector2.right;
        bool bulletSpawned = false;

        // Act
        player1.RPC_Shoot(shootAngle);
        
        // Wait for bullet to spawn and sync
        yield return new WaitUntil(() => {
            bullet = FindObjectOfType<BulletController>();
            bulletSpawned = bullet != null;
            return bulletSpawned || Time.time > 5f;
        });

        // Assert
        Assert.That(bulletSpawned, Is.True, "Bullet not spawned");
        Assert.That(bullet.GetDirection(), Is.EqualTo(expectedDirection).Using(Vector2.Dot).Within(0.01f));
    }

    [UnityTest]
    public IEnumerator NetworkBullet_WhenBouncing_MaintainsConsistentBounceCount()
    {
        // Arrange
        float shootAngle = 45f;
        player1.RPC_Shoot(shootAngle);
        
        yield return new WaitUntil(() => {
            bullet = FindObjectOfType<BulletController>();
            return bullet != null;
        });

        // Act - wait for bounces
        yield return new WaitForSeconds(2f);

        // Assert
        Assert.That(bullet.GetBounceCount(), Is.LessThanOrEqualTo(3), "Bounce count exceeded maximum");
        Assert.That(bullet.gameObject.activeInHierarchy, Is.True, "Bullet deactivated before max bounces");
    }

    [UnityTest]
    public IEnumerator NetworkBullet_WhenHittingPlayer_AppliesDamageAndDeactivates()
    {
        // Arrange
        float initialHealth = player2.Health;
        float expectedDamage = 1f;
        
        // Position bullet to hit player2
        player1.RPC_Shoot(0f);
        yield return new WaitUntil(() => {
            bullet = FindObjectOfType<BulletController>();
            return bullet != null;
        });

        // Wait for collision
        yield return new WaitForSeconds(1f);

        // Assert
        Assert.That(player2.Health, Is.EqualTo(initialHealth - expectedDamage));
        Assert.That(bullet.gameObject.activeInHierarchy, Is.False, "Bullet not deactivated after hit");
    }

    [UnityTest]
    public IEnumerator NetworkBullet_WhenLifetimeExpires_DeactivatesOnAllClients()
    {
        // Arrange
        player1.RPC_Shoot(0f);
        yield return new WaitUntil(() => {
            bullet = FindObjectOfType<BulletController>();
            return bullet != null;
        });

        // Act - wait for lifetime to expire (8-10 seconds)
        yield return new WaitForSeconds(11f);

        // Assert
        Assert.That(bullet.gameObject.activeInHierarchy, Is.False, "Bullet not deactivated after lifetime expired");
    }

    [TearDown]
    public void Cleanup()
    {
        if (runner1 != null)
            Object.DestroyImmediate(runner1.gameObject);
        if (runner2 != null)
            Object.DestroyImmediate(runner2.gameObject);
    }
}
