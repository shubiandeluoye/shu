using Fusion;

namespace GameModule.Data
{
    public enum GameState
    {
        None,
        WaitingForPlayers,
        Countdown,
        Playing,
        Paused,
        GameOver
    }

    public enum GameOverReason
    {
        None,
        TimeUp,
        VictoryConditionMet,
        NotEnoughPlayers,
        ServerError,
        OutOfBounds
    }

    public class PlayerScore
    {
        public PlayerRef PlayerRef;
        public int Score;
        public int Kills;
        public int Deaths;
        public float SurvivalTime;
    }
} 