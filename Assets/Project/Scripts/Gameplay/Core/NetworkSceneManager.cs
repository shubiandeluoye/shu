using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : MonoBehaviour
{
    [Header("Network Prefabs")]
    public NetworkPrefabRef playerPrefab;
    public NetworkPrefabRef bulletPrefab;
    public NetworkPrefabRef wallPrefab;
    
    [Header("Scene References")]
    public GameObject gameManagerPrefab;
    public GameObject eventSystemPrefab;
    public GameObject gameUIPrefab;
    
    private void Awake()
    {
        // Add required scene objects
        if (FindObjectOfType<GameManager>() == null && gameManagerPrefab != null)
            Instantiate(gameManagerPrefab);
            
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null && eventSystemPrefab != null)
            Instantiate(eventSystemPrefab);
            
        if (FindObjectOfType<Canvas>() == null && gameUIPrefab != null)
            Instantiate(gameUIPrefab);
    }
    
    public void SetupScene()
    {
        if (!Runner.IsServer) return;

        // Create walls for player areas (7x7)
        CreatePlayerArea(-7f); // Left player area
        CreatePlayerArea(7f);  // Right player area
        
        // Create central area (2x2)
        var centralArea = Runner.Spawn(centralAreaPrefab, Vector3.zero, Quaternion.identity);
        centralArea.transform.localScale = new Vector3(2f, 2f, 1f);
        
        // Create UI elements if needed
        if (FindObjectOfType<Canvas>() == null && gameUIPrefab != null)
        {
            var ui = Instantiate(gameUIPrefab);
            ui.name = "GameUI";
        }
        
        Debug.Log("[NetworkSceneManager] Scene setup completed");
    }
    
    private void CreatePlayerArea(float xOffset)
    {
        // Create three walls (top, side, bottom) for 7x7 area
        var horizontalWallSize = new Vector3(7f, 0.5f, 1f);
        var verticalWallSize = new Vector3(0.5f, 7f, 1f);
        
        // Top wall
        CreateWall(new Vector3(xOffset, 3.5f, 0f), horizontalWallSize);
        
        // Side wall (right for left area, left for right area)
        float sideOffset = xOffset > 0 ? -3.5f : 3.5f;
        CreateWall(new Vector3(xOffset + sideOffset, 0f, 0f), verticalWallSize);
        
        // Bottom wall
        CreateWall(new Vector3(xOffset, -3.5f, 0f), horizontalWallSize);
        
        // Add spawn point
        var spawnPoint = new GameObject($"SpawnPoint_{(xOffset < 0 ? "Left" : "Right")}");
        spawnPoint.transform.position = new Vector3(xOffset, 0f, 0f);
        spawnPoint.transform.parent = transform;
    }
    
    private void CreateWall(Vector3 position, Vector3 scale)
    {
        var wall = Runner.Spawn(wallPrefab, position, Quaternion.identity);
        wall.transform.localScale = scale;
    }
}
