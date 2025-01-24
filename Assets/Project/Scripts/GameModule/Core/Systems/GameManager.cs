using System;
using GameModule.Core.Data;
using GameModule.Core.Events;
using GameModule.Core.Network;
using UnityEngine;
using System.Collections.Generic;

namespace GameModule.Core.Systems
{
    public class GameManager
    {
        private readonly GameConfig config;
        private readonly GameStateManager stateManager;
        private readonly MatchManager matchManager;
        private readonly ScoreManager scoreManager;
        private readonly IEventSystem eventSystem;
        private readonly INetworkAuthority networkAuthority;

        private bool isGameStarted;
        private bool isGamePaused;
        private float gameTime;

        private GameMode currentMode;
        private GamePhase currentPhase;
        private bool isWarmupPhase;
        private RoundState currentRound;

        public GameManager(IEventSystem eventSystem, INetworkAuthority networkAuthority)
        {
            this.eventSystem = eventSystem;
            this.networkAuthority = networkAuthority;
            SubscribeToEvents();
        }

        public void Initialize(GameConfig config)
        {
            this.config = config;
            stateManager = new GameStateManager(config.StateConfig, eventSystem);
            matchManager = new MatchManager(config.MatchConfig, eventSystem);
            scoreManager = new ScoreManager(config.ScoreConfig, eventSystem);
            
            currentMode = config.GameModeConfig.GameMode;
            currentPhase = GamePhase.Early;
            isWarmupPhase = config.GameModeConfig.EnablePracticeMode;
        }

        #region 游戏流程控制
        public void StartGame()
        {
            if (!networkAuthority.HasStateAuthority) return;
            if (isGameStarted) return;

            isGameStarted = true;
            isGamePaused = false;
            gameTime = 0;

            stateManager.SetState(GameState.Playing);
            
            eventSystem.Publish(new GameStartEvent());
        }

        public void PauseGame()
        {
            if (!isGameStarted) return;
            
            isGamePaused = !isGamePaused;
            eventSystem.Publish(new GamePauseEvent { IsPaused = isGamePaused });
        }

        public void EndGame(GameOverReason reason)
        {
            if (!isGameStarted) return;

            isGameStarted = false;
            var finalScores = scoreManager.GetFinalScores();
            
            eventSystem.Publish(new GameEndEvent 
            { 
                Reason = reason,
                FinalScores = finalScores
            });

            stateManager.SetState(GameState.GameOver);
        }
        #endregion

        #region 事件处理
        private void SubscribeToEvents()
        {
            eventSystem.Subscribe<PlayerJoinEvent>(OnPlayerJoin);
            eventSystem.Subscribe<PlayerLeaveEvent>(OnPlayerLeave);
            eventSystem.Subscribe<PlayerDeathEvent>(OnPlayerDeath);
            eventSystem.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnPlayerJoin(PlayerJoinEvent evt)
        {
            matchManager.AddPlayer(evt.PlayerId);
            
            if (matchManager.CanStartGame())
            {
                StartGame();
            }
        }

        private void OnPlayerLeave(PlayerLeaveEvent evt)
        {
            matchManager.RemovePlayer(evt.PlayerId);
            
            if (!matchManager.HasEnoughPlayers())
            {
                EndGame(GameOverReason.NotEnoughPlayers);
            }
        }

        private void OnPlayerDeath(PlayerDeathEvent evt)
        {
            scoreManager.HandlePlayerDeath(evt);
            CheckGameEndCondition();
        }

        private void OnGameOver(GameOverEvent evt)
        {
            EndGame(evt.Reason);
        }
        #endregion

        private void CheckGameEndCondition()
        {
            // 检查分数胜利
            var scores = scoreManager.GetScores();
            foreach (var score in scores.Values)
            {
                // 胜利条件
                if (score.Score >= config.RuleConfig.ScoreToWin)
                {
                    EndGame(GameOverReason.VictoryConditionMet);
                    return;
                }
                
                // 失败条件
                if (score.Score <= config.RuleConfig.MinScoreToEnd)
                {
                    EndGame(GameOverReason.ScoreTooLow);
                    return;
                }

                // 死亡次数限制
                if (score.Deaths >= config.RuleConfig.MaxDeaths)
                {
                    EndGame(GameOverReason.MaxDeathsReached);
                    return;
                }
            }

            // 检查队伍胜利（如果开启队伍模式）
            if (config.RuleConfig.EnableTeamMode)
            {
                CheckTeamVictory();
            }
        }

        private void CheckTeamVictory()
        {
            var teamScores = new Dictionary<int, int>();
            foreach (var score in scoreManager.GetScores().Values)
            {
                var teamId = matchManager.GetPlayerTeam(score.PlayerId);
                if (!teamScores.ContainsKey(teamId))
                {
                    teamScores[teamId] = 0;
                }
                teamScores[teamId] += score.Score;
            }

            foreach (var teamScore in teamScores)
            {
                if (teamScore.Value >= config.RuleConfig.ScoreToWin)
                {
                    EndGame(GameOverReason.TeamVictory);
                    return;
                }
            }
        }

        private void CheckOutOfBounds()
        {
            foreach (var playerId in matchManager.GetAllPlayers())
            {
                var position = matchManager.GetPlayerPosition(playerId);
                if (!IsInBounds(position))
                {
                    ApplyOutOfBoundsDamage(playerId);
                }
            }
        }

        private bool IsInBounds(Vector2 position)
        {
            // 检查玩家是否在有效区域内
            return true; // 具体实现依赖地图边界定义
        }

        private void ApplyOutOfBoundsDamage(int playerId)
        {
            // 对出界玩家造成伤害
            var damage = config.RuleConfig.OutOfBoundsDamage * Time.deltaTime;
            // 应用伤害...
        }

        #region 游戏模式控制
        public void SetGameMode(GameMode mode)
        {
            if (!networkAuthority.HasStateAuthority) return;
            
            var oldMode = currentMode;
            currentMode = mode;
            eventSystem.Publish(new GameModeChangedEvent
            {
                OldMode = oldMode,
                NewMode = mode
            });

            // 根据模式调整配置
            AdjustConfigForMode(mode);
        }

        private void AdjustConfigForMode(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Practice:
                    config.RuleConfig.EnableRespawn = true;
                    config.RuleConfig.TimeLimit = float.MaxValue;
                    break;
                case GameMode.Tournament:
                    config.RuleConfig.EnableRespawn = false;
                    config.GameModeConfig.AllowLateJoin = false;
                    break;
            }
        }
        #endregion

