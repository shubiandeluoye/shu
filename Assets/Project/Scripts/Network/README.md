# Network 模块说明文档

## 模块结构概览

### InGame/ - 游戏内同步系统
- [NetworkBulletSync.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/InGame/NetworkBulletSync.cs)
  - 负责所有子弹的网络同步
  - 处理子弹的生成、移动和销毁
  - 管理子弹碰撞的网络同步

- [NetworkCenterSync.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/InGame/NetworkCenterSync.cs)
  - 同步地图中心点状态
  - 处理中心点占领逻辑
  - 管理得分区域状态

- [NetworkEffectSync.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/InGame/NetworkEffectSync.cs)
  - 同步所有视觉特效
  - 处理粒子系统的网络同步
  - 管理音效的网络播放

- [NetworkPlayerSync.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/InGame/NetworkPlayerSync.cs)
  - 处理玩家位置和旋转同步
  - 管理玩家状态更新
  - 处理玩家输入同步

- [NetworkSkillSync.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/InGame/NetworkSkillSync.cs)
  - 同步技能释放状态
  - 处理技能效果的网络表现
  - 管理技能冷却和持续时间

- [NetworkSpawnSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/InGame/NetworkSpawnSystem.cs)
  - 管理网络对象的生成
  - 处理预制体的实例化
  - 控制对象池的网络同步

- [NetworkStateSync.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/InGame/NetworkStateSync.cs)
  - 同步游戏整体状态
  - 处理状态切换的网络广播
  - 管理状态机的网络同步

- [NetworkTeamSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/InGame/NetworkTeamSystem.cs)
  - 管理队伍分配
  - 处理队伍切换
  - 同步队伍状态

### Lobby/ - 大厅系统
- [NetworkMapSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Lobby/NetworkMapSystem.cs)
  - 管理地图选择和加载
  - 处理地图投票
  - 同步地图状态

- [NetworkMatchmaking.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Lobby/NetworkMatchmaking.cs)
  - 处理玩家匹配
  - 管理匹配队列
  - 控制匹配规则

- [NetworkReconnect.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Lobby/NetworkReconnect.cs)
  - 处理断线重连
  - 管理重连超时
  - 同步重连状态

- [NetworkRoomSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Lobby/NetworkRoomSystem.cs)
  - 管理房间创建和销毁
  - 处理玩家加入/离开
  - 控制房间设置

- [NetworkSaveSync.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Lobby/NetworkSaveSync.cs)
  - 同步游戏存档
  - 处理存档冲突
  - 管理存档版本

- [NetworkScoreSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Lobby/NetworkScoreSystem.cs)
  - 管理分数计算
  - 同步计分板
  - 处理排行榜

### Types/ - 类型定义
- [BulletType.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Types/BulletType.cs)
```csharp
public enum BulletType
{
    Normal,
    Piercing,
    Explosive,
    Bounce
}
```

- [NetworkActionType.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Types/NetworkActionType.cs)
```csharp
public enum NetworkActionType
{
    Move,
    Shoot,
    UseSkill,
    TakeDamage,
    Die,
    Respawn
}
```

- [NetworkErrorType.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Types/NetworkErrorType.cs)
```csharp
public enum NetworkErrorType
{
    ConnectionFailed,
    Timeout,
    AuthenticationFailed,
    ServerFull,
    InvalidOperation
}
```

- [NetworkEventTypes.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Types/NetworkEventTypes.cs)
```csharp
public enum NetworkEventTypes
{
    PlayerJoined,
    PlayerLeft,
    GameStarted,
    GameEnded,
    RoundStart,
    RoundEnd
}
```

- [NetworkGameMode.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Types/NetworkGameMode.cs)
```csharp
public enum NetworkGameMode
{
    FreeForAll,
    TeamDeathmatch,
    CapturePoint,
    Custom
}
```

- [NetworkInputData.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Types/NetworkInputData.cs)
```csharp
public struct NetworkInputData
{
    public Vector3 Movement;
    public Vector2 Rotation;
    public bool IsShooting;
    public bool IsUsingSkill;
    public int SkillIndex;
    public float Timestamp;
}
```

