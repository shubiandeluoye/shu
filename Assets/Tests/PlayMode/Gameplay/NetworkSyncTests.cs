using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fusion;

public class NetworkSyncTests : NetworkTestBase
{
    private NetworkRunner runner1;
    private NetworkRunner runner2;
    private PlayerController player1;
    private PlayerController player2;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create two network runners for host and client
        runner1 = CreateNetworkRunner("Host");
        runner2 = CreateNetworkRunner("Client");

        // Start game sessions
        yield return StartGame(runner1, GameMode.Host, "TestRoom");
        yield return StartGame(runner2, GameMode.Client, "TestRoom");

        // Wait for players to spawn
        yield return new WaitForSeconds(1f);

        // Get player references
        player1 = FindObjectOfType<PlayerController>();
        player2 = FindObjectOfType<PlayerController>();

        Assert.That(player1, Is.Not.Null, "Host player not spawned");
        Assert.That(player2, Is.Not.Null, "Client player not spawned");
    }

    [UnityTest]
    public IEnumerator NetworkSync_WhenPlayerMoves_PositionSyncedWithinLatencyLimit()
    {
        // Arrange
        Vector2 moveDirection = Vector2.right;
        float maxLatency = 0.1f; // 100ms requirement
        Vector3 startPos = player1.transform.position;

        // Act
        player1.SimulateMove(moveDirection);
        
        // Wait for sync
        yield return new WaitForSeconds(maxLatency);

        // Assert
        float positionDelta = Vector3.Distance(player1.transform.position, player2.transform.position);
        Assert.That(positionDelta, Is.LessThan(0.1f), "Position sync delta too large");
    }

    [UnityTest]
    public IEnumerator NetworkSync_WhenPlayerHitsWall_CollisionHandledConsistently()
    {
        // Arrange
        Vector2 moveDirection = Vector2.left;
        Vector3 startPos = player1.transform.position;
        float expectedMinX = -3.5f; // Left boundary of 7x7 area

        // Act
        player1.SimulateMove(moveDirection);
        yield return new WaitForSeconds(1f);

        // Assert
        Assert.That(player1.transform.position.x, Is.GreaterThanOrEqualTo(expectedMinX));
        Assert.That(player2.transform.position.x, Is.GreaterThanOrEqualTo(expectedMinX));
    }

    [UnityTest]
    public IEnumerator NetworkSync_WhenPlayerCrossesRightBoundary_OutOfBoundsTriggeredOnBothClients()
    {
        // Arrange
        bool host_outOfBounds = false;
        bool client_outOfBounds = false;
        
        EventManager.Instance.AddListener<PlayerOutOfBoundsEvent>(evt => 
        {
            if (evt.PlayerId == player1.Object.Id)
                host_outOfBounds = true;
            else
                client_outOfBounds = true;
        });

        // Act
        player1.SimulateMove(Vector2.right);
        yield return new WaitForSeconds(1f);

        // Assert
        Assert.That(host_outOfBounds, Is.True, "Host out of bounds not triggered");
        Assert.That(client_outOfBounds, Is.True, "Client out of bounds not triggered");
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

public abstract class NetworkTestBase
{
    protected NetworkRunner CreateNetworkRunner(string name)
    {
        var go = new GameObject($"Runner_{name}");
        var runner = go.AddComponent<NetworkRunner>();
        runner.name = name;
        return runner;
    }

    protected IEnumerator StartGame(NetworkRunner runner, GameMode mode, string roomName)
    {
        var result = runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName,
            Scene = 0,
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        while (!result.IsCompleted)
            yield return null;
    }
}
