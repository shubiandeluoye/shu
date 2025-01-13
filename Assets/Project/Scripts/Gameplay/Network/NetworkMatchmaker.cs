using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// Handles matchmaking and room management for 1v1 battles
/// </summary>
public class NetworkMatchmaker : MonoBehaviour
{
    private NetworkRunner _runner;
    private Dictionary<PlayerRef, NetworkPlayer> _players;
    
    private void Awake()
    {
        _players = new Dictionary<PlayerRef, NetworkPlayer>();
        _runner = GetComponent<NetworkRunner>();
    }
    
    public async Task StartMatchmaking()
    {
        // Configure game settings
        var gameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "1v1_Session",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2,
            CustomLobbyName = "1v1_Lobby",
            CustomArgs = new Dictionary<string, SessionProperty>()
            {
                { "GameType", SessionProperty.Create("1v1") },
                { "Version", SessionProperty.Create("1.0") }
            }
        };

        Debug.Log("[NetworkMatchmaker] Starting matchmaking...");
        
        try
        {
            await _runner.StartGame(gameArgs);
            Debug.Log("[NetworkMatchmaker] Matchmaking started successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NetworkMatchmaker] Failed to start matchmaking: {e.Message}");
        }
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[NetworkMatchmaker] Player {player} joined");
        
        if (_players.Count >= 2)
        {
            Debug.LogWarning("[NetworkMatchmaker] Room is full, rejecting player");
            runner.Disconnect(player);
            return;
        }
        
        // Validate room state
        if (_players.Count > 2)
        {
            Debug.LogError("[NetworkMatchmaker] Invalid room state: more than 2 players");
            foreach (var p in _players.Keys)
            {
                runner.Disconnect(p);
            }
            _players.Clear();
            return;
        }
        
        // Calculate spawn position based on player count
        Vector3 spawnPosition;
        if (_players.Count == 0)
        {
            // First player spawns on left side
            spawnPosition = new Vector3(-3.5f, 0, 0); // Half of 7x7 area
        }
        else
        {
            // Second player spawns on right side
            spawnPosition = new Vector3(3.5f, 0, 0);
        }
        var playerObject = runner.Spawn(_runner.GetComponent<NetworkPrefabsConfig>().playerPrefab, 
            spawnPosition, Quaternion.identity, player);
            
        var networkPlayer = playerObject.GetComponent<NetworkPlayer>();
        _players[player] = networkPlayer;
        
        if (_players.Count == 2)
        {
            EventManager.Instance.TriggerEvent(new GameStartEvent 
            { 
                Player1Id = _players.Keys.First().PlayerId,
                Player2Id = _players.Keys.Last().PlayerId
            });
        }
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[NetworkMatchmaker] Player {player} left");
        
        if (_players.TryGetValue(player, out var networkPlayer))
        {
            runner.Despawn(networkPlayer.Object);
            _players.Remove(player);
            
            if (_players.Count < 2)
            {
                EventManager.Instance.TriggerEvent(new GameEndEvent 
                { 
                    WinnerId = _players.Keys.FirstOrDefault().PlayerId,
                    LoserId = player.PlayerId,
                    Reason = "Player disconnected"
                });
            }
        }
    }
    
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"[NetworkMatchmaker] Session shutdown: {shutdownReason}");
        
        foreach (var player in _players.Values)
        {
            if (player != null && player.Object != null)
                runner.Despawn(player.Object);
        }
        
        _players.Clear();
    }
}
