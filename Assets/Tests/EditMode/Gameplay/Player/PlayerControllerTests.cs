using NUnit.Framework;
using UnityEngine;
using Fusion;

[TestFixture]
public class PlayerControllerTests
{
    private PlayerController player;
    private GameObject playerObject;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("TestPlayer");
        player = playerObject.AddComponent<PlayerController>();
    }

    [Test]
    public void InitialState_HasCorrectDefaultValues()
    {
        // Assert
        Assert.That(player.Health, Is.EqualTo(100f));
        Assert.That(player.IsAlive, Is.True);
        Assert.That(player.GetBounceCount(), Is.EqualTo(3));
        Assert.That(player.GetCurrentBulletType(), Is.EqualTo(BulletType.Small));
    }

    [Test]
    public void SimulateMove_UpdatesPosition()
    {
        // Arrange
        Vector2 testDirection = Vector2.right;

        // Act
        player.SimulateMove(testDirection);

        // Assert - Note: Actual position change happens in FixedUpdateNetwork
        Assert.That(player.Position, Is.Not.EqualTo(Vector3.zero));
    }

    [Test]
    public void TakeDamage_ReducesHealth()
    {
        // Act
        player.TakeDamage(20f);

        // Assert
        Assert.That(player.Health, Is.EqualTo(80f));
        Assert.That(player.IsAlive, Is.True);
    }

    [Test]
    public void TakeDamage_WhenFatal_SetsIsAliveFalse()
    {
        // Act
        player.TakeDamage(100f);

        // Assert
        Assert.That(player.Health, Is.EqualTo(0f));
        Assert.That(player.IsAlive, Is.False);
    }

    [Test]
    public void SimulateShoot_UpdatesShootAngle()
    {
        // Arrange
        float testAngle = 45f;

        // Act
        player.SimulateShoot(testAngle);

        // Assert
        Assert.That(player.ShootAngle, Is.EqualTo(testAngle));
    }

    [TearDown]
    public void Cleanup()
    {
        if (playerObject != null)
            Object.DestroyImmediate(playerObject);
    }
}