- [PlatformNetworkTypes.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Types/PlatformNetworkTypes.cs)
```csharp
public enum PlatformNetworkType
{
    Steam,
    Epic,
    Custom
}
```

### Utils/ - 工具类
- [NetworkConstants.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Utils/NetworkConstants.cs)
  - 定义网络常量
  - 配置默认参数
  - 设置超时时间

- [NetworkDebugger.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Utils/NetworkDebugger.cs)
  - 网络调试工具
  - 日志记录系统
  - 性能监控

- [NetworkHelper.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Utils/NetworkHelper.cs)
  - 网络工具函数
  - 数据转换工具
  - 网络状态检查

- [NetworkMetrics.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/Utils/NetworkMetrics.cs)
  - 网络性能指标
  - 数据统计
  - 性能分析

### Root/ - 核心管理
- [NetworkManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Network/NetworkManager.cs)
  - 网络系统总管理器
  - 初始化网络服务
  - 管理网络生命周期

## 网络配置详解

### 1. 连接配置
```csharp
[System.Serializable]
public class NetworkConfig
{
    public float SyncRate = 20f;            // 同步频率
    public float InterpolationTime = 0.1f;  // 插值时间
    public int MaxPlayers = 10;             // 最大玩家数
    public float DisconnectTimeout = 10f;   // 断开超时
    public bool UseWebSockets = true;       // 使用WebSocket
    public string[] RelayServers;           // 中继服务器
    public NetworkCompression Compression;   // 压缩设置
    public NetworkQuality QualitySettings;   // 质量设置
}
```

### 2. 房间配置
```csharp
[System.Serializable]
public class RoomConfig
{
    public string RoomName;                 // 房间名称
    public int MaxPlayers;                  // 最大玩家数
    public bool IsPrivate;                  // 是否私有
    public string Password;                 // 房间密码
    public NetworkGameMode GameMode;        // 游戏模式
    public string MapName;                  // 地图名称
    public RoomRules CustomRules;           // 自定义规则
    public TeamConfig[] Teams;              // 队伍配置
}
```

## 网络同步实现

### 1. 位置同步
```csharp
public class NetworkPlayerSync : NetworkBehaviour
{
    [SerializeField] private float positionThreshold = 0.1f;
    [SerializeField] private float rotationThreshold = 1f;
    
    private NetworkVariable<Vector3> netPosition = new();
    private NetworkVariable<Quaternion> netRotation = new();
    
    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 newPos, ServerRpcParams rpcParams = default)
    {
        if (Vector3.Distance(netPosition.Value, newPos) > positionThreshold)
        {
            netPosition.Value = newPos;
            UpdatePositionClientRpc(newPos);
        }
    }
    
    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPos)
    {
        if (!IsOwner)
        {
            StartCoroutine(SmoothMove(newPos));
        }
    }
}
```

### 2. 状态同步
```csharp
public class NetworkStateSync : NetworkBehaviour
{
    private NetworkVariable<GameState> gameState = new();
    private NetworkVariable<float> gameTime = new();
    
    public void UpdateGameState(GameState newState)
    {
        if (IsServer)
        {
            gameState.Value = newState;
            OnGameStateChanged(newState);
        }
    }
    
    [ClientRpc]
    private void OnGameStateChanged(GameState newState)
    {
        // 处理状态改变
        HandleStateChange(newState);
    }
}
```

### 3. 技能同步
```csharp
public class NetworkSkillSync : NetworkBehaviour
{
    [SerializeField] private float syncInterval = 0.1f;
    private NetworkVariable<SkillState> skillState = new();
    
    [ServerRpc]
    public void CastSkillServerRpc(int skillId, Vector3 direction)
    {
        // 验证技能
        if (!ValidateSkill(skillId)) return;
        
        // 更新技能状态
        skillState.Value = new SkillState(skillId, direction);
        
        // 广播给其他客户端
        CastSkillClientRpc(skillId, direction);
    }
    
    [ClientRpc]
    private void CastSkillClientRpc(int skillId, Vector3 direction)
    {
        if (!IsOwner)
        {
            PlaySkillEffects(skillId, direction);
        }
    }
}
```

## 调试工具

