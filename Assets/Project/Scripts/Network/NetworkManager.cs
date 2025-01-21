using UnityEngine;
using UnityEngine.SceneManagement;  // 添加场景管理引用
using Fusion;
using System.Threading.Tasks;
using System.Collections.Generic;
using Fusion.Sockets;
using System;
using Core.Singleton;
using Core.EventSystem;
using Network.Utils;
using System.Linq;
using Core.Network;  // 添加类型引用
using MapModule.Shapes;  // 添加这个引用

namespace Core.Network
{
    /// <summary>
    /// 网络总管理器
    /// 负责：
    /// 1. 网络系统的初始化和关闭
    /// 2. 管理所有网络子系统
    /// 3. 提供网络状态查询
    /// 4. 确保服务器权威
    /// </summary>
    public class NetworkManager : Singleton<NetworkManager>, INetworkRunnerCallbacks
    {
        [Header("Network Settings")]
        [SerializeField] private NetworkRunner networkRunner;
        [SerializeField] private NetworkSceneManagerDefault sceneManager;
        
        // 服务器权威状态
        public bool IsServer => networkRunner != null && networkRunner.IsServer;
        public bool IsClient => networkRunner != null && networkRunner.IsClient;
        public bool HasStateAuthority => networkRunner != null && networkRunner.IsServer;

        // 战斗中的网络组件
        public NetworkPlayerSync PlayerSync { get; private set; }
        public NetworkBulletSync BulletSync { get; private set; }
        public NetworkCenterSync CenterSync { get; private set; }
        public NetworkEffectSync EffectSync { get; private set; }
        public NetworkTeamSystem TeamSystem { get; private set; }
        public NetworkSkillSync SkillSync { get; private set; }
        public NetworkStateSync StateSync { get; private set; }

        // 大厅网络组件
        public NetworkRoomSystem RoomSystem { get; private set; }
        public NetworkMatchmaking Matchmaking { get; private set; }
        public NetworkMapSystem MapSystem { get; private set; }
        public NetworkSaveSync SaveSync { get; private set; }
        public NetworkScoreSystem ScoreSystem { get; private set; }
        public NetworkReconnect Reconnect { get; private set; }

        // 新增组件引用
        public NetworkSpawnSystem SpawnSystem { get; private set; }
        public NetworkDebugger Debugger { get; private set; }
        public NetworkMetrics Metrics { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            InitializeNetworkComponents();
            StartPerformanceMonitoring();
            RegisterMapEvents();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            CancelAllOperations();
            StopPerformanceMonitoring();
        }

        /// <summary>
        /// 初始化网络组件，确保服务器权威
        /// </summary>
        private void InitializeNetworkComponents()
        {
            ExecuteOnMainThread(() =>
            {
                // 初始化Runner
                networkRunner = gameObject.AddComponent<NetworkRunner>();
                networkRunner.ProvideInput = true;

                // 初始化战斗组件
                PlayerSync = gameObject.AddComponent<NetworkPlayerSync>();
                BulletSync = gameObject.AddComponent<NetworkBulletSync>();
                CenterSync = gameObject.AddComponent<NetworkCenterSync>();
                EffectSync = gameObject.AddComponent<NetworkEffectSync>();
                TeamSystem = gameObject.AddComponent<NetworkTeamSystem>();
                SkillSync = gameObject.AddComponent<NetworkSkillSync>();
                StateSync = gameObject.AddComponent<NetworkStateSync>();

                // 初始化大厅组件
                RoomSystem = gameObject.AddComponent<NetworkRoomSystem>();
                Matchmaking = gameObject.AddComponent<NetworkMatchmaking>();
                MapSystem = gameObject.AddComponent<NetworkMapSystem>();
                SaveSync = gameObject.AddComponent<NetworkSaveSync>();
                ScoreSystem = gameObject.AddComponent<NetworkScoreSystem>();
                Reconnect = gameObject.AddComponent<NetworkReconnect>();

                // 初始化新组件
                SpawnSystem = gameObject.AddComponent<NetworkSpawnSystem>();
                Debugger = gameObject.AddComponent<NetworkDebugger>();
                Debugger.Initialize(networkRunner);
                Metrics = gameObject.AddComponent<NetworkMetrics>();
            });
        }

