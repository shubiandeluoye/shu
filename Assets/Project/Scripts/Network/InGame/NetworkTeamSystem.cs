using UnityEngine;
using Fusion;
using System.Collections.Generic;

namespace Core.Network
{
    /// <summary>
    /// 队伍系统组件
    /// 负责：
    /// 1. 队伍分配
    /// 2. 队伍状态同步
    /// 3. 队伍分数统计
    /// 4. 2v2模式支持
    /// </summary>
    public class NetworkTeamSystem : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<int, TeamData> Teams { get; }
        [Networked] private NetworkDictionary<PlayerRef, int> PlayerTeams { get; }
        [Networked] private int MaxTeamSize { get; set; }
        [Networked] private int TeamCount { get; set; }

        public struct TeamData : INetworkStruct
        {
            public NetworkString<_32> TeamName;
            public int TeamScore;
            public NetworkBool IsLocked;
            public int CurrentSize;
        }

        public void InitializeTeams(int teamCount, int maxTeamSize)
        {
            if (!Runner.IsServer) return;

            TeamCount = teamCount;
            MaxTeamSize = maxTeamSize;

            for (int i = 0; i < teamCount; i++)
            {
                Teams.Set(i, new TeamData
                {
                    TeamName = $"Team {i + 1}",
                    TeamScore = 0,
                    IsLocked = false,
                    CurrentSize = 0
                });
            }

            RPC_OnTeamsInitialized(teamCount, maxTeamSize);
        }

        public bool AssignPlayerToTeam(PlayerRef player, int teamId)
        {
            if (!Runner.IsServer) return false;
            if (!Teams.TryGet(teamId, out var teamData)) return false;
            if (teamData.IsLocked || teamData.CurrentSize >= MaxTeamSize) return false;

            // 如果玩家已在某个队伍中，先移除
            if (PlayerTeams.TryGet(player, out var oldTeamId))
            {
                RemovePlayerFromTeam(player, oldTeamId);
            }

            // 更新队伍数据
            teamData.CurrentSize++;
            Teams.Set(teamId, teamData);
            PlayerTeams.Set(player, teamId);

            RPC_OnPlayerTeamAssigned(player, teamId);
            return true;
        }

        public void RemovePlayerFromTeam(PlayerRef player, int teamId)
        {
            if (!Runner.IsServer) return;
            if (!Teams.TryGet(teamId, out var teamData)) return;

            teamData.CurrentSize = Mathf.Max(0, teamData.CurrentSize - 1);
            Teams.Set(teamId, teamData);
            PlayerTeams.Remove(player);

            RPC_OnPlayerTeamRemoved(player, teamId);
        }

        public void UpdateTeamScore(int teamId, int score)
        {
            if (!Runner.IsServer) return;
            if (!Teams.TryGet(teamId, out var teamData)) return;

            teamData.TeamScore = score;
            Teams.Set(teamId, teamData);
            RPC_OnTeamScoreUpdated(teamId, score);
        }

        public void SetTeamLock(int teamId, bool locked)
        {
            if (!Runner.IsServer) return;
            if (!Teams.TryGet(teamId, out var teamData)) return;

            teamData.IsLocked = locked;
            Teams.Set(teamId, teamData);
            RPC_OnTeamLockChanged(teamId, locked);
        }

        public void SetTeamName(int teamId, NetworkString<_32> name)
        {
            if (!Runner.IsServer) return;
            if (!Teams.TryGet(teamId, out var teamData)) return;

            teamData.TeamName = name;
            Teams.Set(teamId, teamData);
            RPC_OnTeamNameChanged(teamId, name);
        }

        // 数据访问方法
        public TeamData? GetTeamData(int teamId)
        {
            return Teams.TryGet(teamId, out var data) ? data : null;
        }

        public int? GetPlayerTeam(PlayerRef player)
        {
            return PlayerTeams.TryGet(player, out var teamId) ? teamId : null;
        }

        public int GetTeamCount() => TeamCount;
        public int GetMaxTeamSize() => MaxTeamSize;

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnTeamsInitialized(int teamCount, int maxTeamSize)
        {
            TeamCount = teamCount;
            MaxTeamSize = maxTeamSize;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerTeamAssigned(PlayerRef player, int teamId)
        {
            if (Teams.TryGet(teamId, out var teamData))
            {
                teamData.CurrentSize++;
                Teams.Set(teamId, teamData);
                PlayerTeams.Set(player, teamId);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerTeamRemoved(PlayerRef player, int teamId)
        {
            if (Teams.TryGet(teamId, out var teamData))
            {
                teamData.CurrentSize = Mathf.Max(0, teamData.CurrentSize - 1);
                Teams.Set(teamId, teamData);
                PlayerTeams.Remove(player);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnTeamScoreUpdated(int teamId, int score)
        {
            if (Teams.TryGet(teamId, out var teamData))
            {
                teamData.TeamScore = score;
                Teams.Set(teamId, teamData);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnTeamLockChanged(int teamId, NetworkBool locked)
        {
            if (Teams.TryGet(teamId, out var teamData))
            {
                teamData.IsLocked = locked;
                Teams.Set(teamId, teamData);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnTeamNameChanged(int teamId, NetworkString<_32> name)
        {
            if (Teams.TryGet(teamId, out var teamData))
            {
                teamData.TeamName = name;
                Teams.Set(teamId, teamData);
            }
        }
    }
} 