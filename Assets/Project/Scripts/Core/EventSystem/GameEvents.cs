using Core.FSM;
using Fusion;
using UnityEngine;
using MapModule.Shapes;

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
        public NetworkId PlayerId { get; set; }
        public float Damage { get; set; }
        public float RemainingHealth { get; set; }
    }

    // 游戏结束事件
    public class GameEndEvent
    {
        public PlayerRef Winner { get; set; }
        public PlayerRef Loser { get; set; }
        public string EndReason { get; set; }
    }

    // 游戏开始事件
    public class GameStartEvent
    {
        public PlayerRef Player1 { get; set; }
        public PlayerRef Player2 { get; set; }
    }

    // 玩家射击事件
    public class PlayerShootEvent
    {
        public NetworkId PlayerId { get; set; }
        public float Angle { get; set; }
    }

    // 玩家生成事件
    public class PlayerSpawnedEvent
    {
        public PlayerRef PlayerId { get; set; }
        public UnityEngine.Vector3 Position { get; set; }
    }

    // 玩家销毁事件
    public class PlayerDespawnedEvent
    {
        public NetworkId PlayerId { get; set; }
    }

    // 中央区域状态改变事件
    public class CentralAreaStateChangedEvent
    {
        public CentralAreaState OldState { get; set; }
        public CentralAreaState NewState { get; set; }
        public float StateTime { get; set; }
        public int CollectedCount { get; set; }
    }

    // 中央区域状态枚举
    public enum CentralAreaState
    {
        Collecting,
        Charging,
        Firing
    }

    // 形状状态网络同步事件
    public class NetworkShapeStateEvent
    {
        public ShapeState State { get; set; }
        public int ShapeId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }

    // 网络地图系统初始化事件
    public class NetworkMapSystemInitializedEvent
    {
        public bool IsInitialized { get; set; }
        public NetworkId SystemId { get; set; }
    }

    // 网络中心区域事件
    public class NetworkCentralAreaEvent
    {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public bool IsActive { get; set; }
    }

    public enum GameEvents
    {
        // 系统事件
        None = 0,
        SystemInitialize = 1,
        SystemReady = 2,
        SystemError = 3,
        
        // 状态事件
        StateChanged = 10,
        StateEnter = 11,
        StateExit = 12,
        StatePaused = 13,
        StateResumed = 14,
        
        // 对象池事件
        PoolCreated = 20,
        PoolDestroyed = 21,
        ObjectSpawned = 22,
        ObjectRecycled = 23,
        PoolCleared = 24,
        
        // 技能系统事件
        SkillStarted = 30,
        SkillEnded = 31,
        SkillCancelled = 32,
        SkillCooldownStart = 33,
        SkillCooldownEnd = 34,
        
        // 资源事件
        ResourceLoaded = 40,
        ResourceUnloaded = 41,
        ResourceError = 42,
        
        // 网络事件
        NetworkConnected = 50,
        NetworkDisconnected = 51,
        NetworkError = 52,
        
        // 自定义事件起始值
        Custom = 1000
    }
} 