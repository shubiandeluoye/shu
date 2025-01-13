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
