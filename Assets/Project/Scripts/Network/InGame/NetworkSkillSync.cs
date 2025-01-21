using UnityEngine;
using Fusion;

namespace Core.Network
{
    public class NetworkSkillSync : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<PlayerRef, PlayerSkillState> PlayerSkills { get; }
        [Networked] private NetworkDictionary<int, SkillData> ActiveSkills { get; }
        [Networked] private int NextSkillId { get; set; }

        public struct PlayerSkillState : INetworkStruct
        {
            // 冷却时间，最多8个技能
            public float CooldownTimer0;
            public float CooldownTimer1;
            public float CooldownTimer2;
            public float CooldownTimer3;
            public float CooldownTimer4;
            public float CooldownTimer5;
            public float CooldownTimer6;
            public float CooldownTimer7;
            
            // 技能是否启用
            public NetworkBool IsSkillEnabled0;
            public NetworkBool IsSkillEnabled1;
            public NetworkBool IsSkillEnabled2;
            public NetworkBool IsSkillEnabled3;
            public NetworkBool IsSkillEnabled4;
            public NetworkBool IsSkillEnabled5;
            public NetworkBool IsSkillEnabled6;
            public NetworkBool IsSkillEnabled7;
            
            // 技能等级
            public byte SkillLevel0;
            public byte SkillLevel1;
            public byte SkillLevel2;
            public byte SkillLevel3;
            public byte SkillLevel4;
            public byte SkillLevel5;
            public byte SkillLevel6;
            public byte SkillLevel7;
            
            public float Energy;
            public float LastCastTime;

            // 辅助方法
            public float GetCooldownTimer(int index)
            {
                if (index < 0 || index >= 8) return 0;
                switch (index)
                {
                    case 0: return CooldownTimer0;
                    case 1: return CooldownTimer1;
                    case 2: return CooldownTimer2;
                    case 3: return CooldownTimer3;
                    case 4: return CooldownTimer4;
                    case 5: return CooldownTimer5;
                    case 6: return CooldownTimer6;
                    case 7: return CooldownTimer7;
                    default: return 0;
                }
            }

            public void SetCooldownTimer(int index, float value)
            {
                if (index < 0 || index >= 8) return;
                switch (index)
                {
                    case 0: CooldownTimer0 = value; break;
                    case 1: CooldownTimer1 = value; break;
                    case 2: CooldownTimer2 = value; break;
                    case 3: CooldownTimer3 = value; break;
                    case 4: CooldownTimer4 = value; break;
                    case 5: CooldownTimer5 = value; break;
                    case 6: CooldownTimer6 = value; break;
                    case 7: CooldownTimer7 = value; break;
                }
            }

            public NetworkBool GetIsSkillEnabled(int index)
            {
                if (index < 0 || index >= 8) return false;
                switch (index)
                {
                    case 0: return IsSkillEnabled0;
                    case 1: return IsSkillEnabled1;
                    case 2: return IsSkillEnabled2;
                    case 3: return IsSkillEnabled3;
                    case 4: return IsSkillEnabled4;
                    case 5: return IsSkillEnabled5;
                    case 6: return IsSkillEnabled6;
                    case 7: return IsSkillEnabled7;
                    default: return false;
                }
            }

            public void SetIsSkillEnabled(int index, NetworkBool value)
            {
                if (index < 0 || index >= 8) return;
                switch (index)
                {
                    case 0: IsSkillEnabled0 = value; break;
                    case 1: IsSkillEnabled1 = value; break;
                    case 2: IsSkillEnabled2 = value; break;
                    case 3: IsSkillEnabled3 = value; break;
                    case 4: IsSkillEnabled4 = value; break;
                    case 5: IsSkillEnabled5 = value; break;
                    case 6: IsSkillEnabled6 = value; break;
                    case 7: IsSkillEnabled7 = value; break;
                }
            }

            public byte GetSkillLevel(int index)
            {
                if (index < 0 || index >= 8) return 0;
                switch (index)
                {
                    case 0: return SkillLevel0;
                    case 1: return SkillLevel1;
                    case 2: return SkillLevel2;
                    case 3: return SkillLevel3;
                    case 4: return SkillLevel4;
                    case 5: return SkillLevel5;
                    case 6: return SkillLevel6;
                    case 7: return SkillLevel7;
                    default: return 0;
                }
            }

            public void SetSkillLevel(int index, byte value)
            {
                if (index < 0 || index >= 8) return;
                switch (index)
                {
                    case 0: SkillLevel0 = value; break;
                    case 1: SkillLevel1 = value; break;
                    case 2: SkillLevel2 = value; break;
                    case 3: SkillLevel3 = value; break;
                    case 4: SkillLevel4 = value; break;
                    case 5: SkillLevel5 = value; break;
                    case 6: SkillLevel6 = value; break;
                    case 7: SkillLevel7 = value; break;
                }
            }
        }

