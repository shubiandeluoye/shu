using UnityEngine;
using Fusion;

namespace Core.Network
{
    public class NetworkRoomSystem : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<int, RoomData> Rooms { get; }
        [Networked] private NetworkDictionary<PlayerRef, int> PlayerRooms { get; }
        [Networked] private int NextRoomId { get; set; }

        public struct RoomData : INetworkStruct
        {
            public NetworkString<_32> RoomName;
            public NetworkString<_16> GameMode;
            public NetworkString<_16> MapId;
            public byte MaxPlayers;
            public byte CurrentPlayers;
            public NetworkBool IsPrivate;
            public NetworkBool IsLocked;
            public NetworkBool InGame;
            public float CreationTime;
        }

        public int CreateRoom(string roomName, string gameMode, string mapId, byte maxPlayers, bool isPrivate)
        {
            if (!Runner.IsServer) return -1;

            int roomId = NextRoomId++;
            var roomData = new RoomData
            {
                RoomName = roomName,
                GameMode = gameMode,
                MapId = mapId,
                MaxPlayers = maxPlayers,
                CurrentPlayers = 0,
                IsPrivate = isPrivate,
                IsLocked = false,
                InGame = false,
                CreationTime = Runner.SimulationTime
            };

            Rooms.Set(roomId, roomData);
            RPC_OnRoomCreated(roomId, roomData);

            return roomId;
        }

        public bool JoinRoom(PlayerRef player, int roomId)
        {
            if (!Runner.IsServer) return false;
            if (!Rooms.TryGet(roomId, out var roomData)) return false;
            if (roomData.IsLocked || roomData.InGame) return false;
            if (roomData.CurrentPlayers >= roomData.MaxPlayers) return false;

            // 如果玩家已在其他房间，先离开
            if (PlayerRooms.TryGet(player, out var currentRoomId))
            {
                LeaveRoom(player);
            }

            roomData.CurrentPlayers++;
            Rooms.Set(roomId, roomData);
            PlayerRooms.Set(player, roomId);

            RPC_OnPlayerJoinedRoom(player, roomId);
            return true;
        }

        public void LeaveRoom(PlayerRef player)
        {
            if (!Runner.IsServer) return;
            if (!PlayerRooms.TryGet(player, out var roomId)) return;
            if (!Rooms.TryGet(roomId, out var roomData)) return;

            roomData.CurrentPlayers = (byte)Mathf.Max(0, roomData.CurrentPlayers - 1);
            Rooms.Set(roomId, roomData);
            PlayerRooms.Remove(player);

            RPC_OnPlayerLeftRoom(player, roomId);

            // 如果房间空了，删除房间
            if (roomData.CurrentPlayers == 0)
            {
                RemoveRoom(roomId);
            }
        }

        public void UpdateRoomSettings(int roomId, string mapId, byte maxPlayers, bool isPrivate)
        {
            if (!Runner.IsServer) return;
            if (!Rooms.TryGet(roomId, out var roomData)) return;

            roomData.MapId = mapId;
            roomData.MaxPlayers = maxPlayers;
            roomData.IsPrivate = isPrivate;

            Rooms.Set(roomId, roomData);
            RPC_OnRoomSettingsUpdated(roomId, roomData);
        }

        public void SetRoomState(int roomId, bool inGame, bool locked)
        {
            if (!Runner.IsServer) return;
            if (!Rooms.TryGet(roomId, out var roomData)) return;

            roomData.InGame = inGame;
            roomData.IsLocked = locked;

            Rooms.Set(roomId, roomData);
            RPC_OnRoomStateChanged(roomId, inGame, locked);
        }

        private void RemoveRoom(int roomId)
        {
            if (!Runner.IsServer) return;
            
            Rooms.Remove(roomId);
            RPC_OnRoomRemoved(roomId);
        }

        // 数据访问方法
        public RoomData? GetRoomData(int roomId)
        {
            return Rooms.TryGet(roomId, out var data) ? data : null;
        }

        public int? GetPlayerRoom(PlayerRef player)
        {
            return PlayerRooms.TryGet(player, out var roomId) ? roomId : null;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnRoomCreated(int roomId, RoomData data)
        {
            Rooms.Set(roomId, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerJoinedRoom(PlayerRef player, int roomId)
        {
            if (Rooms.TryGet(roomId, out var roomData))
            {
                roomData.CurrentPlayers++;
                Rooms.Set(roomId, roomData);
                PlayerRooms.Set(player, roomId);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerLeftRoom(PlayerRef player, int roomId)
        {
            if (Rooms.TryGet(roomId, out var roomData))
            {
                roomData.CurrentPlayers = (byte)Mathf.Max(0, roomData.CurrentPlayers - 1);
                Rooms.Set(roomId, roomData);
                PlayerRooms.Remove(player);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnRoomSettingsUpdated(int roomId, RoomData data)
        {
            Rooms.Set(roomId, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnRoomStateChanged(int roomId, NetworkBool inGame, NetworkBool locked)
        {
            if (Rooms.TryGet(roomId, out var roomData))
            {
                roomData.InGame = inGame;
                roomData.IsLocked = locked;
                Rooms.Set(roomId, roomData);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnRoomRemoved(int roomId)
        {
            Rooms.Remove(roomId);
        }
    }
} 