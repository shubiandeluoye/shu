using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Core.Network
{
    /// <summary>
    /// 重连系统组件
    /// 负责：
    /// 1. 断线检测
    /// 2. 重连尝试
    /// 3. 会话恢复
    /// 4. 状态同步
    /// </summary>
    public class NetworkReconnect : NetworkBehaviour
    {
        private NetworkManager _networkManager;

        [Networked] private NetworkDictionary<PlayerRef, ReconnectData> DisconnectedPlayers { get; }
        [Networked] private float DisconnectTime { get; set; }
        [Networked] private NetworkBool IsReconnecting { get; set; }
        
        private const float MAX_RECONNECT_TIME = 30f;
        private const int MAX_RECONNECT_ATTEMPTS = 3;
        [SerializeField] private float _reconnectDelay = 5f;
        
        public struct ReconnectData : INetworkStruct
        {
            public NetworkString<_32> SessionId;
            public float DisconnectTimestamp;
            public int AttemptCount;
        }

        public override void Spawned()
        {
            _networkManager = NetworkManager.Instance;
            if (Runner.IsServer)
            {
                IsReconnecting = false;
            }
        }

        public void OnPlayerDisconnected(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            var reconnectData = new ReconnectData
            {
                SessionId = Runner.SessionInfo.Name,
                DisconnectTimestamp = Runner.SimulationTime,
                AttemptCount = 0
            };

            DisconnectedPlayers.Add(player, reconnectData);
            RPC_NotifyPlayerDisconnected(player);
        }

        public async Task<bool> TryReconnectAsync(NetworkRunner runner)
        {
            if (runner == null || !runner.IsRunning)
                return false;

            try
            {
                // 在新版本中使用不同的重连方式
                await runner.Shutdown();
                await Task.Delay((int)(_reconnectDelay * 1000));
                
                var result = await runner.StartGame(new StartGameArgs
                {
                    GameMode = runner.GameMode,
                    SessionName = runner.SessionInfo.Name,
                    SceneManager = runner.SceneManager
                });

                return result.Ok;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NetworkReconnect] Reconnection failed: {e.Message}");
                return false;
            }
        }

        public async void OnDisconnectedFromServer()
        {
            if (Runner == null) return;

            Debug.Log("[NetworkReconnect] Attempting to reconnect...");
            bool success = await TryReconnectAsync(Runner);
            
            if (!success)
            {
                Debug.LogError("[NetworkReconnect] Reconnection failed");
                // 处理重连失败的情况
            }
        }

        private async Task AttemptReconnection(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            try
            {
                // 在新版本中，我们需要使用不同的方式处理玩家重连
                if (DisconnectedPlayers.TryGet(player, out var reconnectData))
                {
                    // 等待一段时间后尝试恢复玩家状态
                    await Task.Delay((int)(_reconnectDelay * 1000));
                    RestorePlayerState(player);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NetworkReconnect] Failed to reconnect player {player}: {e.Message}");
            }
        }

        private void RestorePlayerState(PlayerRef player)
        {
            if (!Runner.IsServer) return;
            if (!DisconnectedPlayers.ContainsKey(player)) return;

            RPC_RestoreClientState(player);
            RemoveDisconnectedPlayer(player);
        }

        private void RemoveDisconnectedPlayer(PlayerRef player)
        {
            if (!Runner.IsServer) return;
            DisconnectedPlayers.Remove(player);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer) return;
            CheckReconnectTimeouts();
        }

        private void CheckReconnectTimeouts()
        {
            if (!Runner.IsServer) return;

            var playersToRemove = new List<PlayerRef>();
            foreach (var kvp in DisconnectedPlayers)
            {
                if (Runner.SimulationTime - kvp.Value.DisconnectTimestamp > MAX_RECONNECT_TIME)
                {
                    playersToRemove.Add(kvp.Key);
                }
            }

            foreach (var player in playersToRemove)
            {
                RemoveDisconnectedPlayer(player);
                RPC_NotifyReconnectTimeout(player);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_NotifyPlayerDisconnected(PlayerRef player)
        {
            // 通知玩家断线
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_RestoreClientState(PlayerRef player)
        {
            // 恢复客户端状态
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_NotifyReconnectTimeout(PlayerRef player)
        {
            // 通知重连超时
        }
    }
} 