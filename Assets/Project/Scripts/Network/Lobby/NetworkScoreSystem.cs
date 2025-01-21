using UnityEngine;
using Fusion;
using System.Collections.Generic;

namespace Core.Network
{
    public class NetworkScoreSystem : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<PlayerRef, PlayerScoreData> PlayerScores { get; }
        [Networked] private NetworkDictionary<int, TeamScoreData> TeamScores { get; }
        [Networked] private NetworkDictionary<int, MatchScoreData> MatchHistory { get; }
        [Networked] private int CurrentMatchId { get; set; }

        public struct PlayerScoreData : INetworkStruct
        {
            public int Kills;
            public int Deaths;
            public int Assists;
            public int Score;
            public int ObjectiveScore;
            public float DamageDealt;
            public float DamageTaken;
            public float HealingDone;
            public int MultiKills;
            public float LastUpdateTime;
        }

        public struct TeamScoreData : INetworkStruct
        {
            public int TeamId;
            public int TotalScore;
            public int ObjectivesCompleted;
            public int RoundsWon;
            public PlayerRef TopScorer0;
            public PlayerRef TopScorer1;
            public PlayerRef TopScorer2;
            public PlayerRef TopScorer3;
            public PlayerRef TopScorer4;
            public PlayerRef TopScorer5;
            public PlayerRef TopScorer6;
            public PlayerRef TopScorer7;
            public byte TopScorerCount;
            public float LastUpdateTime;

            public PlayerRef GetTopScorer(int index)
            {
                if (index < 0 || index >= 8) return default;
                switch (index)
                {
                    case 0: return TopScorer0;
                    case 1: return TopScorer1;
                    case 2: return TopScorer2;
                    case 3: return TopScorer3;
                    case 4: return TopScorer4;
                    case 5: return TopScorer5;
                    case 6: return TopScorer6;
                    case 7: return TopScorer7;
                    default: return default;
                }
            }

            public void SetTopScorer(int index, PlayerRef value)
            {
                if (index < 0 || index >= 8) return;
                switch (index)
                {
                    case 0: TopScorer0 = value; break;
                    case 1: TopScorer1 = value; break;
                    case 2: TopScorer2 = value; break;
                    case 3: TopScorer3 = value; break;
                    case 4: TopScorer4 = value; break;
                    case 5: TopScorer5 = value; break;
                    case 6: TopScorer6 = value; break;
                    case 7: TopScorer7 = value; break;
                }
            }
        }

        public struct MatchScoreData : INetworkStruct
        {
            public int WinningTeam;
            public int TeamScore0;
            public int TeamScore1;
            public int TeamScore2;
            public int TeamScore3;
            public byte TeamCount;
            public PlayerRef MVP0;
            public PlayerRef MVP1;
            public PlayerRef MVP2;
            public byte MVPCount;
            public float MatchDuration;
            public NetworkString<_32> MapId;
            public NetworkString<_16> GameMode;
            public float MatchEndTime;

            public int GetTeamScore(int index)
            {
                if (index < 0 || index >= 4) return 0;
                switch (index)
                {
                    case 0: return TeamScore0;
                    case 1: return TeamScore1;
                    case 2: return TeamScore2;
                    case 3: return TeamScore3;
                    default: return 0;
                }
            }

            public void SetTeamScore(int index, int value)
            {
                if (index < 0 || index >= 4) return;
                switch (index)
                {
                    case 0: TeamScore0 = value; break;
                    case 1: TeamScore1 = value; break;
                    case 2: TeamScore2 = value; break;
                    case 3: TeamScore3 = value; break;
                }
            }

            public PlayerRef GetMVP(int index)
            {
                if (index < 0 || index >= 3) return default;
                switch (index)
                {
                    case 0: return MVP0;
                    case 1: return MVP1;
                    case 2: return MVP2;
                    default: return default;
                }
            }

            public void SetMVP(int index, PlayerRef value)
            {
                if (index < 0 || index >= 3) return;
                switch (index)
                {
                    case 0: MVP0 = value; break;
                    case 1: MVP1 = value; break;
                    case 2: MVP2 = value; break;
                }
            }
        }

        public void InitializePlayerScore(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            var scoreData = new PlayerScoreData
            {
                Kills = 0,
                Deaths = 0,
                Assists = 0,
                Score = 0,
                ObjectiveScore = 0,
                DamageDealt = 0,
                DamageTaken = 0,
                HealingDone = 0,
                MultiKills = 0,
                LastUpdateTime = Runner.SimulationTime
            };

            PlayerScores.Set(player, scoreData);
            RPC_OnPlayerScoreInitialized(player, scoreData);
        }

