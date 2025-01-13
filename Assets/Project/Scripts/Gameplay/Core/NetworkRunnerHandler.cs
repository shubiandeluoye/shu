using UnityEngine;
using Fusion;
using System.Threading.Tasks;

[RequireComponent(typeof(NetworkSceneManager))]
public class NetworkRunnerHandler : MonoBehaviour
{
    private NetworkRunner _runner;
    private NetworkSceneManager _sceneManager;
    
    private void Awake()
    {
        _sceneManager = GetComponent<NetworkSceneManager>();
        _runner = GetComponent<NetworkRunner>();
    }
    
    public async Task StartGame(GameMode mode, string roomName)
    {
        // Configure game settings
        var startGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2, // 1v1 game
            CustomLobbyName = "1v1_Lobby",
            CustomArgs = new Dictionary<string, SessionProperty>()
            {
                { "GameType", SessionProperty.Create("1v1") },
                { "Version", SessionProperty.Create("1.0") }
            }
        };

        Debug.Log($"[NetworkRunnerHandler] Starting game session: {roomName}");
        
        try
        {
            // Create the game
            await _runner.StartGame(startGameArgs);
            
            // Setup scene objects
            _sceneManager.SetupScene();
            
            Debug.Log("[NetworkRunnerHandler] Game session started successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"[NetworkRunnerHandler] Failed to start game: {e.Message}");
        }
    }
}
