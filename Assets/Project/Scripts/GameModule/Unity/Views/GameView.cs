using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameModule.Core.Data;
using GameModule.Core.Events;

namespace GameModule.Unity.Views
{
    public class GameView : MonoBehaviour
    {
        [Header("状态显示")]
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private TextMeshProUGUI countdownText;
        
        [Header("匹配信息")]
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private GameObject waitingPanel;
        
        [Header("游戏信息")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Start()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            EventManager.Instance.AddListener<GameStateChangeEvent>(OnGameStateChanged);
            EventManager.Instance.AddListener<GameCountdownEvent>(OnGameCountdown);
            EventManager.Instance.AddListener<PlayerCountChangeEvent>(OnPlayerCountChanged);
            EventManager.Instance.AddListener<ScoreUpdateEvent>(OnScoreUpdated);
        }

        private void OnGameStateChanged(GameStateChangeEvent evt)
        {
            if (stateText != null)
            {
                stateText.text = $"游戏状态: {evt.NewState}";
            }

            waitingPanel?.SetActive(evt.NewState == GameState.WaitingForPlayers);
        }

        private void OnGameCountdown(GameCountdownEvent evt)
        {
            if (countdownText != null)
            {
                countdownText.text = $"倒计时: {evt.Duration:F1}";
            }
        }

        private void OnPlayerCountChanged(PlayerCountChangeEvent evt)
        {
            if (playerCountText != null)
            {
                playerCountText.text = $"玩家: {evt.CurrentCount}/{evt.MaxCount}";
            }
        }

        private void OnScoreUpdated(ScoreUpdateEvent evt)
        {
            if (scoreText != null)
            {
                scoreText.text = $"分数: {evt.NewScore}";
                if (evt.ScoreChange > 0)
                {
                    ShowScorePopup(evt.ScoreChange, evt.Reason);
                }
            }
        }

        private void ShowScorePopup(int score, string reason)
        {
            // TODO: 实现分数弹出动画
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener<GameStateChangeEvent>(OnGameStateChanged);
            EventManager.Instance.RemoveListener<GameCountdownEvent>(OnGameCountdown);
            EventManager.Instance.RemoveListener<PlayerCountChangeEvent>(OnPlayerCountChanged);
            EventManager.Instance.RemoveListener<ScoreUpdateEvent>(OnScoreUpdated);
        }
    }
} 