using UnityEngine;
using System.Collections.Generic;
using Core.EventSystem;
using GameModule.Data;
using Fusion;  // TODO: [网络重构] 临时直接使用 Fusion，后期需要替换

namespace GameModule.Core
{
    /// <summary>
    /// 分数管理器
    /// 负责：
    /// 1. 分数计算和更新
    /// 2. 击杀统计
    /// 3. 生存时间统计
    /// 4. 排行榜管理
    /// </summary>
    public class ScoreManager
    {
        private readonly ScoreConfig config;
        private readonly NetworkObject networkObject;  // TODO: [网络重构] 临时使用 NetworkObject
        private Dictionary<PlayerRef, PlayerScore> scores;
        private Dictionary<PlayerRef, float> playerStartTimes;

        public ScoreManager(ScoreConfig config, NetworkObject networkObject)
        {
            this.config = config;
            this.networkObject = networkObject;
            scores = new Dictionary<PlayerRef, PlayerScore>();
            playerStartTimes = new Dictionary<PlayerRef, float>();
        }

        #region 分数管理

        public void InitializePlayer(PlayerRef playerRef)
        {
            if (scores.ContainsKey(playerRef)) return;

            scores[playerRef] = new PlayerScore
            {
                PlayerRef = playerRef,
                Score = 0,
                Kills = 0,
                Deaths = 0,
                SurvivalTime = 0
            };

            playerStartTimes[playerRef] = Time.time;

            // 通知新玩家分数初始化
            NotifyScoreUpdate(playerRef, 0, "初始化");
        }

        public void AddScore(PlayerRef playerRef, int amount, string reason)
        {
            if (!scores.ContainsKey(playerRef)) return;

            var oldScore = scores[playerRef].Score;
            scores[playerRef].Score += amount;

            // 通知分数变化
            NotifyScoreUpdate(playerRef, amount, reason);

            // 检查是否触发排名变化
            CheckRankingChange(playerRef, oldScore, scores[playerRef].Score);
        }

        public void HandlePlayerDeath(PlayerDeathEvent evt)
        {
            var victim = evt.Victim;
            var killer = evt.Killer;

            if (scores.ContainsKey(victim))
            {
                scores[victim].Deaths++;
                AddScore(victim, config.DeathPenalty, "死亡惩罚");
                UpdateSurvivalTime(victim);
            }

            // TODO: [网络重构] 直接使用 Fusion 的 PlayerRef 判断
            if (scores.ContainsKey(killer) && killer != PlayerRef.None)  // 改用 PlayerRef.None 判断
            {
                scores[killer].Kills++;
                AddScore(killer, config.KillScore, "击杀奖励");
            }
        }

        private void UpdateSurvivalTime(PlayerRef playerRef)
        {
            if (playerStartTimes.ContainsKey(playerRef))
            {
                float survivalTime = Time.time - playerStartTimes[playerRef];
                scores[playerRef].SurvivalTime += survivalTime;
                
                // 重置开始时间（用于重生）
                playerStartTimes[playerRef] = Time.time;

                // 给予生存奖励
                int survivalBonus = Mathf.FloorToInt(survivalTime * config.SurvivalBonus);
                if (survivalBonus > 0)
                {
                    AddScore(playerRef, survivalBonus, "生存奖励");
                }
            }
        }

        #endregion

        #region 排行榜管理

        private void CheckRankingChange(PlayerRef playerRef, int oldScore, int newScore)
        {
            var oldRanking = GetPlayerRanking(playerRef, oldScore);
            var newRanking = GetPlayerRanking(playerRef, newScore);

            if (oldRanking != newRanking)
            {
                EventManager.Instance.TriggerEvent(new RankingChangeEvent
                {
                    PlayerRef = playerRef,
                    OldRank = oldRanking,
                    NewRank = newRanking,
                    Score = newScore
                });
            }
        }

        private int GetPlayerRanking(PlayerRef playerRef, int score)
        {
            int ranking = 1;
            foreach (var playerScore in scores.Values)
            {
                if (playerScore.PlayerRef != playerRef && playerScore.Score > score)
                {
                    ranking++;
                }
            }
            return ranking;
        }

        public List<PlayerScore> GetTopPlayers(int count)
        {
            var playerList = new List<PlayerScore>(scores.Values);
            playerList.Sort((a, b) => b.Score.CompareTo(a.Score));
            return playerList.GetRange(0, Mathf.Min(count, playerList.Count));
        }

        #endregion

        #region 统计和查询

        public PlayerScore GetPlayerScore(PlayerRef playerRef)
        {
            return scores.TryGetValue(playerRef, out var score) ? score : null;
        }

        public Dictionary<PlayerRef, PlayerScore> GetScores()
        {
            return new Dictionary<PlayerRef, PlayerScore>(scores);
        }

        public Dictionary<PlayerRef, PlayerScore> GetFinalScores()
        {
            // 在游戏结束时更新所有玩家的最终生存时间
            foreach (var playerRef in scores.Keys)
            {
                UpdateSurvivalTime(playerRef);
            }
            return GetScores();
        }

        #endregion

        private void NotifyScoreUpdate(PlayerRef playerRef, int scoreChange, string reason)
        {
            EventManager.Instance.TriggerEvent(new ScoreUpdateEvent
            {
                PlayerRef = playerRef,
                NewScore = scores[playerRef].Score,
                ScoreChange = scoreChange,
                Reason = reason
            });
        }

        public void Reset()
        {
            scores.Clear();
            playerStartTimes.Clear();
        }

        public void Dispose()
        {
            Reset();
        }
    }

    public struct RankingChangeEvent
    {
        public PlayerRef PlayerRef;
        public int OldRank;
        public int NewRank;
        public int Score;
    }
} 