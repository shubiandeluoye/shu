using UnityEngine;
using Fusion;

namespace Core.Network
{
    public class NetworkSpawnSystem : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<int, SpawnPointData> SpawnPoints { get; }
        [Networked] private NetworkDictionary<PlayerRef, SpawnState> PlayerSpawns { get; }
        [Networked] private int NextSpawnId { get; set; }

        public struct SpawnPointData : INetworkStruct
        {
            public Vector3 Position;
            public int TeamId;
            public NetworkBool IsOccupied;
            public float LastUseTime;
        }

        public struct SpawnState : INetworkStruct
        {
            public int CurrentSpawnId;
            public float RespawnTime;
            public NetworkBool IsSpawning;
        }

        // 主要功能实现
        public void RegisterSpawnPoint(Vector3 position, int teamId)
        {
            if (!Runner.IsServer) return;
            
            var spawnData = new SpawnPointData
            {
                Position = position,
                TeamId = teamId,
                IsOccupied = false,
                LastUseTime = 0
            };
            
            SpawnPoints.Set(NextSpawnId++, spawnData);
        }

        public Vector3? GetSpawnPoint(PlayerRef player, int teamId)
        {
            if (!Runner.IsServer) return null;

            // 找到最适合的重生点
            float currentTime = Runner.SimulationTime;
            float bestScore = float.MinValue;
            int bestSpawnId = -1;

            foreach (var kvp in SpawnPoints)
            {
                var spawnData = kvp.Value;
                if (spawnData.TeamId != teamId || spawnData.IsOccupied) continue;

                float score = EvaluateSpawnPoint(spawnData, currentTime);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestSpawnId = kvp.Key;
                }
            }

            if (bestSpawnId != -1)
            {
                var spawnData = SpawnPoints.Get(bestSpawnId);
                spawnData.IsOccupied = true;
                spawnData.LastUseTime = currentTime;
                SpawnPoints.Set(bestSpawnId, spawnData);

                var spawnState = new SpawnState
                {
                    CurrentSpawnId = bestSpawnId,
                    RespawnTime = currentTime,
                    IsSpawning = true
                };
                PlayerSpawns.Set(player, spawnState);

                return spawnData.Position;
            }

            return null;
        }

        private float EvaluateSpawnPoint(SpawnPointData spawnData, float currentTime)
        {
            float timeSinceLastUse = currentTime - spawnData.LastUseTime;
            return timeSinceLastUse;
        }

        public void ReleaseSpawnPoint(PlayerRef player)
        {
            if (!Runner.IsServer) return;
            if (!PlayerSpawns.TryGet(player, out var spawnState)) return;

            if (SpawnPoints.TryGet(spawnState.CurrentSpawnId, out var spawnData))
            {
                spawnData.IsOccupied = false;
                SpawnPoints.Set(spawnState.CurrentSpawnId, spawnData);
            }

            PlayerSpawns.Remove(player);
        }
    }
} 