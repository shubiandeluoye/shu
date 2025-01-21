using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;

namespace Core.Network
{
    public class NetworkMatchmaking : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<PlayerRef, MatchmakingData> QueuedPlayers { get; }
        [Networked] private NetworkDictionary<int, MatchData> ActiveMatches { get; }
        [Networked] private int NextMatchId { get; set; }
        [Networked] private float LastMatchmakingUpdate { get; set; }

        public struct MatchmakingData : INetworkStruct
        {
            public NetworkString<_16> PreferredMode;
            public byte TeamPreference;
            public float QueueTime;
            public int SkillRating;
            public NetworkBool IsReady;
        }

        public struct MatchData : INetworkStruct
        {
            public NetworkString<_16> GameMode;
            public NetworkString<_16> MapId;
            public PlayerRef Team1Player0;
            public PlayerRef Team1Player1;
            public PlayerRef Team1Player2;
            public PlayerRef Team1Player3;
            public PlayerRef Team2Player0;
            public PlayerRef Team2Player1;
            public PlayerRef Team2Player2;
            public PlayerRef Team2Player3;
            public byte Team1Count;
            public byte Team2Count;
            public float StartTime;
            public NetworkBool IsStarted;
        }

        public void QueueForMatch(PlayerRef player, string gameMode, byte teamPreference, int skillRating)
        {
            if (!Runner.IsServer) return;

            var matchmakingData = new MatchmakingData
            {
                PreferredMode = gameMode,
                TeamPreference = teamPreference,
                QueueTime = Runner.SimulationTime,
                SkillRating = skillRating,
                IsReady = false
            };

            QueuedPlayers.Set(player, matchmakingData);
            RPC_OnPlayerQueued(player, matchmakingData);
        }

        public void LeaveQueue(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            QueuedPlayers.Remove(player);
            RPC_OnPlayerLeftQueue(player);
        }

        public void SetPlayerReady(PlayerRef player, bool isReady)
        {
            if (!Runner.IsServer) return;
            if (!QueuedPlayers.TryGet(player, out var data)) return;

            data.IsReady = isReady;
            QueuedPlayers.Set(player, data);
            RPC_OnPlayerReadyStateChanged(player, isReady);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer) return;

            // 每秒更新一次匹配
            if (Runner.SimulationTime - LastMatchmakingUpdate < 1f) return;
            LastMatchmakingUpdate = Runner.SimulationTime;

            TryCreateMatches();
            UpdateActiveMatches();
        }

        private void TryCreateMatches()
        {
            var readyPlayers = new List<PlayerRef>();
            foreach (var kvp in QueuedPlayers)
            {
                if (kvp.Value.IsReady)
                {
                    readyPlayers.Add(kvp.Key);
                }
            }

            // 简单匹配逻辑：每8个准备好的玩家创建一场比赛
            while (readyPlayers.Count >= 8)
            {
                CreateMatch(readyPlayers.GetRange(0, 8));
                readyPlayers.RemoveRange(0, 8);
            }
        }

        private void CreateMatch(List<PlayerRef> players)
        {
            int matchId = NextMatchId++;
            var matchData = new MatchData
            {
                GameMode = "Default",
                MapId = "Random",
                StartTime = Runner.SimulationTime,
                IsStarted = false,
                Team1Count = 0,
                Team2Count = 0
            };

            // 分配玩家到队伍
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (i < 4)
                {
                    // 分配到队伍1
                    switch (i)
                    {
                        case 0: matchData.Team1Player0 = player; break;
                        case 1: matchData.Team1Player1 = player; break;
                        case 2: matchData.Team1Player2 = player; break;
                        case 3: matchData.Team1Player3 = player; break;
                    }
                    matchData.Team1Count++;
                }
                else
                {
                    // 分配到队伍2
                    switch (i - 4)
                    {
                        case 0: matchData.Team2Player0 = player; break;
                        case 1: matchData.Team2Player1 = player; break;
                        case 2: matchData.Team2Player2 = player; break;
                        case 3: matchData.Team2Player3 = player; break;
                    }
                    matchData.Team2Count++;
                }
                QueuedPlayers.Remove(player);
            }

            ActiveMatches.Set(matchId, matchData);
            RPC_OnMatchCreated(matchId, matchData);
        }

        private void UpdateActiveMatches()
        {
            var matchesToRemove = new List<int>();

            foreach (var kvp in ActiveMatches)
            {
                var matchId = kvp.Key;
                var match = kvp.Value;

                // 检查是否所有玩家都还在线
                bool allPlayersConnected = true;
                
                // 检查队伍1
                if (!CheckTeam1Players(match))
                {
                    allPlayersConnected = false;
                }
                
                // 检查队伍2
                if (!CheckTeam2Players(match))
                {
                    allPlayersConnected = false;
                }

                if (!allPlayersConnected)
                {
                    matchesToRemove.Add(matchId);
                }
            }

            foreach (var matchId in matchesToRemove)
            {
                RemoveMatch(matchId);
            }
        }

        private bool CheckTeam1Players(MatchData match)
        {
            if (match.Team1Count > 0 && !IsPlayerConnected(match.Team1Player0)) return false;
            if (match.Team1Count > 1 && !IsPlayerConnected(match.Team1Player1)) return false;
            if (match.Team1Count > 2 && !IsPlayerConnected(match.Team1Player2)) return false;
            if (match.Team1Count > 3 && !IsPlayerConnected(match.Team1Player3)) return false;
            return true;
        }

        private bool CheckTeam2Players(MatchData match)
        {
            if (match.Team2Count > 0 && !IsPlayerConnected(match.Team2Player0)) return false;
            if (match.Team2Count > 1 && !IsPlayerConnected(match.Team2Player1)) return false;
            if (match.Team2Count > 2 && !IsPlayerConnected(match.Team2Player2)) return false;
            if (match.Team2Count > 3 && !IsPlayerConnected(match.Team2Player3)) return false;
            return true;
        }

        private bool IsPlayerConnected(PlayerRef player)
        {
            return Runner != null && Runner.ActivePlayers.Contains(player);
        }

        private void RemoveMatch(int matchId)
        {
            if (!Runner.IsServer) return;

            ActiveMatches.Remove(matchId);
            RPC_OnMatchRemoved(matchId);
        }

        // 数据访问方法
        public MatchmakingData? GetPlayerQueueData(PlayerRef player)
        {
            return QueuedPlayers.TryGet(player, out var data) ? data : null;
        }

        public MatchData? GetMatchData(int matchId)
        {
            return ActiveMatches.TryGet(matchId, out var data) ? data : null;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerQueued(PlayerRef player, MatchmakingData data)
        {
            QueuedPlayers.Set(player, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerLeftQueue(PlayerRef player)
        {
            QueuedPlayers.Remove(player);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerReadyStateChanged(PlayerRef player, NetworkBool isReady)
        {
            if (QueuedPlayers.TryGet(player, out var data))
            {
                data.IsReady = isReady;
                QueuedPlayers.Set(player, data);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnMatchCreated(int matchId, MatchData data)
        {
            ActiveMatches.Set(matchId, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnMatchRemoved(int matchId)
        {
            ActiveMatches.Remove(matchId);
        }
    }
} 