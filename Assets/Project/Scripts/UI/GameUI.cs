using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages in-game UI elements including health, score, and game state
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider healthSlider;
    
    [Header("Game State")]
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject gameOverPanel;
    
    private void Start()
    {
        // Set initial UI state
        SetHealth(100);
        SetScore(0);
        SetGameState(GameState.MainMenu);
        
        // Subscribe to events
        EventManager.Instance.AddListener<PlayerDamagedEvent>(OnPlayerDamaged);
        EventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        EventManager.Instance.AddListener<GameEndEvent>(OnGameEnd);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (EventManager.Instance != null)
        {
            EventManager.Instance.RemoveListener<PlayerDamagedEvent>(OnPlayerDamaged);
            EventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
            EventManager.Instance.RemoveListener<GameEndEvent>(OnGameEnd);
        }
    }
    
    public void SetHealth(float health)
    {
        if (healthText != null)
            healthText.text = $"Health: {health:F0}";
        if (healthSlider != null)
            healthSlider.value = health / 100f;
    }
    
    public void SetScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
    
    public void SetGameState(GameState state)
    {
        if (gameStateText != null)
            gameStateText.text = $"State: {state}";
            
        // Update UI panels
        if (waitingPanel != null)
            waitingPanel.SetActive(state == GameState.MainMenu);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(state == GameState.GameOver);
    }
    
    private void OnPlayerDamaged(PlayerDamagedEvent evt)
    {
        SetHealth(evt.RemainingHealth);
    }
    
    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        SetGameState(evt.NewState);
    }
    
    private void OnGameEnd(GameEndEvent evt)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            var resultText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (resultText != null)
            {
                resultText.text = evt.WinnerId == GameManager.Instance.LocalPlayerId ? 
                    "Victory!" : "Defeat!";
            }
        }
    }
}