        public struct SkillData : INetworkStruct
        {
            public int SkillId;
            public PlayerRef CasterId;
            public Vector3 Position;
            public Vector3 Direction;
            public float StartTime;
            public float Duration;
            public NetworkBool IsActive;
        }

        public void InitializePlayerSkills(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            var skillState = new PlayerSkillState
            {
                Energy = 100f,
                LastCastTime = 0f
            };

            PlayerSkills.Set(player, skillState);
            RPC_OnPlayerSkillsInitialized(player, skillState);
        }

        public bool CastSkill(PlayerRef player, int skillIndex, Vector3 position, Vector3 direction, float duration)
        {
            if (!Runner.IsServer) return false;
            if (!PlayerSkills.TryGet(player, out var state)) return false;

            // 检查技能是否可用
            if (!state.GetIsSkillEnabled(skillIndex)) return false;
            if (state.GetCooldownTimer(skillIndex) > 0) return false;
            if (state.Energy < GetSkillEnergyCost(skillIndex)) return false;

            // 创建新的技能数据
            var skillId = NextSkillId++;
            var skillData = new SkillData
            {
                SkillId = skillId,
                CasterId = player,
                Position = position,
                Direction = direction,
                StartTime = Runner.SimulationTime,
                Duration = duration,
                IsActive = true
            };

            // 更新状态
            state.SetCooldownTimer(skillIndex, GetSkillCooldown(skillIndex));
            state.Energy -= GetSkillEnergyCost(skillIndex);
            state.LastCastTime = Runner.SimulationTime;
            
            PlayerSkills.Set(player, state);
            ActiveSkills.Set(skillId, skillData);

            RPC_OnSkillCast(skillId, player, skillIndex, skillData);
            return true;
        }

        public void UpdateSkillState(int skillId, Vector3 position)
        {
            if (!Runner.IsServer) return;
            if (!ActiveSkills.TryGet(skillId, out var skillData)) return;

            skillData.Position = position;
            ActiveSkills.Set(skillId, skillData);
            RPC_OnSkillStateUpdated(skillId, position);
        }

        public void EndSkill(int skillId)
        {
            if (!Runner.IsServer) return;
            if (!ActiveSkills.TryGet(skillId, out var skillData)) return;

            skillData.IsActive = false;
            ActiveSkills.Set(skillId, skillData);
            RPC_OnSkillEnded(skillId);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer) return;

            // 更新冷却时间
            foreach (var kvp in PlayerSkills)
            {
                var player = kvp.Key;
                var skillState = kvp.Value;
                bool updated = false;

                for (int i = 0; i < 8; i++)
                {
                    float cooldown = skillState.GetCooldownTimer(i);
                    if (cooldown > 0)
                    {
                        skillState.SetCooldownTimer(i, Mathf.Max(0, cooldown - Runner.DeltaTime));
                        updated = true;
                    }
                }

                if (updated)
                {
                    PlayerSkills.Set(player, skillState);
                }
            }
        }

        // 数据访问方法
        public PlayerSkillState? GetPlayerSkillState(PlayerRef player)
        {
            return PlayerSkills.TryGet(player, out var state) ? state : null;
        }

        public SkillData? GetActiveSkillData(int skillId)
        {
            return ActiveSkills.TryGet(skillId, out var data) ? data : null;
        }

        private float GetSkillCooldown(int skillIndex)
        {
            // 这里可以根据技能等级或其他因素计算冷却时间
            return 5f;
        }

        private float GetSkillEnergyCost(int skillIndex)
        {
            // 这里可以根据技能等级或其他因素计算能量消耗
            return 20f;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnPlayerSkillsInitialized(PlayerRef player, PlayerSkillState state)
        {
            PlayerSkills.Set(player, state);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnSkillCast(int skillId, PlayerRef player, int skillIndex, SkillData data)
        {
            if (PlayerSkills.TryGet(player, out var state))
            {
                state.SetCooldownTimer(skillIndex, GetSkillCooldown(skillIndex));
                state.Energy -= GetSkillEnergyCost(skillIndex);
                state.LastCastTime = Runner.SimulationTime;
                PlayerSkills.Set(player, state);
            }

            ActiveSkills.Set(skillId, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnSkillStateUpdated(int skillId, Vector3 position)
        {
            if (ActiveSkills.TryGet(skillId, out var data))
            {
                data.Position = position;
                ActiveSkills.Set(skillId, data);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnSkillEnded(int skillId)
        {
            if (ActiveSkills.TryGet(skillId, out var data))
            {
                data.IsActive = false;
                ActiveSkills.Set(skillId, data);
            }
        }
    }
}