### NetworkDebugger
```csharp
public class NetworkDebugger : MonoBehaviour
{
    public bool ShowNetworkStats = true;
    public bool LogNetworkEvents = true;
    
    private void OnGUI()
    {
        if (ShowNetworkStats)
        {
            GUI.Label(new Rect(10, 10, 200, 20), $"Ping: {NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt()} ms");
            GUI.Label(new Rect(10, 30, 200, 20), $"Connected Players: {NetworkManager.Singleton.ConnectedClients.Count}");
            GUI.Label(new Rect(10, 50, 200, 20), $"Is Host: {NetworkManager.Singleton.IsHost}");
        }
    }
}
```

### NetworkMetrics
```csharp
public class NetworkMetrics : MonoBehaviour
{
    public struct NetworkStats
    {
        public float BytesSent;
        public float BytesReceived;
        public float PacketLoss;
        public float Latency;
    }
    
    public NetworkStats GetCurrentStats()
    {
        return new NetworkStats
        {
            BytesSent = NetworkManager.Singleton.NetworkMetrics.BytesSent,
            BytesReceived = NetworkManager.Singleton.NetworkMetrics.BytesReceived,
            PacketLoss = CalculatePacketLoss(),
            Latency = GetAverageLatency()
        };
    }
}
```

## 性能优化

### 1. 数据压缩
- 使用增量更新
- 压缩向量数据
- 使用固定点数
- 优化枚举大小

### 2. 带宽优化
- 调整同步频率
- 使用插值
- 实现预测
- 优化同步范围

### 3. CPU优化
- 使用对象池
- 批量处理更新
- 优化物理检测
- 减少GC

### 4. 内存优化
- 复用网络消息
- 优化预制体
- 控制对象数量
- 使用对象池

## 常见问题解决

### 1. 连接问题
- 症状：无法连接到服务器
- 解决方案：
  1. 检查网络连接
  2. 验证服务器地址
  3. 确认防火墙设置
  4. 检查版本匹配
  5. 尝试不同的传输协议

### 2. 同步问题
- 症状：位置同步不准确
- 解决方案：
  1. 调整同步频率
  2. 优化插值参数
  3. 检查网络延迟
  4. 验证预测设置
  5. 调整阈值设置

### 3. 断线问题
- 症状：频繁断线
- 解决方案：
  1. 检查网络稳定性
  2. 调整超时设置
  3. 实现断线重连
  4. 优化网络包大小
  5. 使用心跳包

### 4. 性能问题
- 症状：游戏卡顿
- 解决方案：
  1. 优化同步频率
  2. 使用对象池
  3. 实现预测
  4. 优化物理检测
  5. 控制同步对象数量

## 禁止修改项

### 1. 核心网络架构
- NetworkManager基础结构
- RPC调用机制
- 网络变量系统
- 同步基础逻辑
- 安全验证机制

### 2. 安全机制
- 权限验证
- 反作弊系统
- 加密机制
- 数据验证
- 状态检查

### 3. 网络事件系统
- 事件定义
- 事件分发
- 回调机制
- 事件队列
- 优先级系统

## 可以修改项

### 1. 同步配置
- 同步频率
- 插值参数
- 阈值设置
- 预测设置
- 压缩选项

### 2. 游戏逻辑
- 房间规则
- 匹配逻辑
- 队伍系统
- 分数计算
- 游戏模式

### 3. 网络优化
- 带宽优化
- 延迟补偿
- 状态压缩
- 预测算法
- 同步策略

## 使用示例

### 1. 创建房间
```csharp
public class GameLobby : MonoBehaviour
{
    private NetworkRoomSystem roomSystem;
    
    public async Task CreateRoom(RoomConfig config)
    {
        try
        {
            var result = await roomSystem.CreateRoom(config);
            if (result.Success)
            {
                Debug.Log($"Room created: {config.RoomName}");
                OnRoomCreated?.Invoke(result.RoomId);
            }
            else
            {
                Debug.LogError($"Failed to create room: {result.Error}");
            }
        }
        catch (NetworkException e)
        {
            Debug.LogError($"Network error: {e.Message}");
        }
    }
}
```

