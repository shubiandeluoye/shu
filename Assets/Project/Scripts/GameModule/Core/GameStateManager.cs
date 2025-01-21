using UnityEngine;
using Core.EventSystem;
using GameModule.Data;
using Fusion;

namespace GameModule.Core
{
    /// <summary>
    /// 游戏状态管理器
    /// 负责：
    /// 1. 管理游戏状态
    /// 2. 处理状态转换
    /// 3. 通知状态变化
    /// </summary>
    public class GameStateManager
    {
        private readonly StateConfig config;
        private GameState currentState;
        private float stateTimer;

        public GameState CurrentState => currentState;

        public GameStateManager(StateConfig config)
        {
            this.config = config;
            currentState = GameState.None;
            stateTimer = 0;
        }

        public void SetState(GameState newState)
        {
            if (currentState == newState) return;

            var oldState = currentState;
            currentState = newState;
            stateTimer = 0;

            // 处理状态进入逻辑
            OnStateEnter(newState);

            // 通知状态变化
            EventManager.Instance.TriggerEvent(new GameStateChangeEvent 
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
                    HandleWaitingState();
                    break;
                case GameState.Countdown:
                    HandleCountdownState();
                    break;
                case GameState.Playing:
                    HandlePlayingState();
                    break;
                case GameState.GameOver:
                    HandleGameOverState();
                    break;
            }
        }

        private void HandleWaitingState()
        {
            // 等待玩家加入的逻辑
        }

        private void HandleCountdownState()
        {
            // 开始倒计时
            EventManager.Instance.TriggerEvent(new GameCountdownEvent 
            { 
                Duration = config.CountdownDuration 
            });
        }

        private void HandlePlayingState()
        {
            // 游戏开始时的初始化
        }

        private void HandleGameOverState()
        {
            // 游戏结束时的清理
        }

        public void Update()
        {
            stateTimer += Time.deltaTime;

            // 处理特定状态的计时逻辑
            switch (currentState)
            {
                case GameState.Countdown:
                    UpdateCountdown();
                    break;
            }
        }

        private void UpdateCountdown()
        {
            if (stateTimer >= config.CountdownDuration)
            {
                SetState(GameState.Playing);
            }
        }

        public void Dispose()
        {
            // 清理资源
        }
    }
} 