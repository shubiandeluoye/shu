using UnityEngine;
using Fusion;

namespace Core.Network
{
    /// <summary>
    /// 游戏状态同步组件
    /// 负责：
    /// 1. 游戏状态的同步
    /// 2. 回合计时同步
    /// 3. 胜负条件检查
    /// 4. 游戏进程控制
    /// </summary>
    public class NetworkStateSync : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<PlayerRef, PlayerGameState> GameStates { get; }
        [Networked] private GameState CurrentGameState { get; set; }
        [Networked] private float StateTimer { get; set; }
        [Networked] private NetworkBool IsPaused { get; set; }

        public enum GameState
        {
            WaitingForPlayers,
            Preparation,
            InProgress,
            RoundEnd,
            GameOver
        }

        public struct PlayerGameState : INetworkStruct
        {
            public NetworkBool IsLoaded;
            public NetworkBool IsReady;
            public float LoadProgress;
            public int Score;
            public int RoundWins;
        }

        public void SetGameState(GameState newState)
        {
            if (!Runner.IsServer) return;

            CurrentGameState = newState;
            StateTimer = 0;
            RPC_OnGameStateChanged(newState);
        }

        public void UpdatePlayerState(PlayerRef player, bool isLoaded, bool isReady, float loadProgress)
        {
            if (!Runner.IsServer) return;

            var state = GameStates.TryGet(player, out var existingState) ? existingState : new PlayerGameState();
            state.IsLoaded = isLoaded;
            state.IsReady = isReady;
            state.LoadProgress = loadProgress;
            
            GameStates.Set(player, state);
            RPC_OnPlayerStateUpdated(player, state);
        }

        public void UpdatePlayerScore(PlayerRef player, int score, int roundWins)
        {
            if (!Runner.IsServer) return;

            if (GameStates.TryGet(player, out var state))
            {
                state.Score = score;
                state.RoundWins = roundWins;
                GameStates.Set(player, state);
                RPC_OnPlayerScoreUpdated(player, score, roundWins);
            }
        }

        public void SetPauseState(bool isPaused)
        {
            if (!Runner.IsServer) return;

            IsPaused = isPaused;
            RPC_OnPauseStateChanged(isPaused);
        }

        public void RemovePlayerState(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            GameStates.Remove(player);
            RPC_OnPlayerStateRemoved(player);
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.IsServer && !IsPaused)
            {
                StateTimer += Runner.DeltaTime;
            }
        }

        // 数据访问方法
        public GameState GetCurrentGameState() => CurrentGameState;
        public float GetStateTimer() => StateTimer;
        public bool GetPauseState() => IsPaused;
        
        public PlayerGameState? GetPlayerGameState(PlayerRef player)
        {
            return GameStates.TryGet(player, out var state) ? state : null;
        }

        public bool AreAllPlayersReady()
        {
            foreach (var kvp in GameStates)
            {
                if (!kvp.Value.IsReady) return false;
            }
            return true;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnGameStateChanged(GameState newState)
        {
            CurrentGameState = newState;
            StateTimer = 0;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerStateUpdated(PlayerRef player, PlayerGameState state)
        {
            GameStates.Set(player, state);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerScoreUpdated(PlayerRef player, int score, int roundWins)
        {
            if (GameStates.TryGet(player, out var state))
            {
                state.Score = score;
                state.RoundWins = roundWins;
                GameStates.Set(player, state);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPauseStateChanged(NetworkBool isPaused)
        {
            IsPaused = isPaused;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerStateRemoved(PlayerRef player)
        {
            GameStates.Remove(player);
        }
    }
} 