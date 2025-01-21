using UnityEngine;
using Fusion;

namespace Core.Network
{
    public class NetworkPlayerSync : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<PlayerRef, PlayerData> Players { get; }
        [Networked] private NetworkDictionary<PlayerRef, PlayerState> PlayerStates { get; }

        public struct PlayerData : INetworkStruct
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public int TeamId;
            public NetworkBool IsAlive;
            public float Health;
            public float Energy;
        }

        public struct PlayerState : INetworkStruct
        {
            public NetworkBool IsReady;
            public NetworkBool IsSpawned;
            public NetworkBool IsConnected;
            public float LastUpdateTime;
        }

        public void RegisterPlayer(PlayerRef player, Vector3 spawnPosition, int teamId)
        {
            if (!Runner.IsServer) return;

            var playerData = new PlayerData
            {
                Position = spawnPosition,
                Rotation = Quaternion.identity,
                TeamId = teamId,
                IsAlive = true,
                Health = 100f,
                Energy = 100f
            };

            var playerState = new PlayerState
            {
                IsReady = false,
                IsSpawned = true,
                IsConnected = true,
                LastUpdateTime = Runner.SimulationTime
            };

            Players.Set(player, playerData);
            PlayerStates.Set(player, playerState);
            RPC_OnPlayerRegistered(player, playerData, playerState);
        }

        public void UpdatePlayerPosition(PlayerRef player, Vector3 position, Quaternion rotation)
        {
            if (!Runner.IsServer) return;

            if (Players.TryGet(player, out var playerData))
            {
                playerData.Position = position;
                playerData.Rotation = rotation;
                Players.Set(player, playerData);
                RPC_OnPlayerPositionUpdated(player, position, rotation);
            }
        }

        public void UpdatePlayerStatus(PlayerRef player, float health, float energy)
        {
            if (!Runner.IsServer) return;

            if (Players.TryGet(player, out var playerData))
            {
                playerData.Health = health;
                playerData.Energy = energy;
                playerData.IsAlive = health > 0;
                Players.Set(player, playerData);
                RPC_OnPlayerStatusUpdated(player, health, energy);
            }
        }

        public void SetPlayerReady(PlayerRef player, bool isReady)
        {
            if (!Runner.IsServer) return;

            if (PlayerStates.TryGet(player, out var state))
            {
                state.IsReady = isReady;
                state.LastUpdateTime = Runner.SimulationTime;
                PlayerStates.Set(player, state);
                RPC_OnPlayerReadyStateChanged(player, isReady);
            }
        }

        public void RemovePlayer(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            Players.Remove(player);
            PlayerStates.Remove(player);
            RPC_OnPlayerRemoved(player);
        }

        public PlayerData? GetPlayerData(PlayerRef player)
        {
            return Players.TryGet(player, out var data) ? data : null;
        }

        public PlayerState? GetPlayerState(PlayerRef player)
        {
            return PlayerStates.TryGet(player, out var state) ? state : null;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerRegistered(PlayerRef player, PlayerData data, PlayerState state)
        {
            Players.Set(player, data);
            PlayerStates.Set(player, state);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerPositionUpdated(PlayerRef player, Vector3 position, Quaternion rotation)
        {
            if (Players.TryGet(player, out var playerData))
            {
                playerData.Position = position;
                playerData.Rotation = rotation;
                Players.Set(player, playerData);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerStatusUpdated(PlayerRef player, float health, float energy)
        {
            if (Players.TryGet(player, out var playerData))
            {
                playerData.Health = health;
                playerData.Energy = energy;
                playerData.IsAlive = health > 0;
                Players.Set(player, playerData);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerReadyStateChanged(PlayerRef player, NetworkBool isReady)
        {
            if (PlayerStates.TryGet(player, out var state))
            {
                state.IsReady = isReady;
                state.LastUpdateTime = Runner.SimulationTime;
                PlayerStates.Set(player, state);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerRemoved(PlayerRef player)
        {
            Players.Remove(player);
            PlayerStates.Remove(player);
        }
    }
} 