using UnityEngine;
using Core.Singleton;
using Core.Network;
using Core.EventSystem;
using GameModule.Core;
using GameModule.Data;
using System.Collections.Generic;
using Fusion;
// 明确引用事件定义
using GameEvents = GameModule.Data;  // 给命名空间起个别名

namespace GameModule
{
    /// <summary>
    /// 游戏总管理器
    /// 注意：当前使用 Fusion 网络方案，后期可能更换
    /// 网络相关的接口调用应该通过 INetworkAuthority 接口隔离
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("游戏配置")]
        [SerializeField] private GameConfig gameConfig;
        
        [Header("系统引用")]
        [SerializeField] private NetworkObject networkObject;
        
        private GameStateManager stateManager;
        private MatchManager matchManager;
        private ScoreManager scoreManager;

        [Networked] public NetworkBool IsGameStarted { get; private set; }
        [Networked] public NetworkBool IsGamePaused { get; private set; }
        [Networked] public float GameTime { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            stateManager = new GameStateManager(gameConfig.StateConfig);
            matchManager = new MatchManager(gameConfig.MatchConfig);
            scoreManager = new ScoreManager(gameConfig.ScoreConfig, networkObject);
        }

        #region 游戏流程控制
        public void StartGame()
        {
            if (!networkObject.HasStateAuthority || IsGameStarted) return;

            IsGameStarted = true;
            IsGamePaused = false;
            GameTime = 0;

            stateManager.SetState(GameState.Playing);
            SpawnPlayers();
            
            // 使用别名引用事件
            EventManager.Instance.TriggerEvent(new GameEvents.GameStartEvent());
        }

        public void PauseGame()
        {
            if (!networkObject.HasStateAuthority || !IsGameStarted) return;
            
            IsGamePaused = !IsGamePaused;
            EventManager.Instance.TriggerEvent(new GamePauseEvent { IsPaused = IsGamePaused });
        }

        public void EndGame(GameOverReason reason)
        {
            if (!networkObject.HasStateAuthority || !IsGameStarted) return;

            IsGameStarted = false;
            var finalScores = scoreManager.GetFinalScores();
            
            EventManager.Instance.TriggerEvent(new GameModule.Data.GameEndEvent 
            { 
                Reason = reason,
                FinalScores = finalScores
            });

            stateManager.SetState(GameState.GameOver);
        }
        #endregion

        #region 玩家管理
        private void SpawnPlayers()
        {
            var spawnPoints = GetSpawnPoints();
            var players = matchManager.GetAllPlayers();

            for (int i = 0; i < players.Count; i++)
            {
                if (i < spawnPoints.Count)
                {
                    SpawnPlayer(players[i], spawnPoints[i]);
                }
            }
        }

        private void SpawnPlayer(PlayerRef player, Vector3 position)
        {
            if (!networkObject.HasStateAuthority) return;
            
            // 初始化玩家分数
            scoreManager.InitializePlayer(player);
        }

        private List<Vector3> GetSpawnPoints()
        {
            // 从地图获取出生点
            return new List<Vector3>
            {
                new Vector3(-5, 0, 0),
                new Vector3(5, 0, 0),
                new Vector3(0, 5, 0),
                new Vector3(0, -5, 0)
            };
        }
        #endregion

        #region 事件处理
        private void OnPlayerJoin(PlayerJoinEvent evt)
        {
            matchManager.AddPlayer(evt.PlayerRef);
            
            // 如果达到开始条件，自动开始游戏
            if (matchManager.CanStartGame())
            {
                StartGame();
            }
        }

        private void OnPlayerLeave(PlayerLeaveEvent evt)
        {
            matchManager.RemovePlayer(evt.PlayerRef);
            
            // 检查是否需要结束游戏
            if (!matchManager.HasEnoughPlayers())
            {
                EndGame(GameOverReason.NotEnoughPlayers);
            }
        }

        private void OnPlayerDeath(PlayerDeathEvent evt)
        {
            scoreManager.HandlePlayerDeath(evt);
            
            // 检查是否达到游戏结束条件
            CheckGameEndCondition();
        }

        private void OnGameOver(GameOverEvent evt)
        {
            EndGame(evt.Reason);
        }
        #endregion

        private void CheckGameEndCondition()
        {
            // 检查是否有玩家达到胜利分数
            var scores = scoreManager.GetScores();
            foreach (var score in scores.Values)
            {
                if (score.Score >= gameConfig.RuleConfig.ScoreToWin)
                {
                    EndGame(GameOverReason.VictoryConditionMet);
                    return;
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!IsGameStarted || IsGamePaused) return;

            GameTime += Time.deltaTime;
            
            // 更新各个管理器
            stateManager.Update();
            matchManager.Update();
            
            // 检查时间限制
            if (GameTime >= gameConfig.RuleConfig.TimeLimit)
            {
                EndGame(GameOverReason.TimeUp);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 清理事件监听
            EventManager.Instance.RemoveListener<PlayerJoinEvent>(OnPlayerJoin);
            EventManager.Instance.RemoveListener<PlayerLeaveEvent>(OnPlayerLeave);
            EventManager.Instance.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
            EventManager.Instance.RemoveListener<GameOverEvent>(OnGameOver);

            // 清理管理器
            stateManager?.Dispose();
            matchManager?.Dispose();
            scoreManager?.Dispose();
        }
    }
} 