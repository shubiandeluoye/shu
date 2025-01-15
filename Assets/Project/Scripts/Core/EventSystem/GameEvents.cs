using Core.FSM;

namespace Core.EventSystem
{
    // 游戏状态改变事件
    public class GameStateChangedEvent
    {
        public GameState NewState { get; set; }
    }

    // 玩家受伤事件
    public class PlayerDamagedEvent
    {
        public float RemainingHealth { get; set; }
    }

    // 游戏结束事件
    public class GameEndEvent
    {
        public int WinnerId { get; set; }
    }

    // 玩家射击事件
    public class PlayerShootEvent
    {
        public int PlayerId { get; set; }
        public float Angle { get; set; }
    }
} 