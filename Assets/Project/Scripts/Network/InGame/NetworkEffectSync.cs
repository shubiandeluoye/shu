using UnityEngine;
using Fusion;

namespace Core.Network
{
    /// <summary>
    /// 特效同步组件
    /// 负责：
    /// 1. 特效的网络同步
    /// 2. 大型子弹特效处理
    /// 3. 碎屏效果同步
    /// 4. 摄像机晃动同步
    /// </summary>
    public class NetworkEffectSync : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<int, EffectData> ActiveEffects { get; }
        [Networked] private NetworkDictionary<PlayerRef, PlayerEffectState> PlayerEffects { get; }
        [Networked] private int NextEffectId { get; set; }

        public struct EffectData : INetworkStruct
        {
            public int EffectId;
            public byte EffectType;
            public Vector3 Position;
            public Vector3 Direction;
            public float StartTime;
            public float Duration;
            public float Scale;
            public NetworkBool IsLooping;
            public NetworkBool IsActive;
        }

        public struct PlayerEffectState : INetworkStruct
        {
            [Networked, Capacity(8)]  // 设置适当的容量
            public NetworkArray<NetworkBool> ActiveBuffs => default;
            
            [Networked, Capacity(8)]
            public NetworkArray<float> BuffDurations => default;
            
            [Networked, Capacity(8)]
            public NetworkArray<float> BuffStrengths => default;
            
            public float LastEffectTime;
        }

        public int SpawnEffect(byte effectType, Vector3 position, Vector3 direction, float duration, float scale, bool isLooping)
        {
            if (!Runner.IsServer) return -1;

            int effectId = NextEffectId++;
            var effectData = new EffectData
            {
                EffectId = effectId,
                EffectType = effectType,
                Position = position,
                Direction = direction,
                StartTime = Runner.SimulationTime,
                Duration = duration,
                Scale = scale,
                IsLooping = isLooping,
                IsActive = true
            };

            ActiveEffects.Set(effectId, effectData);
            RPC_OnEffectSpawned(effectId, effectData);

            return effectId;
        }

        public void UpdateEffectPosition(int effectId, Vector3 position, Vector3 direction)
        {
            if (!Runner.IsServer) return;
            if (!ActiveEffects.TryGet(effectId, out var effectData)) return;

            effectData.Position = position;
            effectData.Direction = direction;
            ActiveEffects.Set(effectId, effectData);
            RPC_OnEffectPositionUpdated(effectId, position, direction);
        }

        public void StopEffect(int effectId)
        {
            if (!Runner.IsServer) return;
            if (!ActiveEffects.TryGet(effectId, out var effectData)) return;

            effectData.IsActive = false;
            ActiveEffects.Set(effectId, effectData);
            RPC_OnEffectStopped(effectId);
        }

        public void ApplyPlayerEffect(PlayerRef player, byte buffType, float duration, float strength)
        {
            if (!Runner.IsServer) return;

            var state = PlayerEffects.TryGet(player, out var existingState) ? existingState : new PlayerEffectState();
            
            // 创建临时变量
            var activeBuffs = state.ActiveBuffs;
            var buffDurations = state.BuffDurations;
            var buffStrengths = state.BuffStrengths;

            // 修改临时变量
            activeBuffs[buffType] = true;
            buffDurations[buffType] = duration;
            buffStrengths[buffType] = strength;
            state.LastEffectTime = Runner.SimulationTime;

            PlayerEffects.Set(player, state);
            RPC_OnPlayerEffectApplied(player, buffType, duration, strength);
        }

        public void RemovePlayerEffect(PlayerRef player, byte buffType)
        {
            if (!Runner.IsServer) return;
            if (!PlayerEffects.TryGet(player, out var state)) return;

            // 创建临时变量
            var activeBuffs = state.ActiveBuffs;
            var buffDurations = state.BuffDurations;
            var buffStrengths = state.BuffStrengths;

            // 修改临时变量
            activeBuffs[buffType] = false;
            buffDurations[buffType] = 0;
            buffStrengths[buffType] = 0;

            PlayerEffects.Set(player, state);
            RPC_OnPlayerEffectRemoved(player, buffType);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer) return;

            float currentTime = Runner.SimulationTime;

            // 更新玩家效果持续时间
            foreach (var kvp in PlayerEffects)
            {
                var player = kvp.Key;
                var state = kvp.Value;
                bool updated = false;

                // 创建临时变量
                var activeBuffs = state.ActiveBuffs;
                var buffDurations = state.BuffDurations;
                var buffStrengths = state.BuffStrengths;

                for (byte i = 0; i < activeBuffs.Length; i++)
                {
                    if (activeBuffs[i])
                    {
                        buffDurations[i] -= Runner.DeltaTime;
                        if (buffDurations[i] <= 0)
                        {
                            activeBuffs[i] = false;
                            buffStrengths[i] = 0;
                            updated = true;
                        }
                    }
                }

                if (updated)
                {
                    PlayerEffects.Set(player, state);
                }
            }

            // 清理过期的特效
            foreach (var kvp in ActiveEffects)
            {
                var effectId = kvp.Key;
                var effect = kvp.Value;

                if (effect.IsActive && !effect.IsLooping && 
                    currentTime - effect.StartTime >= effect.Duration)
                {
                    StopEffect(effectId);
                }
            }
        }

        // 数据访问方法
        public EffectData? GetEffectData(int effectId)
        {
            return ActiveEffects.TryGet(effectId, out var data) ? data : null;
        }

        public PlayerEffectState? GetPlayerEffectState(PlayerRef player)
        {
            return PlayerEffects.TryGet(player, out var state) ? state : null;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnEffectSpawned(int effectId, EffectData data)
        {
            ActiveEffects.Set(effectId, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnEffectPositionUpdated(int effectId, Vector3 position, Vector3 direction)
        {
            if (ActiveEffects.TryGet(effectId, out var data))
            {
                data.Position = position;
                data.Direction = direction;
                ActiveEffects.Set(effectId, data);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnEffectStopped(int effectId)
        {
            if (ActiveEffects.TryGet(effectId, out var data))
            {
                data.IsActive = false;
                ActiveEffects.Set(effectId, data);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerEffectApplied(PlayerRef player, byte buffType, float duration, float strength)
        {
            var state = PlayerEffects.TryGet(player, out var existingState) ? existingState : new PlayerEffectState();
            
            // 创建临时变量
            var activeBuffs = state.ActiveBuffs;
            var buffDurations = state.BuffDurations;
            var buffStrengths = state.BuffStrengths;

            // 修改临时变量
            activeBuffs[buffType] = true;
            buffDurations[buffType] = duration;
            buffStrengths[buffType] = strength;
            state.LastEffectTime = Runner.SimulationTime;

            PlayerEffects.Set(player, state);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerEffectRemoved(PlayerRef player, byte buffType)
        {
            if (PlayerEffects.TryGet(player, out var state))
            {
                // 创建临时变量
                var activeBuffs = state.ActiveBuffs;
                var buffDurations = state.BuffDurations;
                var buffStrengths = state.BuffStrengths;

                // 修改临时变量
                activeBuffs[buffType] = false;
                buffDurations[buffType] = 0;
                buffStrengths[buffType] = 0;

                PlayerEffects.Set(player, state);
            }
        }
    }
} 
