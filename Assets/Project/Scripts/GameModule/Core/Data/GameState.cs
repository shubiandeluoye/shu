public enum GameOverReason
{
    None,
    TimeUp,                // 时间到
    VictoryConditionMet,   // 达到胜利分数
    NotEnoughPlayers,      // 玩家不足
    ScoreTooLow,          // 分数过低
    MaxDeathsReached,     // 达到最大死亡次数
    TeamVictory,          // 队伍胜利
    OutOfBounds,          // 出界死亡
    ServerError           // 服务器错误
}

public enum GameState
{
    None,
    WaitingForPlayers,
    Warmup,           // 新增：热身
    Countdown,
    RoundStarting,    // 新增：回合开始
    Playing,
    RoundEnding,      // 新增：回合结束
    Paused,
    GameOver
}

public enum GameMode
{
    Normal,           // 普通模式
    Practice,         // 练习模式
    Tournament,       // 比赛模式
    Custom            // 自定义模式
}

public class RoundState
{
    public int CurrentRound;
    public float RoundTime;
    public Dictionary<int, int> RoundScores;  // 每轮分数
} 