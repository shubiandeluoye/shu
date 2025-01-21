using UnityEngine;
using Fusion;

namespace Core.Network
{
    /// <summary>
    /// 中央区域同步组件
    /// 负责：
    /// 1. 中央形状状态同步
    /// 2. 形状变换同步
    /// 3. 子弹收集计数
    /// 4. 形状效果触发
    /// </summary>
    public class NetworkCenterSync : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<int, CenterData> GameCenters { get; }
        [Networked] private NetworkDictionary<int, CenterState> CenterStates { get; }
        [Networked] private int NextCenterId { get; set; }

        public struct CenterData : INetworkStruct
        {
            public Vector3 Position;
            public int TeamId;
            public float Radius;
            public float CaptureProgress;
            public NetworkBool IsActive;
            public byte CenterType;
        }

        public struct CenterState : INetworkStruct
        {
            [Networked, Capacity(8)]
            public NetworkArray<int> PlayerCount => default;
            
            public int ControllingTeam;
            public float LastUpdateTime;
            public NetworkBool IsContested;
            public float ContestStartTime;
        }

        public int RegisterCenter(Vector3 position, float radius, byte centerType)
        {
            if (!Runner.IsServer) return -1;

            int centerId = NextCenterId++;
            var centerData = new CenterData
            {
                Position = position,
                TeamId = -1,  // 初始无归属
                Radius = radius,
                CaptureProgress = 0f,
                IsActive = true,
                CenterType = centerType
            };

            var centerState = new CenterState
            {
                ControllingTeam = -1,
                LastUpdateTime = Runner.SimulationTime,
                IsContested = false,
                ContestStartTime = 0f
            };

            GameCenters.Set(centerId, centerData);
            CenterStates.Set(centerId, centerState);
            RPC_OnCenterRegistered(centerId, centerData, centerState);

            return centerId;
        }

        public void UpdateCenterPlayerCount(int centerId, int teamId, int delta)
        {
            if (!Runner.IsServer) return;
            if (!CenterStates.TryGet(centerId, out var state)) return;

            // 直接修改数组元素，而不是整个数组
            var playerCounts = state.PlayerCount;
            playerCounts[teamId] = Mathf.Max(0, playerCounts[teamId] + delta);
            
            state.LastUpdateTime = Runner.SimulationTime;
            UpdateContestState(centerId, state);
            CenterStates.Set(centerId, state);
            RPC_OnCenterPlayerCountUpdated(centerId, teamId, playerCounts[teamId]);
        }

        private void UpdateContestState(int centerId, CenterState state)
        {
            int dominantTeam = -1;
            int teamsPresent = 0;

            for (int i = 0; i < state.PlayerCount.Length; i++)
            {
                if (state.PlayerCount[i] > 0)
                {
                    teamsPresent++;
                    dominantTeam = i;
                }
            }

            state.IsContested = teamsPresent > 1;
            if (state.IsContested && !CenterStates.TryGet(centerId, out var oldState))
            {
                state.ContestStartTime = Runner.SimulationTime;
            }
            else if (!state.IsContested)
            {
                state.ControllingTeam = teamsPresent == 1 ? dominantTeam : -1;
            }
        }

        public void UpdateCenterProgress(int centerId, float progress)
        {
            if (!Runner.IsServer) return;
            if (!GameCenters.TryGet(centerId, out var data)) return;

            data.CaptureProgress = Mathf.Clamp01(progress);
            GameCenters.Set(centerId, data);
            RPC_OnCenterProgressUpdated(centerId, progress);
        }

        public void SetCenterTeam(int centerId, int teamId)
        {
            if (!Runner.IsServer) return;
            if (!GameCenters.TryGet(centerId, out var data)) return;

            data.TeamId = teamId;
            GameCenters.Set(centerId, data);
            RPC_OnCenterTeamChanged(centerId, teamId);
        }

        public void SetCenterActive(int centerId, bool active)
        {
            if (!Runner.IsServer) return;
            if (!GameCenters.TryGet(centerId, out var data)) return;

            data.IsActive = active;
            GameCenters.Set(centerId, data);
            RPC_OnCenterActiveStateChanged(centerId, active);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer) return;

            foreach (var kvp in GameCenters)
            {
                int centerId = kvp.Key;
                var centerData = kvp.Value;
                
                if (!centerData.IsActive) continue;
                if (!CenterStates.TryGet(centerId, out var state)) continue;

                // 更新占领进度
                if (!state.IsContested && state.ControllingTeam >= 0)
                {
                    float newProgress = centerData.CaptureProgress;
                    if (state.ControllingTeam != centerData.TeamId)
                    {
                        newProgress += Runner.DeltaTime * GetCaptureRate(centerData.CenterType);
                    }
                    
                    if (newProgress >= 1f)
                    {
                        SetCenterTeam(centerId, state.ControllingTeam);
                        newProgress = 0f;
                    }
                    
                    if (newProgress != centerData.CaptureProgress)
                    {
                        UpdateCenterProgress(centerId, newProgress);
                    }
                }
            }
        }

        private float GetCaptureRate(byte centerType)
        {
            // 根据中心点类型返回不同的占领速率
            return 0.1f;
        }

        // 数据访问方法
        public CenterData? GetCenterData(int centerId)
        {
            return GameCenters.TryGet(centerId, out var data) ? data : null;
        }

        public CenterState? GetCenterState(int centerId)
        {
            return CenterStates.TryGet(centerId, out var state) ? state : null;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnCenterRegistered(int centerId, CenterData data, CenterState state)
        {
            GameCenters.Set(centerId, data);
            CenterStates.Set(centerId, state);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnCenterPlayerCountUpdated(int centerId, int teamId, int count)
        {
            if (CenterStates.TryGet(centerId, out var state))
            {
                // 直接修改数组元素
                var playerCounts = state.PlayerCount;
                playerCounts[teamId] = count;
                
                state.LastUpdateTime = Runner.SimulationTime;
                CenterStates.Set(centerId, state);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnCenterProgressUpdated(int centerId, float progress)
        {
            if (GameCenters.TryGet(centerId, out var data))
            {
                data.CaptureProgress = progress;
                GameCenters.Set(centerId, data);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnCenterTeamChanged(int centerId, int teamId)
        {
            if (GameCenters.TryGet(centerId, out var data))
            {
                data.TeamId = teamId;
                GameCenters.Set(centerId, data);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnCenterActiveStateChanged(int centerId, NetworkBool active)
        {
            if (GameCenters.TryGet(centerId, out var data))
            {
                data.IsActive = active;
                GameCenters.Set(centerId, data);
            }
        }
    }
} 