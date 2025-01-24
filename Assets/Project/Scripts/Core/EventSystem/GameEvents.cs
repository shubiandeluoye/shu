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
} 