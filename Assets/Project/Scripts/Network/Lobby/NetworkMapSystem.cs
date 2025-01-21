using UnityEngine;
using Fusion;

namespace Core.Network
{
    public class NetworkMapSystem : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<NetworkString<_16>, MapData> AvailableMaps { get; }
        [Networked] private NetworkDictionary<int, VoteData> MapVotes { get; }
        [Networked] private NetworkString<_16> CurrentMapId { get; set; }
        [Networked] private float VotingEndTime { get; set; }
        [Networked] private NetworkBool IsVotingActive { get; set; }

        public struct MapData : INetworkStruct
        {
            public NetworkString<_32> MapName;
            public NetworkString<_16> GameMode;
            public byte MaxPlayers;
            public NetworkBool IsAvailable;
            public NetworkBool IsRotationEnabled;
        }

        public struct VoteData : INetworkStruct
        {
            public NetworkString<_16> MapId;
            public int VoteCount;
            // 改用固定字段存储投票者，最多16人
            public PlayerRef Voter0;
            public PlayerRef Voter1;
            public PlayerRef Voter2;
            public PlayerRef Voter3;
            public PlayerRef Voter4;
            public PlayerRef Voter5;
            public PlayerRef Voter6;
            public PlayerRef Voter7;
            public PlayerRef Voter8;
            public PlayerRef Voter9;
            public PlayerRef Voter10;
            public PlayerRef Voter11;
            public PlayerRef Voter12;
            public PlayerRef Voter13;
            public PlayerRef Voter14;
            public PlayerRef Voter15;
            public byte VoterCount;  // 记录实际投票人数

            // 辅助方法
            public PlayerRef GetVoter(int index)
            {
                if (index < 0 || index >= 16) return default;
                switch (index)
                {
                    case 0: return Voter0;
                    case 1: return Voter1;
                    case 2: return Voter2;
                    case 3: return Voter3;
                    case 4: return Voter4;
                    case 5: return Voter5;
                    case 6: return Voter6;
                    case 7: return Voter7;
                    case 8: return Voter8;
                    case 9: return Voter9;
                    case 10: return Voter10;
                    case 11: return Voter11;
                    case 12: return Voter12;
                    case 13: return Voter13;
                    case 14: return Voter14;
                    case 15: return Voter15;
                    default: return default;
                }
            }

            public void SetVoter(int index, PlayerRef value)
            {
                if (index < 0 || index >= 16) return;
                switch (index)
                {
                    case 0: Voter0 = value; break;
                    case 1: Voter1 = value; break;
                    case 2: Voter2 = value; break;
                    case 3: Voter3 = value; break;
                    case 4: Voter4 = value; break;
                    case 5: Voter5 = value; break;
                    case 6: Voter6 = value; break;
                    case 7: Voter7 = value; break;
                    case 8: Voter8 = value; break;
                    case 9: Voter9 = value; break;
                    case 10: Voter10 = value; break;
                    case 11: Voter11 = value; break;
                    case 12: Voter12 = value; break;
                    case 13: Voter13 = value; break;
                    case 14: Voter14 = value; break;
                    case 15: Voter15 = value; break;
                }
            }
        }

        public void RegisterMap(string mapId, string mapName, string gameMode, byte maxPlayers)
        {
            if (!Runner.IsServer) return;

            var mapData = new MapData
            {
                MapName = mapName,
                GameMode = gameMode,
                MaxPlayers = maxPlayers,
                IsAvailable = true,
                IsRotationEnabled = true
            };

            AvailableMaps.Set(mapId, mapData);
            RPC_OnMapRegistered(mapId, mapData);
        }

        public void SetMapAvailability(string mapId, bool isAvailable, bool inRotation)
        {
            if (!Runner.IsServer) return;
            if (!AvailableMaps.TryGet(mapId, out var mapData)) return;

            mapData.IsAvailable = isAvailable;
            mapData.IsRotationEnabled = inRotation;
            AvailableMaps.Set(mapId, mapData);
            RPC_OnMapAvailabilityChanged(mapId, isAvailable, inRotation);
        }

        public void StartMapVoting(float duration)
        {
            if (!Runner.IsServer) return;

            MapVotes.Clear();
            IsVotingActive = true;
            VotingEndTime = Runner.SimulationTime + duration;

            // 初始化所有可用地图的投票数据
            int voteId = 0;
            foreach (var kvp in AvailableMaps)
            {
                if (kvp.Value.IsAvailable && kvp.Value.IsRotationEnabled)
                {
                    var voteData = new VoteData
                    {
                        MapId = kvp.Key,
                        VoteCount = 0,
                        VoterCount = 0
                        // 所有Voter字段默认为default
                    };
                    MapVotes.Set(voteId++, voteData);
                }
            }

            RPC_OnVotingStarted(duration);
        }

        public void CastVote(PlayerRef player, int voteId)
        {
            if (!Runner.IsServer) return;
            if (!MapVotes.TryGet(voteId, out var voteData)) return;

            // 检查是否已经投过票
            for (int i = 0; i < voteData.VoterCount; i++)
            {
                if (voteData.GetVoter(i) == player)
                {
                    return; // 已经投过票了
                }
            }

            // 添加新投票
            if (voteData.VoterCount < 16)
            {
                voteData.SetVoter(voteData.VoterCount, player);
                voteData.VoterCount++;
                voteData.VoteCount++;
                MapVotes.Set(voteId, voteData);
                RPC_OnVoteCast(voteId, voteData);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer) return;

            if (IsVotingActive && Runner.SimulationTime >= VotingEndTime)
            {
                EndVoting();
            }
        }

        private void EndVoting()
        {
            if (!Runner.IsServer) return;

            // 找出得票最多的地图
            int highestVotes = -1;
            NetworkString<_16> winningMapId = default;

            foreach (var kvp in MapVotes)
            {
                if (kvp.Value.VoteCount > highestVotes)
                {
                    highestVotes = kvp.Value.VoteCount;
                    winningMapId = kvp.Value.MapId;
                }
            }

            IsVotingActive = false;
            CurrentMapId = winningMapId;
            MapVotes.Clear();

            RPC_OnVotingEnded(winningMapId);
        }

        // 数据访问方法
        public MapData? GetMapData(string mapId)
        {
            return AvailableMaps.TryGet(mapId, out var data) ? data : null;
        }

        public VoteData? GetVoteData(int voteId)
        {
            return MapVotes.TryGet(voteId, out var data) ? data : null;
        }

        public string GetCurrentMapId()
        {
            return CurrentMapId.ToString();
        }

        public bool IsVotingInProgress()
        {
            return IsVotingActive;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnMapRegistered(NetworkString<_16> mapId, MapData data)
        {
            AvailableMaps.Set(mapId, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnMapAvailabilityChanged(NetworkString<_16> mapId, NetworkBool isAvailable, NetworkBool inRotation)
        {
            if (AvailableMaps.TryGet(mapId, out var mapData))
            {
                mapData.IsAvailable = isAvailable;
                mapData.IsRotationEnabled = inRotation;
                AvailableMaps.Set(mapId, mapData);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnVotingStarted(float duration)
        {
            IsVotingActive = true;
            VotingEndTime = Runner.SimulationTime + duration;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnVoteCast(int voteId, VoteData data)
        {
            MapVotes.Set(voteId, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnVotingEnded(NetworkString<_16> winningMapId)
        {
            IsVotingActive = false;
            CurrentMapId = winningMapId;
            MapVotes.Clear();
        }
    }
}
