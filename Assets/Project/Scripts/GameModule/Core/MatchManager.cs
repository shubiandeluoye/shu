using UnityEngine;
using System.Collections.Generic;
using Core.Network;
using Core.EventSystem;
using GameModule.Data;
using Fusion;
namespace GameModule.Core
{
    /// <summary>
    /// 匹配管理器
    /// 负责：
    /// 1. 玩家加入/离开管理
    /// 2. 房间状态管理
    /// 3. 匹配超时处理
    /// </summary>
    public class MatchManager
    {
        private readonly MatchConfig config;
        private Dictionary<PlayerRef, PlayerMatchData> players;
        private float matchmakingStartTime;
        private bool isMatchmaking;

        public int PlayerCount => players.Count;
        public bool IsMatchmaking => isMatchmaking;

        public MatchManager(MatchConfig config)
        {
            this.config = config;
            players = new Dictionary<PlayerRef, PlayerMatchData>();
            Reset();
        }

        public void Reset()
        {
            players.Clear();
            isMatchmaking = false;
            matchmakingStartTime = 0;
        }

        #region 玩家管理

        public void AddPlayer(PlayerRef playerRef)
        {
            if (players.ContainsKey(playerRef)) return;

            var playerData = new PlayerMatchData
            {
                PlayerRef = playerRef,
                JoinTime = Time.time,
                IsReady = false
            };

            players.Add(playerRef, playerData);

            // 通知玩家加入
            EventManager.Instance.TriggerEvent(new PlayerCountChangeEvent 
            { 
                CurrentCount = PlayerCount,
                MaxCount = config.MaxPlayers
            });

            // 如果是第一个玩家，开始匹配
            if (PlayerCount == 1)
            {
                StartMatchmaking();
            }
        }

        public void RemovePlayer(PlayerRef playerRef)
        {
            if (!players.ContainsKey(playerRef)) return;

            players.Remove(playerRef);

            // 通知玩家离开
            EventManager.Instance.TriggerEvent(new PlayerCountChangeEvent 
            { 
                CurrentCount = PlayerCount,
                MaxCount = config.MaxPlayers
            });

            // 如果玩家数量不足，停止匹配
            if (PlayerCount < config.MinPlayers)
            {
                StopMatchmaking();
            }
        }

        public void SetPlayerReady(PlayerRef playerRef, bool isReady)
        {
            if (players.TryGetValue(playerRef, out var playerData))
            {
                playerData.IsReady = isReady;
                CheckAllPlayersReady();
            }
        }

        #endregion

        #region 匹配控制

        public void StartMatchmaking()
        {
            if (isMatchmaking) return;

            isMatchmaking = true;
            matchmakingStartTime = Time.time;

            // 通知匹配开始
            EventManager.Instance.TriggerEvent(new MatchmakingStartEvent 
            { 
                Timeout = config.MatchmakingTimeout 
            });
        }

        public void StopMatchmaking()
        {
            if (!isMatchmaking) return;

            isMatchmaking = false;

            // 通知匹配停止
            EventManager.Instance.TriggerEvent(new MatchmakingStopEvent());
        }

        public void Update()
        {
            if (!isMatchmaking) return;

            // 检查匹配超时
            if (Time.time - matchmakingStartTime >= config.MatchmakingTimeout)
            {
                HandleMatchmakingTimeout();
            }
        }

        private void HandleMatchmakingTimeout()
        {
            // 如果玩家数量达到最小要求，开始游戏
            if (PlayerCount >= config.MinPlayers)
            {
                EventManager.Instance.TriggerEvent(new MatchReadyEvent());
            }
            else
            {
                // 否则重置匹配
                StopMatchmaking();
                EventManager.Instance.TriggerEvent(new MatchTimeoutEvent());
            }
        }

        #endregion

        #region 状态检查

        public bool CanStartGame()
        {
            return PlayerCount >= config.MinPlayers && 
                   PlayerCount <= config.MaxPlayers && 
                   AreAllPlayersReady();
        }

        public bool HasEnoughPlayers()
        {
            return PlayerCount >= config.MinPlayers;
        }

        private bool AreAllPlayersReady()
        {
            foreach (var playerData in players.Values)
            {
                if (!playerData.IsReady) return false;
            }
            return true;
        }

        private void CheckAllPlayersReady()
        {
            if (AreAllPlayersReady() && PlayerCount >= config.MinPlayers)
            {
                EventManager.Instance.TriggerEvent(new MatchReadyEvent());
            }
        }

        #endregion

        public List<PlayerRef> GetAllPlayers()
        {
            return new List<PlayerRef>(players.Keys);
        }

        public void Dispose()
        {
            players.Clear();
        }
    }

    public class PlayerMatchData
    {
        public PlayerRef PlayerRef;
        public float JoinTime;
        public bool IsReady;
    }
} 