        public void InitializeTeamScore(int teamId)
        {
            if (!Runner.IsServer) return;

            var scoreData = new TeamScoreData
            {
                TeamId = teamId,
                TotalScore = 0,
                ObjectivesCompleted = 0,
                RoundsWon = 0,
                TopScorer0 = default,
                TopScorer1 = default,
                TopScorer2 = default,
                TopScorer3 = default,
                TopScorer4 = default,
                TopScorer5 = default,
                TopScorer6 = default,
                TopScorer7 = default,
                TopScorerCount = 0,
                LastUpdateTime = Runner.SimulationTime
            };

            TeamScores.Set(teamId, scoreData);
            RPC_OnTeamScoreInitialized(teamId, scoreData);
        }

        public void UpdatePlayerScore(PlayerRef player, PlayerScoreData newScore)
        {
            if (!Runner.IsServer) return;
            if (!PlayerScores.TryGet(player, out var currentScore)) return;

            // 验证分数变化
            if (!ValidateScoreChange(currentScore, newScore)) return;

            newScore.LastUpdateTime = Runner.SimulationTime;
            PlayerScores.Set(player, newScore);
            RPC_OnPlayerScoreUpdated(player, newScore);

            // 更新队伍得分
            UpdateTeamTopScorers();
        }

        public void AddPlayerScore(PlayerRef player, int score, int objectiveScore = 0)
        {
            if (!Runner.IsServer) return;
            if (!PlayerScores.TryGet(player, out var currentScore)) return;

            currentScore.Score += score;
            currentScore.ObjectiveScore += objectiveScore;
            currentScore.LastUpdateTime = Runner.SimulationTime;

            PlayerScores.Set(player, currentScore);
            RPC_OnPlayerScoreUpdated(player, currentScore);

            // 更新队伍得分
            UpdateTeamTopScorers();
        }

        public void RecordKillEvent(PlayerRef killer, PlayerRef victim, PlayerRef? assister = null)
        {
            if (!Runner.IsServer) return;
            if (!PlayerScores.TryGet(killer, out var killerScore)) return;
            if (!PlayerScores.TryGet(victim, out var victimScore)) return;

            killerScore.Kills++;
            victimScore.Deaths++;
            
            if (assister.HasValue && PlayerScores.TryGet(assister.Value, out var assisterScore))
            {
                assisterScore.Assists++;
                PlayerScores.Set(assister.Value, assisterScore);
            }

            PlayerScores.Set(killer, killerScore);
            PlayerScores.Set(victim, victimScore);

            RPC_OnKillEventRecorded(killer, victim, assister ?? default);
        }

        public void RecordDamage(PlayerRef source, PlayerRef target, float amount)
        {
            if (!Runner.IsServer) return;
            if (!PlayerScores.TryGet(source, out var sourceScore)) return;
            if (!PlayerScores.TryGet(target, out var targetScore)) return;

            sourceScore.DamageDealt += amount;
            targetScore.DamageTaken += amount;

            PlayerScores.Set(source, sourceScore);
            PlayerScores.Set(target, targetScore);

            RPC_OnDamageRecorded(source, target, amount);
        }

        public void RecordHealing(PlayerRef healer, float amount)
        {
            if (!Runner.IsServer) return;
            if (!PlayerScores.TryGet(healer, out var healerScore)) return;

            healerScore.HealingDone += amount;
            PlayerScores.Set(healer, healerScore);

            RPC_OnHealingRecorded(healer, amount);
        }

        public void EndMatch(int winningTeam, string mapId, string gameMode)
        {
            if (!Runner.IsServer) return;

            var matchData = new MatchScoreData
            {
                WinningTeam = winningTeam,
                TeamCount = 0,
                MVPCount = 0,
                MatchDuration = Runner.SimulationTime,
                MapId = mapId,
                GameMode = gameMode,
                MatchEndTime = Runner.SimulationTime
            };

            // 记录队伍分数
            byte teamCount = 0;
            foreach (var kvp in TeamScores)
            {
                if (teamCount < 4)
                {
                    matchData.SetTeamScore(teamCount, kvp.Value.TotalScore);
                    teamCount++;
                }
            }
            matchData.TeamCount = teamCount;

            // 确定MVP
            DetermineMVPs(ref matchData);

            MatchHistory.Set(CurrentMatchId++, matchData);
            RPC_OnMatchEnded(matchData);
        }

