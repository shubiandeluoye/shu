using UnityEngine;
using GameModule.Core.Data;

namespace GameModule.Unity.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config")]
    public class GameConfigSO : ScriptableObject
    {
        [Header("状态配置")]
        [SerializeField] private float countdownDuration = 3f;
        [SerializeField] private float gameOverDisplayDuration = 5f;

        [Header("匹配配置")]
        [SerializeField] private int minPlayers = 2;
        [SerializeField] private int maxPlayers = 4;
        [SerializeField] private float matchmakingTimeout = 60f;

        [Header("分数配置")]
        [SerializeField] private int killScore = 100;
        [SerializeField] private int deathPenalty = -50;
        [SerializeField] private int survivalBonus = 10;

        [Header("规则配置")]
        [SerializeField] private float timeLimit = 300f;
        [SerializeField] private int scoreToWin = 1000;
        [SerializeField] private bool enableRespawn = true;
        [SerializeField] private float respawnDelay = 3f;

        public GameConfig ToGameConfig()
        {
            return new GameConfig
            {
                StateConfig = new StateConfig
                {
                    CountdownDuration = countdownDuration,
                    GameOverDisplayDuration = gameOverDisplayDuration
                },
                MatchConfig = new MatchConfig
                {
                    MinPlayers = minPlayers,
                    MaxPlayers = maxPlayers,
                    MatchmakingTimeout = matchmakingTimeout
                },
                ScoreConfig = new ScoreConfig
                {
                    KillScore = killScore,
                    DeathPenalty = deathPenalty,
                    SurvivalBonus = survivalBonus
                },
                RuleConfig = new RuleConfig
                {
                    TimeLimit = timeLimit,
                    ScoreToWin = scoreToWin,
                    EnableRespawn = enableRespawn,
                    RespawnDelay = respawnDelay
                }
            };
        }
    }
} 