### 2. 加入房间
```csharp
public class GameLobby : MonoBehaviour
{
    private NetworkRoomSystem roomSystem;
    
    public async Task JoinRoom(string roomId, string password = null)
    {
        try
        {
            var result = await roomSystem.JoinRoom(new JoinRoomRequest
            {
                RoomId = roomId,
                Password = password,
                PlayerData = GetPlayerData()
            });
            
            if (result.Success)
            {
                Debug.Log($"Joined room: {roomId}");
                OnRoomJoined?.Invoke(result.RoomData);
            }
            else
            {
                Debug.LogError($"Failed to join room: {result.Error}");
            }
        }
        catch (NetworkException e)
        {
            Debug.LogError($"Network error: {e.Message}");
        }
    }
}
```

### 3. 同步玩家状态
```csharp
public class PlayerController : NetworkBehaviour
{
    private NetworkStateSync stateSync;
    private NetworkVariable<PlayerState> playerState = new();
    
    private void Update()
    {
        if (!IsOwner) return;
        
        var newState = new PlayerState
        {
            Position = transform.position,
            Rotation = transform.rotation,
            Health = currentHealth,
            CurrentWeapon = weaponController.CurrentWeapon,
            IsMoving = movementController.IsMoving
        };
        
        if (HasStateChanged(newState))
        {
            UpdatePlayerStateServerRpc(newState);
        }
    }
    
    [ServerRpc]
    private void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        // 服务器验证
        if (!ValidateState(newState)) return;
        
        // 更新状态
        playerState.Value = newState;
        
        // 通知其他客户端
        UpdatePlayerStateClientRpc(newState);
    }
    
    [ClientRpc]
    private void UpdatePlayerStateClientRpc(PlayerState newState)
    {
        if (!IsOwner)
        {
            ApplyState(newState);
        }
    }
}
```

## 网络事件处理

### 1. 注册事件
```csharp
public class NetworkEventHandler : MonoBehaviour
{
    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    }
    
    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        }
    }
}
```

### 2. 处理事件
```csharp
public class NetworkEventHandler : MonoBehaviour
{
    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");
        // 处理客户端连接
        OnClientConnected?.Invoke(clientId);
    }
    
    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");
        // 处理客户端断开
        OnClientDisconnected?.Invoke(clientId);
    }
    
    private void HandleServerStarted()
    {
        Debug.Log("Server started");
        // 处理服务器启动
        OnServerStarted?.Invoke();
    }
}
```

## 网络调试

### 1. 启用调试日志
```csharp
public class NetworkDebugger : MonoBehaviour
{
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool enableMetricsCollection = true;
    
    private void Start()
    {
        if (enableDebugLogs)
        {
            NetworkManager.Singleton.LogLevel = LogLevel.Developer;
        }
        
        if (enableMetricsCollection)
        {
            NetworkManager.Singleton.NetworkMetrics.EnableMetrics();
        }
    }
}
```

### 2. 性能监控
```csharp
public class NetworkPerformanceMonitor : MonoBehaviour
{
    [SerializeField] private float updateInterval = 1f;
    private NetworkMetrics metrics;
    
    private void Update()
    {
        if (Time.time % updateInterval < Time.deltaTime)
        {
            var stats = metrics.GetCurrentStats();
            Debug.Log($"Network Stats:\n" +
                     $"Bytes Sent: {stats.BytesSent}\n" +
                     $"Bytes Received: {stats.BytesReceived}\n" +
                     $"Packet Loss: {stats.PacketLoss}%\n" +
                     $"Latency: {stats.Latency}ms");
        }
    }
}
```

## 注意事项

1. 网络安全
   - 始终验证客户端输入
   - 使用加密传输敏感数据
   - 实现反作弊机制
   - 保护服务器免受DDoS攻击
   - 定期更新安全策略

2. 性能优化
   - 合理设置同步频率
   - 使用适当的压缩算法
   - 实现预测和插值
   - 优化网络包大小
   - 使用对象池管理

3. 可扩展性
   - 使用模块化设计
   - 实现可配置的同步策略
   - 支持自定义游戏模式
   - 提供扩展接口
   - 维护向后兼容性

4. 调试和维护
   - 保持详细的日志记录
   - 实现监控系统
   - 提供调试工具
   - 定期进行性能测试
   - 维护文档和注释