        private void UpdateTeamTopScorers()
        {
            foreach (var teamKvp in TeamScores)
            {
                var teamId = teamKvp.Key;
                var teamData = teamKvp.Value;
                var topScorers = new PlayerRef[8];
                int topScorersCount = 0;

                // 找出得分最高的玩家
                foreach (var playerKvp in PlayerScores)
                {
                    if (topScorersCount < 8)
                    {
                        topScorers[topScorersCount++] = playerKvp.Key;
                    }
                    else
                    {
                        // 找到得分最低的位置
                        int lowestIndex = 0;
                        float lowestScore = float.MaxValue;
                        for (int i = 0; i < topScorers.Length; i++)
                        {
                            if (PlayerScores.TryGet(topScorers[i], out var score) && score.Score < lowestScore)
                            {
                                lowestScore = score.Score;
                                lowestIndex = i;
                            }
                        }

                        if (playerKvp.Value.Score > lowestScore)
                        {
                            topScorers[lowestIndex] = playerKvp.Key;
                        }
                    }
                }

                // 更新队伍数据
                teamData.TopScorerCount = (byte)topScorersCount;
                for (int i = 0; i < topScorersCount; i++)
                {
                    teamData.SetTopScorer(i, topScorers[i]);
                }

                TeamScores.Set(teamId, teamData);
            }
        }

        private void DetermineMVPs(ref MatchScoreData matchData)
        {
            var mvpList = new List<(PlayerRef player, int score)>();

            // 收集所有玩家分数
            foreach (var kvp in PlayerScores)
            {
                mvpList.Add((kvp.Key, kvp.Value.Score));
            }

            // 按分数排序
            mvpList.Sort((a, b) => b.score.CompareTo(a.score));

            // 取前三名
            int mvpCount = Mathf.Min(3, mvpList.Count);
            for (int i = 0; i < mvpCount; i++)
            {
                matchData.SetMVP(i, mvpList[i].player);
            }
            matchData.MVPCount = (byte)mvpCount;
        }

        private bool ValidateScoreChange(PlayerScoreData oldScore, PlayerScoreData newScore)
        {
            // 基本验证
            if (newScore.Score < oldScore.Score) return false;
            if (newScore.Deaths < oldScore.Deaths) return false;
            if (newScore.DamageDealt < oldScore.DamageDealt) return false;
            if (newScore.DamageTaken < oldScore.DamageTaken) return false;
            if (newScore.HealingDone < oldScore.HealingDone) return false;

            // 分数变化验证
            float maxScoreChange = (newScore.LastUpdateTime - oldScore.LastUpdateTime) * 1000; // 每秒最大1000分
            if (newScore.Score - oldScore.Score > maxScoreChange) return false;

            return true;
        }

        // 数据访问方法
        public PlayerScoreData? GetPlayerScore(PlayerRef player)
        {
            return PlayerScores.TryGet(player, out var data) ? data : null;
        }

        public TeamScoreData? GetTeamScore(int teamId)
        {
            return TeamScores.TryGet(teamId, out var data) ? data : null;
        }

        public MatchScoreData? GetMatchHistory(int matchId)
        {
            return MatchHistory.TryGet(matchId, out var data) ? data : null;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerScoreInitialized(PlayerRef player, PlayerScoreData data)
        {
            PlayerScores.Set(player, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnTeamScoreInitialized(int teamId, TeamScoreData data)
        {
            TeamScores.Set(teamId, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerScoreUpdated(PlayerRef player, PlayerScoreData data)
        {
            PlayerScores.Set(player, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnKillEventRecorded(PlayerRef killer, PlayerRef victim, PlayerRef assister)
        {
            if (PlayerScores.TryGet(killer, out var killerScore))
            {
                killerScore.Kills++;
                PlayerScores.Set(killer, killerScore);
            }

            if (PlayerScores.TryGet(victim, out var victimScore))
            {
                victimScore.Deaths++;
                PlayerScores.Set(victim, victimScore);
            }

            if (assister != default && PlayerScores.TryGet(assister, out var assisterScore))
            {
                assisterScore.Assists++;
                PlayerScores.Set(assister, assisterScore);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnDamageRecorded(PlayerRef source, PlayerRef target, float amount)
        {
            if (PlayerScores.TryGet(source, out var sourceScore))
            {
                sourceScore.DamageDealt += amount;
                PlayerScores.Set(source, sourceScore);
            }

            if (PlayerScores.TryGet(target, out var targetScore))
            {
                targetScore.DamageTaken += amount;
                PlayerScores.Set(target, targetScore);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnHealingRecorded(PlayerRef healer, float amount)
        {
            if (PlayerScores.TryGet(healer, out var healerScore))
            {
                healerScore.HealingDone += amount;
                PlayerScores.Set(healer, healerScore);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnMatchEnded(MatchScoreData matchData)
        {
            MatchHistory.Set(CurrentMatchId++, matchData);
        }
    }
} 