        #region 回合管理
        public void StartRound()
        {
            if (!networkAuthority.HasStateAuthority) return;

            currentRound = new RoundState
            {
                CurrentRound = currentRound?.CurrentRound + 1 ?? 1,
                RoundTime = config.GameModeConfig.RoundTime,
                RoundScores = new Dictionary<int, int>()
            };

            scoreManager.StartNewRound(currentRound.CurrentRound);
            stateManager.StartRound(currentRound.CurrentRound);
        }

        public void EndRound()
        {
            if (!networkAuthority.HasStateAuthority) return;

            stateManager.EndRound();
            
            // 检查是否需要开始新回合或结束游戏
            if (currentRound.CurrentRound >= config.GameModeConfig.RoundCount)
            {
                EndGame(GameOverReason.RoundComplete);
            }
            else
            {
                StartRound();
            }
        }
        #endregion

        #region 游戏进度
        private void UpdateGameProgress()
        {
            float progress = gameTime / config.RuleConfig.TimeLimit;
            var newPhase = GetGamePhase(progress);
            
            if (newPhase != currentPhase)
            {
                currentPhase = newPhase;
                eventSystem.Publish(new GameProgressEvent
                {
                    Progress = progress,
                    CurrentPhase = currentPhase,
                    RemainingTime = config.RuleConfig.TimeLimit - gameTime
                });

                // 根据阶段调整游戏参数
                AdjustGameForPhase(newPhase);
            }
        }

        private GamePhase GetGamePhase(float progress)
        {
            if (progress < 0.25f) return GamePhase.Early;
            if (progress < 0.5f) return GamePhase.Mid;
            if (progress < 0.75f) return GamePhase.Late;
            return GamePhase.Final;
        }

        private void AdjustGameForPhase(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Final:
                    // 最终阶段特殊规则
                    config.ScoreConfig.KillScore *= 2; // 双倍积分
                    break;
            }
        }
        #endregion

        public override void Update(float deltaTime)
        {
            if (!isGameStarted || isGamePaused) return;

            gameTime += deltaTime;
            
            // 更新各个系统
            stateManager.Update(deltaTime);
            matchManager.Update(deltaTime);
            
            // 更新游戏进度
            UpdateGameProgress();
            
            // 更新回合时间
            if (currentRound != null)
            {
                currentRound.RoundTime -= deltaTime;
                if (currentRound.RoundTime <= 0)
                {
                    EndRound();
                }
            }

            // 检查游戏结束条件
            if (gameTime >= config.RuleConfig.TimeLimit)
            {
                EndGame(GameOverReason.TimeUp);
            }
        }

        public void Dispose()
        {
            eventSystem.Unsubscribe<PlayerJoinEvent>(OnPlayerJoin);
            eventSystem.Unsubscribe<PlayerLeaveEvent>(OnPlayerLeave);
            eventSystem.Unsubscribe<PlayerDeathEvent>(OnPlayerDeath);
            eventSystem.Unsubscribe<GameOverEvent>(OnGameOver);

            stateManager?.Dispose();
            matchManager?.Dispose();
            scoreManager?.Dispose();
        }
    }
} 