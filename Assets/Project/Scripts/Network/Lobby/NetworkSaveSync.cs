using UnityEngine;
using Fusion;

namespace Core.Network
{
    public class NetworkSaveSync : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<PlayerRef, PlayerSaveData> PlayerSaves { get; }
        [Networked] private NetworkDictionary<PlayerRef, SaveSyncState> SyncStates { get; }

        public struct PlayerSaveData : INetworkStruct
        {
            // 解锁的物品，最多32个
            public int UnlockedItem0;
            public int UnlockedItem1;
            public int UnlockedItem2;
            public int UnlockedItem3;
            public int UnlockedItem4;
            public int UnlockedItem5;
            public int UnlockedItem6;
            public int UnlockedItem7;
            public int UnlockedItem8;
            public int UnlockedItem9;
            public int UnlockedItem10;
            public int UnlockedItem11;
            public int UnlockedItem12;
            public int UnlockedItem13;
            public int UnlockedItem14;
            public int UnlockedItem15;
            public int UnlockedItem16;
            public int UnlockedItem17;
            public int UnlockedItem18;
            public int UnlockedItem19;
            public int UnlockedItem20;
            public int UnlockedItem21;
            public int UnlockedItem22;
            public int UnlockedItem23;
            public int UnlockedItem24;
            public int UnlockedItem25;
            public int UnlockedItem26;
            public int UnlockedItem27;
            public int UnlockedItem28;
            public int UnlockedItem29;
            public int UnlockedItem30;
            public int UnlockedItem31;

            // 装备的物品，最多8个
            public int EquippedItem0;
            public int EquippedItem1;
            public int EquippedItem2;
            public int EquippedItem3;
            public int EquippedItem4;
            public int EquippedItem5;
            public int EquippedItem6;
            public int EquippedItem7;

            public int Level;
            public int Experience;
            public int Currency;
            public NetworkString<_32> LastUpdateTime;
            public int SaveVersion;

            // 辅助方法
            public int GetUnlockedItem(int index)
            {
                if (index < 0 || index >= 32) return 0;
                switch (index)
                {
                    case 0: return UnlockedItem0;
                    case 1: return UnlockedItem1;
                    // ... 其他case
                    case 31: return UnlockedItem31;
                    default: return 0;
                }
            }

            public int GetEquippedItem(int index)
            {
                if (index < 0 || index >= 8) return 0;
                switch (index)
                {
                    case 0: return EquippedItem0;
                    case 1: return EquippedItem1;
                    case 2: return EquippedItem2;
                    case 3: return EquippedItem3;
                    case 4: return EquippedItem4;
                    case 5: return EquippedItem5;
                    case 6: return EquippedItem6;
                    case 7: return EquippedItem7;
                    default: return 0;
                }
            }

            public void SetUnlockedItem(int index, int value)
            {
                if (index < 0 || index >= 32) return;
                switch (index)
                {
                    case 0: UnlockedItem0 = value; break;
                    case 1: UnlockedItem1 = value; break;
                    // ... 其他case
                    case 31: UnlockedItem31 = value; break;
                }
            }

            public void SetEquippedItem(int index, int value)
            {
                if (index < 0 || index >= 8) return;
                switch (index)
                {
                    case 0: EquippedItem0 = value; break;
                    case 1: EquippedItem1 = value; break;
                    case 2: EquippedItem2 = value; break;
                    case 3: EquippedItem3 = value; break;
                    case 4: EquippedItem4 = value; break;
                    case 5: EquippedItem5 = value; break;
                    case 6: EquippedItem6 = value; break;
                    case 7: EquippedItem7 = value; break;
                }
            }
        }

        public struct SaveSyncState : INetworkStruct
        {
            public NetworkBool IsSyncing;
            public float SyncStartTime;
            public float LastSyncTime;
            public byte SyncProgress;
            public NetworkBool HasError;
            public NetworkString<_32> ErrorMessage;
        }

        public void InitializePlayerSave(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            var saveData = new PlayerSaveData
            {
                Level = 1,
                Experience = 0,
                Currency = 0,
                LastUpdateTime = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                SaveVersion = 1
            };

            // 初始化所有物品为0
            for (int i = 0; i < 32; i++)
            {
                saveData.SetUnlockedItem(i, 0);
            }
            
            for (int i = 0; i < 8; i++)
            {
                saveData.SetEquippedItem(i, 0);
            }

            var syncState = new SaveSyncState
            {
                IsSyncing = false,
                SyncStartTime = 0,
                LastSyncTime = Runner.SimulationTime,
                SyncProgress = 0,
                HasError = false,
                ErrorMessage = ""
            };

            PlayerSaves.Set(player, saveData);
            SyncStates.Set(player, syncState);
            RPC_OnPlayerSaveInitialized(player, saveData, syncState);
        }

