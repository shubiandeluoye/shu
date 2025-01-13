using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fusion;

public class NetworkMatchmakingTests : NetworkTestBase
{
    private NetworkRunner runner1;
    private NetworkRunner runner2;
    private NetworkRunner runner3;
    private NetworkMatchmaker matchmaker1;
    private NetworkMatchmaker matchmaker2;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        runner1 = CreateNetworkRunner("Host");
        runner2 = CreateNetworkRunner("Client1");
        runner3 = CreateNetworkRunner("Client2");

        matchmaker1 = runner1.gameObject.AddComponent<NetworkMatchmaker>();
        matchmaker2 = runner2.gameObject.AddComponent<NetworkMatchmaker>();

        yield return null;
    }

    [UnityTest]
    public IEnumerator Matchmaking_WhenTwoPlayersJoin_StartsGame()
    {
        // Arrange
        bool gameStarted = false;
        EventManager.Instance.AddListener<GameStartEvent>(evt => gameStarted = true);

        // Act
        yield return StartGame(runner1, GameMode.Host, "TestRoom");
        yield return StartGame(runner2, GameMode.Client, "TestRoom");

        // Wait for game start
        yield return new WaitForSeconds(1f);

        // Assert
        Assert.That(gameStarted, Is.True, "Game did not start with 2 players");
        Assert.That(FindObjectsOfType<NetworkPlayer>().Length, Is.EqualTo(2), "Incorrect number of players spawned");
    }

    [UnityTest]
    public IEnumerator Matchmaking_WhenThirdPlayerJoins_RejectsConnection()
    {
        // Arrange
        yield return StartGame(runner1, GameMode.Host, "TestRoom");
        yield return StartGame(runner2, GameMode.Client, "TestRoom");
        
        // Wait for first two players to connect
        yield return new WaitForSeconds(1f);
        
        // Act - attempt to connect third player
        yield return StartGame(runner3, GameMode.Client, "TestRoom");
        yield return new WaitForSeconds(1f);

        // Assert
        Assert.That(FindObjectsOfType<NetworkPlayer>().Length, Is.EqualTo(2), "Third player was not rejected");
    }

    [UnityTest]
    public IEnumerator Matchmaking_WhenPlayerLeaves_TriggersGameEnd()
    {
        // Arrange
        bool gameEnded = false;
        EventManager.Instance.AddListener<GameEndEvent>(evt => gameEnded = true);

        yield return StartGame(runner1, GameMode.Host, "TestRoom");
        yield return StartGame(runner2, GameMode.Client, "TestRoom");
        yield return new WaitForSeconds(1f);

        // Act
        Object.DestroyImmediate(runner2.gameObject);
        yield return new WaitForSeconds(1f);

        // Assert
        Assert.That(gameEnded, Is.True, "Game end not triggered when player left");
        Assert.That(FindObjectsOfType<NetworkPlayer>().Length, Is.EqualTo(1), "Incorrect number of players after disconnect");
    }

    [UnityTest]
    public IEnumerator NetworkLatency_UnderRequiredLimit()
    {
        // Arrange
        float maxLatency = 0.1f; // 100ms requirement
        yield return StartGame(runner1, GameMode.Host, "TestRoom");
        yield return StartGame(runner2, GameMode.Client, "TestRoom");
        yield return new WaitForSeconds(1f);

        // Act
        float latency = runner2.GetPlayerRtt(runner2.LocalPlayer);

        // Assert
        Assert.That(latency, Is.LessThan(maxLatency), $"Network latency ({latency}s) exceeds requirement (0.1s)");
    }

    [TearDown]
    public void Cleanup()
    {
        if (runner1 != null)
            Object.DestroyImmediate(runner1.gameObject);
        if (runner2 != null)
            Object.DestroyImmediate(runner2.gameObject);
        if (runner3 != null)
            Object.DestroyImmediate(runner3.gameObject);
    }
}
