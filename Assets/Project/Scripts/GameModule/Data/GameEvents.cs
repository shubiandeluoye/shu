using System.Collections.Generic;
using Fusion;

namespace GameModule.Data
{
    #region 游戏核心事件
    public struct GameStartEvent
    {
        public float StartTime;
    }

    public struct GameEndEvent
    {
        public GameOverReason Reason;
        public Dictionary<PlayerRef, PlayerScore> FinalScores;
    }

    public struct GamePauseEvent
    {
        public bool IsPaused;
    }

    public struct GameStateChangeEvent
    {
        public GameState OldState;
        public GameState NewState;
    }

    public struct GameCountdownEvent
    {
        public float Duration;
    }
    #endregion

    #region 玩家事件
    public struct PlayerJoinEvent
    {
        public PlayerRef PlayerRef;
        public string PlayerName;
    }

    public struct PlayerLeaveEvent
    {
        public PlayerRef PlayerRef;
    }

    public struct PlayerDeathEvent
    {
        public PlayerRef Victim;
        public PlayerRef Killer;
        public string DeathReason;
    }
    #endregion

    #region 匹配事件
    public struct MatchmakingStartEvent
    {
        public float Timeout;
    }

    public struct MatchmakingStopEvent {}

    public struct MatchReadyEvent {}

    public struct MatchTimeoutEvent {}

    public struct PlayerCountChangeEvent
    {
        public int CurrentCount;
        public int MaxCount;
    }
    #endregion

    #region 分数事件
    public struct ScoreUpdateEvent
    {
        public PlayerRef PlayerRef;
        public int NewScore;
        public int ScoreChange;
        public string Reason;
    }

    public struct GameOverEvent
    {
        public GameOverReason Reason;
    }
    #endregion

    #region 排行榜事件
    public struct RankingChangeEvent
    {
        public PlayerRef PlayerRef;
        public int OldRank;
        public int NewRank;
        public int Score;
    }
    #endregion
} 