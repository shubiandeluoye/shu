namespace Core.FSM
{
    /// <summary>
    /// 游戏状态枚举
    /// 定义了游戏可能的所有状态
    /// </summary>
    public enum GameState
    {
        MainMenu,   // 主菜单状态
        Playing,    // 游戏进行中状态
        Paused,     // 游戏暂停状态
        GameOver    // 游戏结束状态
    }
} 