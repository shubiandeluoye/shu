using UnityEngine;
using Fusion;

public class NetworkPrefabsConfig : MonoBehaviour
{
    [Header("Network Prefabs")]
    public NetworkPrefabRef playerPrefab;
    public NetworkPrefabRef bulletPrefab;
    public NetworkPrefabRef wallPrefab;
    public NetworkPrefabRef centralAreaPrefab;
    
    [Header("Scene Objects")]
    public GameObject gameUIPrefab;
    public GameObject eventSystemPrefab;
    
    private void Awake()
    {
        // Add required scene objects if missing
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null && eventSystemPrefab != null)
        {
            var eventSystem = Instantiate(eventSystemPrefab);
            eventSystem.name = "EventSystem";
            Debug.Log("[NetworkPrefabsConfig] Created EventSystem");
        }
        
        if (FindObjectOfType<Canvas>() == null && gameUIPrefab != null)
        {
            var ui = Instantiate(gameUIPrefab);
            ui.name = "GameUI";
            
            // Verify UI layers
            var healthDisplay = ui.transform.Find("HealthDisplay");
            var scoreDisplay = ui.transform.Find("ScoreDisplay");
            
            if (healthDisplay != null && healthDisplay.gameObject.layer != 17)
            {
                healthDisplay.gameObject.layer = 17;
                Debug.Log("[NetworkPrefabsConfig] Set HealthDisplay to layer 17");
            }
            
            if (scoreDisplay != null && scoreDisplay.gameObject.layer != 31)
            {
                scoreDisplay.gameObject.layer = 31;
                Debug.Log("[NetworkPrefabsConfig] Set ScoreDisplay to layer 31");
            }
            
            Debug.Log("[NetworkPrefabsConfig] Created GameUI");
        }
    }
    
    public void RegisterPrefabs(NetworkRunner runner)
    {
        // Register all network prefabs
        if (playerPrefab != null)
        {
            runner.AddNetworkPrefab(playerPrefab);
            Debug.Log($"[NetworkPrefabsConfig] Registered Player prefab");
        }
            
        if (bulletPrefab != null)
        {
            runner.AddNetworkPrefab(bulletPrefab);
            Debug.Log($"[NetworkPrefabsConfig] Registered Bullet prefab");
        }
            
        if (wallPrefab != null)
        {
            runner.AddNetworkPrefab(wallPrefab);
            Debug.Log($"[NetworkPrefabsConfig] Registered Wall prefab");
        }
            
        if (centralAreaPrefab != null)
        {
            runner.AddNetworkPrefab(centralAreaPrefab);
            Debug.Log($"[NetworkPrefabsConfig] Registered Central Area prefab");
        }
        
        Debug.Log("[NetworkPrefabsConfig] All prefabs registered");
    }
}