        /// <summary>
        /// 启动游戏会话
        /// </summary>
        private async Task StartGame(GameMode mode, string roomName)
        {
            try
            {
                // 使用 SceneRef，这是 Fusion 最新版本推荐的方式
                var sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
                
                var args = new StartGameArgs()
                {
                    GameMode = mode,
                    SessionName = roomName,
                    Scene = sceneRef,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
                };

                await Task.Run(() => {
                    Debug.Log($"[NetworkManager] Starting {mode} game, room: {roomName}");
                    return Task.CompletedTask;
                });

                var result = await networkRunner.StartGame(args);
                if (!result.Ok)
                {
                    Debug.LogError($"[NetworkManager] Failed to start game: {result.ShutdownReason}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NetworkManager] Error starting game: {e.Message}");
            }
        }

        /// <summary>
        /// 关闭网络会话
        /// </summary>
        public async Task ShutdownNetwork()
        {
            await ExecuteAsyncOperation(async (token) =>
            {
                if (networkRunner != null)
                {
                    await networkRunner.Shutdown();
                }
            }, "ShutdownNetwork");
        }

        /// <summary>
        /// 验证操作权限
        /// </summary>
        public bool ValidateAuthority(NetworkObject networkObject)
        {
            return HasStateAuthority && networkObject.HasStateAuthority;
        }

        // INetworkRunnerCallbacks 实现
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;

            if (PlayerSync != null)
            {
                Vector3 spawnPosition = GetSpawnPosition();
                int teamId = AssignTeam(player);
                PlayerSync.RegisterPlayer(player, spawnPosition, teamId);
                
                if (SaveSync != null)
                {
                    SaveSync.InitializePlayerSave(player);
                }
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;

            if (PlayerSync != null)
            {
                PlayerSync.RemovePlayer(player);
            }
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"[Network] Shutdown: {shutdownReason}");
            // 清理网络状态
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            // 对象进入玩家的感兴趣区域
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            // 对象离开玩家的感兴趣区域
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("[Network] Connected to server");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            HandleNetworkError(NetworkErrorType.ConnectionFailed, 
                $"Failed to connect to {remoteAddress}: {reason}");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            // 处理连接请求
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            // 处理自定义认证响应
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Debug.Log($"[Network] Disconnected from server: {reason}");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            // 处理主机迁移
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            // 处理网络输入
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            // 处理丢失的输入
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            // 处理可靠数据接收
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            // 处理可靠数据传输进度
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log("[Network] Scene load completed");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("[Network] Scene load started");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            // 处理会话列表更新
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            // 处理用户模拟消息
        }

        // 错误处理
        private void HandleNetworkError(NetworkErrorType errorType, string message)
        {
            Debug.LogError($"[Network] Error: {errorType} - {message}");
            // provider?.HandleError(errorType, message);  // 注释掉或删除这行，除非你定义了 provider
        }

        // 性能监控
        public string GetNetworkStatus()
        {
            if (Metrics == null) return "Metrics not available";
            return Metrics.GetMetricsReport();
        }

        protected override void StopPerformanceMonitoring()
        {
            base.StopPerformanceMonitoring();  // 调用基类方法
            
            if (Metrics != null)
            {
                Destroy(Metrics);
            }
            if (Debugger != null)
            {
                Destroy(Debugger);
            }
        }

        protected override void Update()
        {
            base.Update();  // 调用基类的 Update
            // 你的其他 Update 逻辑
        }

        private void RegisterMapEvents()
        {
            EventManager.Instance.AddListener<NetworkShapeStateEvent>(OnNetworkShapeState);
            EventManager.Instance.AddListener<NetworkMapSystemInitializedEvent>(OnNetworkMapSystemInitialized);
            EventManager.Instance.AddListener<NetworkCentralAreaEvent>(OnNetworkCentralArea);
        }

        private void OnNetworkShapeState(NetworkShapeStateEvent evt)
        {
            if (!networkRunner.IsServer) return;
            
            // 通过 RPC 发送到所有客户端
            RPC_UpdateShapeState(evt.ShapeId, evt.Position, evt.Rotation, evt.Scale);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_UpdateShapeState(int shapeId, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            // 创建新的状态，使用完全限定名称
            var newState = new MapModule.Shapes.ShapeState
            {
                ShapeId = shapeId,
                Position = position,
                Rotation = rotation.eulerAngles.z,  // 这里是对的，因为 ShapeState.Rotation 是 float
                Scale = scale
            };

            // 触发事件
            EventManager.Instance.TriggerEvent(new ShapeStateChangedEvent 
            { 
                OldState = null,
                NewState = newState
            });
        }

        private void OnNetworkMapSystemInitialized(NetworkMapSystemInitializedEvent evt)
        {
            Debug.Log($"[NetworkManager] Map system initialized: {evt.IsInitialized}");
        }

        private void OnNetworkCentralArea(NetworkCentralAreaEvent evt)
        {
            Debug.Log($"[NetworkManager] Central area updated: {evt.Position}, {evt.Radius}");
        }

        private Vector3 GetSpawnPosition()
        {
            // 这里可以实现具体的出生点逻辑
            return Vector3.zero;  // 临时返回原点
        }

        private int AssignTeam(PlayerRef player)
        {
            // 这里可以实现具体的队伍分配逻辑
            return 0;  // 临时返回默认队伍
        }

        private void OnShapeStateChanged(MapModule.Shapes.ShapeStateChangedEvent evt)  // 使用完全限定名称
        {
            // ... 事件处理代码 ...
        }
    }

    public struct ShapeState
    {
        public int ShapeId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }
} 