using UnityEngine;

namespace GameModule.Data
{
    [System.Serializable]
    public class GameConfig
    {
        public StateConfig StateConfig;
        public MatchConfig MatchConfig;
        public ScoreConfig ScoreConfig;
        public RuleConfig RuleConfig;
    }

    [System.Serializable]
    public class StateConfig
    {
        public float CountdownDuration = 3f;
        public float GameOverDisplayDuration = 5f;
    }

    [System.Serializable]
    public class MatchConfig
    {
        public int MinPlayers = 2;
        public int MaxPlayers = 4;
        public float MatchmakingTimeout = 60f;
    }

    [System.Serializable]
    public class ScoreConfig
    {
        public int KillScore = 100;
        public int DeathPenalty = -50;
        public int SurvivalBonus = 10;
    }

    [System.Serializable]
    public class RuleConfig
    {
        public float TimeLimit = 300f;        // 5分钟
        public int ScoreToWin = 1000;
        public bool EnableRespawn = true;
        public float RespawnDelay = 3f;
    }
} 