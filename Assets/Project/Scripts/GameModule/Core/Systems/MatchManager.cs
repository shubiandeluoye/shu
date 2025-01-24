using System.Collections.Generic;
using GameModule.Core.Data;
using GameModule.Core.Events;
using UnityEngine;

namespace GameModule.Core.Systems
{
    public class MatchManager
    {
        private readonly MatchConfig config;
        private readonly IEventSystem eventSystem;
        private Dictionary<int, PlayerMatchData> players;
        private Dictionary<int, int> playerTeams;      // playerId -> teamId
        private HashSet<int> spectators;
        private float matchmakingStartTime;
        private bool isMatchmaking;

        public int PlayerCount => players.Count;
        public bool IsMatchmaking => isMatchmaking;

        public MatchManager(MatchConfig config, IEventSystem eventSystem)
        {
            this.config = config;
            this.eventSystem = eventSystem;
            players = new Dictionary<int, PlayerMatchData>();
            playerTeams = new Dictionary<int, int>();
            spectators = new HashSet<int>();
            Reset();
        }

        public void Reset()
        {
            players.Clear();
            playerTeams.Clear();
            spectators.Clear();
            isMatchmaking = false;
            matchmakingStartTime = 0;
        }

        public void AddPlayer(int playerId)
        {
            if (players.ContainsKey(playerId)) return;
            if (!config.GameModeConfig.AllowLateJoin && isGameStarted) return;

            var playerData = new PlayerMatchData
            {
                PlayerId = playerId,
                JoinTime = 0,
                IsReady = false
            };

            players.Add(playerId, playerData);
            
            // 自动分配队伍
            if (config.RuleConfig.EnableTeamMode)
            {
                AssignPlayerToBalancedTeam(playerId);
            }

            eventSystem.Publish(new PlayerCountChangeEvent 
            { 
                CurrentCount = PlayerCount,
                MaxCount = config.MaxPlayers
            });

            if (PlayerCount == 1)
            {
                StartMatchmaking();
            }
        }

        public void RemovePlayer(int playerId)
        {
            if (!players.ContainsKey(playerId)) return;

            players.Remove(playerId);

            eventSystem.Publish(new PlayerCountChangeEvent 
            { 
                CurrentCount = PlayerCount,
                MaxCount = config.MaxPlayers
            });

            if (PlayerCount < config.MinPlayers)
            {
                StopMatchmaking();
            }
        }

        public void SetPlayerReady(int playerId, bool isReady)
        {
            if (players.TryGetValue(playerId, out var playerData))
            {
                playerData.IsReady = isReady;
                CheckAllPlayersReady();
            }
        }

        public void Update(float deltaTime)
        {
            if (!isMatchmaking) return;

            matchmakingStartTime += deltaTime;
            if (matchmakingStartTime >= config.MatchmakingTimeout)
            {
                HandleMatchmakingTimeout();
            }
        }

        private void HandleMatchmakingTimeout()
        {
            if (PlayerCount >= config.MinPlayers)
            {
                eventSystem.Publish(new MatchReadyEvent());
            }
            else
            {
                StopMatchmaking();
                eventSystem.Publish(new MatchTimeoutEvent());
            }
        }

        public bool CanStartGame()
        {
            return PlayerCount >= config.MinPlayers && 
                   PlayerCount <= config.MaxPlayers && 
                   AreAllPlayersReady();
        }

        private bool AreAllPlayersReady()
        {
            foreach (var playerData in players.Values)
            {
                if (!playerData.IsReady) return false;
            }
            return true;
        }

        #region 队伍管理
        public void AssignPlayerTeam(int playerId, int teamId)
        {
            if (!players.ContainsKey(playerId)) return;
            if (teamId >= config.TeamsCount) return;

            var oldTeamId = GetPlayerTeam(playerId);
            playerTeams[playerId] = teamId;

            eventSystem.Publish(new PlayerTeamChangedEvent
            {
                PlayerId = playerId,
                OldTeamId = oldTeamId,
                NewTeamId = teamId
            });
        }

        public int GetPlayerTeam(int playerId)
        {
            return playerTeams.TryGetValue(playerId, out var teamId) ? teamId : -1;
        }

        public List<int> GetTeamPlayers(int teamId)
        {
            var teamPlayers = new List<int>();
            foreach (var kvp in playerTeams)
            {
                if (kvp.Value == teamId)
                {
                    teamPlayers.Add(kvp.Key);
                }
            }
            return teamPlayers;
        }

        public bool AreInSameTeam(int player1Id, int player2Id)
        {
            return GetPlayerTeam(player1Id) == GetPlayerTeam(player2Id);
        }
        #endregion

        #region 观战系统
        public void AddSpectator(int spectatorId, int targetPlayerId)
        {
            if (players.ContainsKey(spectatorId)) return;

            spectators.Add(spectatorId);
            eventSystem.Publish(new SpectatorJoinEvent
            {
                SpectatorId = spectatorId,
                TargetPlayerId = targetPlayerId
            });
        }

        public void RemoveSpectator(int spectatorId)
        {
            if (spectators.Remove(spectatorId))
            {
                eventSystem.Publish(new SpectatorLeaveEvent
                {
                    SpectatorId = spectatorId
                });
            }
        }

        public bool IsSpectator(int playerId)
        {
            return spectators.Contains(playerId);
        }

        public List<int> GetSpectators()
        {
            return new List<int>(spectators);
        }
        #endregion

        #region 玩家管理扩展
        private void AssignPlayerToBalancedTeam(int playerId)
        {
            // 找出人数最少的队伍
            var teamCounts = new int[config.TeamsCount];
            foreach (var teamId in playerTeams.Values)
            {
                teamCounts[teamId]++;
            }

            int minTeamId = 0;
            for (int i = 1; i < teamCounts.Length; i++)
            {
                if (teamCounts[i] < teamCounts[minTeamId])
                {
                    minTeamId = i;
                }
            }

            AssignPlayerTeam(playerId, minTeamId);
        }
        #endregion

        public Vector2 GetPlayerPosition(int playerId)
        {
            // TODO: 实现获取玩家位置的逻辑
            return Vector2.zero;
        }

        public void Dispose()
        {
            Reset();
            players = null;
            playerTeams = null;
            spectators = null;
        }
    }

    public class PlayerMatchData
    {
        public int PlayerId;
        public float JoinTime;
        public bool IsReady;
    }
} 