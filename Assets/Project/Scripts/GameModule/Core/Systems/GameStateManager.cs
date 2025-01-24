using GameModule.Core.Data;
using GameModule.Core.Events;
using System.Collections.Generic;

namespace GameModule.Core.Systems
{
    public class GameStateManager
    {
        private readonly StateConfig config;
        private readonly IEventSystem eventSystem;
        private GameState currentState;
        private float stateTimer;
        private float warmupTimer;
        private RoundState currentRound;
        private bool isWarmupComplete;

        public GameState CurrentState => currentState;

        public GameStateManager(StateConfig config, IEventSystem eventSystem)
        {
            this.config = config;
            this.eventSystem = eventSystem;
            Reset();
        }

        public void Reset()
        {
            currentState = GameState.None;
            stateTimer = 0;
            warmupTimer = 0;
            currentRound = null;
            isWarmupComplete = false;
        }

        #region 状态管理
        public void SetState(GameState newState)
        {
            if (currentState == newState) return;

            var oldState = currentState;
            currentState = newState;
            stateTimer = 0;

            OnStateEnter(newState);
            eventSystem.Publish(new GameStateChangeEvent 
            { 
                OldState = oldState,
                NewState = newState
            });
        }

        private void OnStateEnter(GameState state)
        {
            switch (state)
            {
                case GameState.WaitingForPlayers:
                    break;
                    
                case GameState.Warmup:
                    StartWarmup();
                    break;
                    
                case GameState.Countdown:
                    eventSystem.Publish(new GameCountdownEvent 
                    { 
                        Duration = config.CountdownDuration 
                    });
                    break;
                    
                case GameState.RoundStarting:
                    OnRoundStarting();
                    break;
                    
                case GameState.Playing:
                    break;
                    
                case GameState.RoundEnding:
                    OnRoundEnding();
                    break;
            }
        }
        #endregion

        #region 热身阶段
        private void StartWarmup()
        {
            warmupTimer = config.WarmupTime;
            isWarmupComplete = false;
            
            eventSystem.Publish(new WarmupStartEvent 
            { 
                WarmupTime = warmupTimer 
            });
        }

        private void UpdateWarmup(float deltaTime)
        {
            if (isWarmupComplete) return;

            warmupTimer -= deltaTime;
            if (warmupTimer <= 0)
            {
                CompleteWarmup();
            }
        }

        private void CompleteWarmup()
        {
            isWarmupComplete = true;
            SetState(GameState.Countdown);
        }
        #endregion

        #region 回合管理
        public void StartRound(int roundNumber)
        {
            currentRound = new RoundState
            {
                CurrentRound = roundNumber,
                RoundTime = config.RoundTime,
                RoundScores = new Dictionary<int, int>()
            };

            SetState(GameState.RoundStarting);
        }

        private void OnRoundStarting()
        {
            eventSystem.Publish(new RoundStartEvent
            {
                RoundNumber = currentRound.CurrentRound,
                RoundTime = currentRound.RoundTime
            });

            // 短暂延迟后开始回合
            stateTimer = config.RoundStartDelay;
        }

        public void EndRound()
        {
            if (currentRound == null) return;
            SetState(GameState.RoundEnding);
        }

        private void OnRoundEnding()
        {
            eventSystem.Publish(new RoundEndEvent
            {
                RoundNumber = currentRound.CurrentRound,
                RoundScores = currentRound.RoundScores
            });

            // 短暂延迟后结束回合
            stateTimer = config.RoundEndDelay;
        }

        private void UpdateRound(float deltaTime)
        {
            if (currentRound == null) return;

            currentRound.RoundTime -= deltaTime;
            if (currentRound.RoundTime <= 0)
            {
                EndRound();
            }
        }
        #endregion

        public void Update(float deltaTime)
        {
            stateTimer += deltaTime;

            switch (currentState)
            {
                case GameState.Warmup:
                    UpdateWarmup(deltaTime);
                    break;
                    
                case GameState.Countdown:
                    if (stateTimer >= config.CountdownDuration)
                    {
                        SetState(GameState.Playing);
                    }
                    break;
                    
                case GameState.RoundStarting:
                    if (stateTimer >= config.RoundStartDelay)
                    {
                        SetState(GameState.Playing);
                    }
                    break;
                    
                case GameState.Playing:
                    UpdateRound(deltaTime);
                    break;
                    
                case GameState.RoundEnding:
                    if (stateTimer >= config.RoundEndDelay)
                    {
                        SetState(GameState.Countdown);
                    }
                    break;
            }
        }

        public void Dispose()
        {
            Reset();
        }
    }
} 