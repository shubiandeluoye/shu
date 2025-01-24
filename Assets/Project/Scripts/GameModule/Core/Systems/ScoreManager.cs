using System.Collections.Generic;
using GameModule.Core.Data;
using GameModule.Core.Events;
using System.Linq;

namespace GameModule.Core.Systems
{
    public class ScoreManager
    {
        private readonly ScoreConfig config;
        private readonly IEventSystem eventSystem;
        private Dictionary<int, PlayerScore> scores;
        private Dictionary<int, float> playerStartTimes;
        private Dictionary<int, Dictionary<int, int>> roundScores; // roundId -> playerId -> score
        private Dictionary<int, int> killStreaks;  // 连杀记录

        public ScoreManager(ScoreConfig config, IEventSystem eventSystem)
        {
            this.config = config;
            this.eventSystem = eventSystem;
            scores = new Dictionary<int, PlayerScore>();
            playerStartTimes = new Dictionary<int, float>();
            roundScores = new Dictionary<int, Dictionary<int, int>>();
            killStreaks = new Dictionary<int, int>();
        }

        public void InitializePlayer(int playerId)
        {
            if (scores.ContainsKey(playerId)) return;

            scores[playerId] = new PlayerScore
            {
                PlayerId = playerId,
                Score = 0,
                Kills = 0,
                Deaths = 0,
                SurvivalTime = 0
            };

            playerStartTimes[playerId] = 0;
            NotifyScoreUpdate(playerId, 0, "初始化");
        }

        #region 回合分数管理
        public void StartNewRound(int roundId)
        {
            roundScores[roundId] = new Dictionary<int, int>();
            foreach (var playerId in scores.Keys)
            {
                roundScores[roundId][playerId] = 0;
            }

            // 重置连杀记录
            killStreaks.Clear();
        }

        public Dictionary<int, int> GetRoundScores(int roundId)
        {
            return roundScores.TryGetValue(roundId, out var scores) 
                ? new Dictionary<int, int>(scores) 
                : new Dictionary<int, int>();
        }

        public int GetPlayerRoundScore(int playerId, int roundId)
        {
            return roundScores.TryGetValue(roundId, out var scores) && 
                   scores.TryGetValue(playerId, out var score) 
                ? score : 0;
        }

        private void AddRoundScore(int roundId, int playerId, int amount)
        {
            if (!roundScores.TryGetValue(roundId, out var scores))
            {
                scores = new Dictionary<int, int>();
                roundScores[roundId] = scores;
            }

            if (!scores.ContainsKey(playerId))
            {
                scores[playerId] = 0;
            }

            scores[playerId] += amount;
        }
        #endregion

        #region 连杀系统
        private void UpdateKillStreak(int killerId)
        {
            if (!killStreaks.ContainsKey(killerId))
            {
                killStreaks[killerId] = 0;
            }

            killStreaks[killerId]++;
            
            // 检查连杀奖励
            CheckKillStreakBonus(killerId, killStreaks[killerId]);
        }

        private void ResetKillStreak(int playerId)
        {
            if (killStreaks.ContainsKey(playerId))
            {
                killStreaks[playerId] = 0;
            }
        }

        private void CheckKillStreakBonus(int playerId, int streak)
        {
            int bonus = 0;
            string reason = string.Empty;

            switch (streak)
            {
                case 3:
                    bonus = 50;
                    reason = "三连杀";
                    break;
                case 5:
                    bonus = 100;
                    reason = "五连杀";
                    break;
                case 7:
                    bonus = 200;
                    reason = "七连杀";
                    break;
                case 10:
                    bonus = 500;
                    reason = "超神";
                    break;
            }

            if (bonus > 0)
            {
                AddScore(playerId, bonus, reason);
            }
        }
        #endregion

        public void HandlePlayerDeath(PlayerDeathEvent evt)
        {
            var victimId = evt.VictimId;
            var killerId = evt.KillerId;

            if (scores.ContainsKey(victimId))
            {
                scores[victimId].Deaths++;
                AddScore(victimId, config.DeathPenalty, "死亡惩罚");
                UpdateSurvivalTime(victimId);
                ResetKillStreak(victimId);
            }

            if (scores.ContainsKey(killerId) && killerId != 0)
            {
                scores[killerId].Kills++;
                AddScore(killerId, config.KillScore, "击杀奖励");
                UpdateKillStreak(killerId);
            }
        }

        public void AddScore(int playerId, int amount, string reason)
        {
            if (!scores.ContainsKey(playerId)) return;

            var oldScore = scores[playerId].Score;
            scores[playerId].Score += amount;

            // 更新当前回合分数
            if (roundScores.Count > 0)
            {
                var currentRoundId = roundScores.Keys.Max();
                AddRoundScore(currentRoundId, playerId, amount);
            }

            NotifyScoreUpdate(playerId, amount, reason);
            CheckRankingChange(playerId, oldScore, scores[playerId].Score);
        }

        private void UpdateSurvivalTime(int playerId, float currentTime)
        {
            if (playerStartTimes.ContainsKey(playerId))
            {
                float survivalTime = currentTime - playerStartTimes[playerId];
                scores[playerId].SurvivalTime += survivalTime;
                
                playerStartTimes[playerId] = currentTime;

                int survivalBonus = (int)(survivalTime * config.SurvivalBonus);
                if (survivalBonus > 0)
                {
                    AddScore(playerId, survivalBonus, "生存奖励");
                }
            }
        }

        private void CheckRankingChange(int playerId, int oldScore, int newScore)
        {
            var oldRanking = GetPlayerRanking(playerId, oldScore);
            var newRanking = GetPlayerRanking(playerId, newScore);

            if (oldRanking != newRanking)
            {
                eventSystem.Publish(new RankingChangeEvent
                {
                    PlayerId = playerId,
                    OldRank = oldRanking,
                    NewRank = newRanking,
                    Score = newScore
                });
            }
        }

        private int GetPlayerRanking(int playerId, int score)
        {
            int ranking = 1;
            foreach (var playerScore in scores.Values)
            {
                if (playerScore.PlayerId != playerId && playerScore.Score > score)
                {
                    ranking++;
                }
            }
            return ranking;
        }

        private void NotifyScoreUpdate(int playerId, int scoreChange, string reason)
        {
            eventSystem.Publish(new ScoreUpdateEvent
            {
                PlayerId = playerId,
                NewScore = scores[playerId].Score,
                ScoreChange = scoreChange,
                Reason = reason
            });
        }

        public Dictionary<int, PlayerScore> GetScores()
        {
            return new Dictionary<int, PlayerScore>(scores);
        }

        public void Reset()
        {
            scores.Clear();
            playerStartTimes.Clear();
            roundScores.Clear();
            killStreaks.Clear();
        }

        public void Dispose()
        {
            Reset();
        }
    }
} 