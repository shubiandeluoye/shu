using System.Collections.Generic;

namespace GameModule.Core.Data
{
    public struct GameStartEvent
    {
        public float StartTime;
    }

    public struct GameEndEvent
    {
        public GameOverReason Reason;
        public Dictionary<int, PlayerScore> FinalScores;
    }

    public struct GamePauseEvent
    {
        public bool IsPaused;
    }

    public struct PlayerJoinEvent
    {
        public int PlayerId;
        public string PlayerName;
    }

    public struct PlayerLeaveEvent
    {
        public int PlayerId;
    }

    public struct PlayerDeathEvent
    {
        public int VictimId;
        public int KillerId;
        public string DeathReason;
    }

    #region 游戏模式事件
    public struct GameModeChangedEvent
    {
        public GameMode NewMode;
        public GameMode OldMode;
    }

    public struct RoundStartEvent
    {
        public int RoundNumber;
        public float RoundTime;
    }

    public struct RoundEndEvent
    {
        public int RoundNumber;
        public Dictionary<int, int> RoundScores;
    }

    public struct WarmupStartEvent
    {
        public float WarmupTime;
    }

    public struct SpectatorJoinEvent
    {
        public int SpectatorId;
        public int TargetPlayerId;
    }
    #endregion

    #region 游戏进度事件
    public struct GameProgressEvent
    {
        public float Progress;          // 0-1
        public GamePhase CurrentPhase;  // 早期/中期/后期
        public float RemainingTime;
    }

    public enum GamePhase
    {
        Early,
        Mid,
        Late,
        Final
    }
    #endregion
} 