        public void UpdatePlayerSave(PlayerRef player, PlayerSaveData saveData)
        {
            if (!Runner.IsServer) return;
            if (!PlayerSaves.TryGet(player, out var currentSave)) return;

            // 验证数据
            if (!ValidateSaveData(saveData)) return;

            saveData.LastUpdateTime = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            PlayerSaves.Set(player, saveData);
            RPC_OnPlayerSaveUpdated(player, saveData);
        }

        public void StartSaveSync(PlayerRef player)
        {
            if (!Runner.IsServer) return;
            if (!SyncStates.TryGet(player, out var syncState)) return;

            syncState.IsSyncing = true;
            syncState.SyncStartTime = Runner.SimulationTime;
            syncState.SyncProgress = 0;
            syncState.HasError = false;
            syncState.ErrorMessage = "";

            SyncStates.Set(player, syncState);
            RPC_OnSaveSyncStarted(player);
        }

        public void UpdateSyncProgress(PlayerRef player, byte progress)
        {
            if (!Runner.IsServer) return;
            if (!SyncStates.TryGet(player, out var syncState)) return;

            syncState.SyncProgress = progress;
            syncState.LastSyncTime = Runner.SimulationTime;
            SyncStates.Set(player, syncState);
            RPC_OnSyncProgressUpdated(player, progress);
        }

        public void CompleteSaveSync(PlayerRef player, bool success, string errorMessage = "")
        {
            if (!Runner.IsServer) return;
            if (!SyncStates.TryGet(player, out var syncState)) return;

            syncState.IsSyncing = false;
            syncState.SyncProgress = success ? (byte)100 : syncState.SyncProgress;
            syncState.HasError = !success;
            syncState.ErrorMessage = errorMessage;
            syncState.LastSyncTime = Runner.SimulationTime;

            SyncStates.Set(player, syncState);
            RPC_OnSaveSyncCompleted(player, success, errorMessage);
        }

        private bool ValidateSaveData(PlayerSaveData saveData)
        {
            // 基本数据验证
            if (saveData.Level < 1) return false;
            if (saveData.Experience < 0) return false;
            if (saveData.Currency < 0) return false;
            if (saveData.SaveVersion < 1) return false;

            // 装备验证
            for (int i = 0; i < 8; i++)
            {
                int equippedItem = saveData.GetEquippedItem(i);
                if (equippedItem != 0 && !IsItemUnlocked(saveData, equippedItem))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsItemUnlocked(PlayerSaveData saveData, int itemId)
        {
            for (int i = 0; i < 32; i++)
            {
                if (saveData.GetUnlockedItem(i) == itemId) return true;
            }
            return false;
        }

        // 数据访问方法
        public PlayerSaveData? GetPlayerSaveData(PlayerRef player)
        {
            return PlayerSaves.TryGet(player, out var data) ? data : null;
        }

        public SaveSyncState? GetSyncState(PlayerRef player)
        {
            return SyncStates.TryGet(player, out var state) ? state : null;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerSaveInitialized(PlayerRef player, PlayerSaveData saveData, SaveSyncState syncState)
        {
            PlayerSaves.Set(player, saveData);
            SyncStates.Set(player, syncState);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerSaveUpdated(PlayerRef player, PlayerSaveData saveData)
        {
            PlayerSaves.Set(player, saveData);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnSaveSyncStarted(PlayerRef player)
        {
            if (SyncStates.TryGet(player, out var syncState))
            {
                syncState.IsSyncing = true;
                syncState.SyncStartTime = Runner.SimulationTime;
                syncState.SyncProgress = 0;
                syncState.HasError = false;
                syncState.ErrorMessage = "";
                SyncStates.Set(player, syncState);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnSyncProgressUpdated(PlayerRef player, byte progress)
        {
            if (SyncStates.TryGet(player, out var syncState))
            {
                syncState.SyncProgress = progress;
                syncState.LastSyncTime = Runner.SimulationTime;
                SyncStates.Set(player, syncState);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnSaveSyncCompleted(PlayerRef player, NetworkBool success, NetworkString<_32> errorMessage)
        {
            if (SyncStates.TryGet(player, out var syncState))
            {
                syncState.IsSyncing = false;
                syncState.SyncProgress = success ? (byte)100 : syncState.SyncProgress;
                syncState.HasError = !success;
                syncState.ErrorMessage = errorMessage;
                syncState.LastSyncTime = Runner.SimulationTime;
                SyncStates.Set(player, syncState);
            }
        }
    }
}