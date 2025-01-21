namespace Core.FSM
{
    /// <summary>
    /// 游戏状态枚举
    /// 定义了游戏可能的所有状态
    /// </summary>
    public enum GameState
    {
        None = 0,
        
        // 系统状态
        Initializing = 1,
        Loading = 2,
        
        // 主菜单状态
        MainMenu = 10,
        Options = 11,
        Credits = 12,
        
        // 游戏状态
        Playing = 20,
        Paused = 21,
        GameOver = 22,
        
        // 战斗状态
        Combat = 30,
        NonCombat = 31,
        
        // 对话状态
        Dialogue = 40,
        Cutscene = 41,
        
        // 技能状态
        CastingSkill = 50,
        SkillCooldown = 51,
        
        // 特殊状态
        Transition = 90,
        Error = 99
    }

    // 添加状态分类
    [System.Flags]
    public enum GameStateCategory
    {
        None = 0,
        System = 1 << 0,
        Menu = 1 << 1,
        Gameplay = 1 << 2,
        Combat = 1 << 3,
        Dialogue = 1 << 4,
        Skill = 1 << 5,
        Special = 1 << 6
    }
} 