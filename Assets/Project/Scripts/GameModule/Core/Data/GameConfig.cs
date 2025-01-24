[System.Serializable]
public class GameConfig
{
    public StateConfig StateConfig;
    public MatchConfig MatchConfig;
    public ScoreConfig ScoreConfig;
    public RuleConfig RuleConfig;
    public GameModeConfig GameModeConfig;
}

[System.Serializable]
public class GameModeConfig
{
    public GameMode GameMode = GameMode.Normal;
    public bool EnablePracticeMode = false;    // 练习模式
    public bool EnableSpectatorMode = true;    // 观战模式
    public bool AllowLateJoin = true;         // 允许中途加入
    public float WarmupTime = 30f;            // 热身时间
    public float RoundTime = 180f;            // 每轮时间
    public int RoundCount = 3;                // 总轮数
}

public class RuleConfig
{
    public float TimeLimit = 300f;        // 游戏时间限制
    public int ScoreToWin = 1000;         // 胜利分数
    public int MinScoreToEnd = -500;      // 失败分数
    public bool EnableRespawn = true;     // 是否允许重生
    public float RespawnDelay = 3f;       // 重生延迟
    public int MaxDeaths = 3;             // 最大死亡次数
    public float OutOfBoundsDamage = 10f; // 出界伤害
    public bool EnableTeamMode = false;   // 是否开启队伍模式
    public int TeamsCount = 2;            // 队伍数量
} 