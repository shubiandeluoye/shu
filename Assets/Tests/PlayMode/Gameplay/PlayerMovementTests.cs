using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovementTests : InputTestBase
{
    private PlayerController player;
    private GameObject playerObject;
    private Vector3 initialPosition;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create player
        playerObject = new GameObject("TestPlayer");
        player = playerObject.AddComponent<PlayerController>();
        initialPosition = Vector3.zero;
        playerObject.transform.position = initialPosition;
        
        // Wait for initialization
        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Movement_WhenMovingRight_StaysWithinBounds()
    {
        // Arrange
        Vector2 rightMovement = Vector2.right;
        float expectedMaxX = 3.5f; // Half of 7x7 area

        // Act
        player.SimulateMove(rightMovement);
        yield return new WaitForSeconds(1f); // Wait for movement

        // Assert
        Assert.That(player.transform.position.x, Is.LessThanOrEqualTo(expectedMaxX));
    }

    [UnityTest]
    public IEnumerator Movement_WhenMovingDiagonally_NormalizesSpeed()
    {
        // Arrange
        Vector2 diagonalMovement = new Vector2(1f, 1f).normalized;
        float expectedSpeed = 5f;
        Vector3 startPos = player.transform.position;

        // Act
        player.SimulateMove(diagonalMovement);
        yield return new WaitForFixedUpdate();

        // Assert
        Vector3 movement = player.transform.position - startPos;
        float actualSpeed = movement.magnitude / Time.fixedDeltaTime;
        Assert.That(actualSpeed, Is.EqualTo(expectedSpeed).Within(0.1f));
    }

    [UnityTest]
    public IEnumerator Movement_WhenHittingWall_StopsAtBoundary()
    {
        // Arrange
        Vector2 leftMovement = Vector2.left;
        float expectedMinX = -3.5f; // Left boundary of 7x7 area

        // Act
        player.SimulateMove(leftMovement);
        yield return new WaitForSeconds(1f); // Wait for movement to complete

        // Assert
        Assert.That(player.transform.position.x, Is.GreaterThanOrEqualTo(expectedMinX));
    }

    [UnityTest]
    public IEnumerator Movement_WhenCrossingRightBoundary_TriggersOutOfBounds()
    {
        // Arrange
        bool outOfBoundsTriggered = false;
        EventManager.Instance.AddListener<PlayerOutOfBoundsEvent>(evt => outOfBoundsTriggered = true);
        Vector2 rightMovement = Vector2.right;

        // Act
        player.SimulateMove(rightMovement);
        yield return new WaitForSeconds(1f);

        // Assert
        Assert.That(outOfBoundsTriggered, Is.True);
    }

    [UnityTest]
    public IEnumerator Movement_AllDirections_MaintainsConstantSpeed()
    {
        // Test all 8 directions
        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            new Vector2(1, 1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(-1, -1).normalized
        };

        float expectedSpeed = 5f;

        foreach (var direction in directions)
        {
            // Reset position
            playerObject.transform.position = Vector3.zero;
            Vector3 startPos = playerObject.transform.position;

            // Move in direction
            player.SimulateMove(direction);
            yield return new WaitForFixedUpdate();

            // Verify speed
            Vector3 movement = playerObject.transform.position - startPos;
            float actualSpeed = movement.magnitude / Time.fixedDeltaTime;
            Assert.That(actualSpeed, Is.EqualTo(expectedSpeed).Within(0.1f),
                $"Speed not correct for direction {direction}");
        }
    }

    [UnityTest]
    public IEnumerator Components_WhenSpawned_HaveCorrectConfiguration()
    {
        yield return new WaitForFixedUpdate();

        // Verify BoxCollider2D
        var collider = playerObject.GetComponent<BoxCollider2D>();
        Assert.That(collider, Is.Not.Null, "BoxCollider2D missing");
        Assert.That(collider.size, Is.EqualTo(Vector2.one), "Collider size not 1x1");
        Assert.That(collider.isTrigger, Is.False, "Collider should not be trigger");

        // Verify Rigidbody2D
        var rb = playerObject.GetComponent<Rigidbody2D>();
        Assert.That(rb, Is.Not.Null, "Rigidbody2D missing");
        Assert.That(rb.isKinematic, Is.True, "Rigidbody should be kinematic");
        Assert.That(rb.interpolation, Is.EqualTo(RigidbodyInterpolation2D.Interpolate));
        Assert.That(rb.collisionDetection, Is.EqualTo(CollisionDetectionMode2D.Continuous));
        Assert.That(rb.constraints.HasFlag(RigidbodyConstraints2D.FreezeRotation), Is.True);

        // Verify NetworkObject
        var netObj = playerObject.GetComponent<NetworkObject>();
        if (netObj != null) // Only check if NetworkObject exists
        {
            Assert.That(netObj.PredictionMode, Is.EqualTo(NetworkPredictionMode.Full));
        }
    }

    [TearDown]
    public void Cleanup()
    {
        if (playerObject != null)
            Object.DestroyImmediate(playerObject);
    }
}

public abstract class InputTestBase
{
    protected InputManager inputManager;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var inputManagerObject = new GameObject("InputManager");
        inputManager = inputManagerObject.AddComponent<InputManager>();
    }

    [OneTimeTearDown]
    public void GlobalCleanup()
    {
        if (inputManager != null)
            Object.DestroyImmediate(inputManager.gameObject);
    }
}
