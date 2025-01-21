using UnityEngine;
using System.Collections.Generic;
using SkillModule.Types;
using Core.EventSystem;
using SkillModule.Events;

namespace SkillModule.Core
{
    public class SkillManager : MonoBehaviour
    {
        private Dictionary<int, SkillConfig> skillConfigs = new Dictionary<int, SkillConfig>();
        private Dictionary<int, float> skillCooldowns = new Dictionary<int, float>();

        public bool UseSkill(int skillId, Vector3 position, Vector3 direction)
        {
            if (skillConfigs.TryGetValue(skillId, out var config))
            {
                // 检查冷却
                if (IsSkillOnCooldown(skillId))
                {
                    return false;
                }

                // 开始冷却
                StartCooldown(skillId, config.Cooldown);

                // 触发技能事件
                EventManager.Instance.TriggerEvent(new SkillStartEvent
                {
                    SkillId = skillId,
                    Position = position,
                    Direction = direction
                });

                return true;
            }
            return false;
        }

        public void RegisterSkill(SkillConfig config)
        {
            if (config != null && config.IsValid())
            {
                skillConfigs[config.SkillId] = config;
            }
        }

        public SkillConfig GetSkillConfig(int skillId)
        {
            skillConfigs.TryGetValue(skillId, out var config);
            return config;
        }

        private bool IsSkillOnCooldown(int skillId)
        {
            if (skillCooldowns.TryGetValue(skillId, out float cooldownEndTime))
            {
                return Time.time < cooldownEndTime;
            }
            return false;
        }

        private void StartCooldown(int skillId, float duration)
        {
            skillCooldowns[skillId] = Time.time + duration;
        }

        private void Update()
        {
            // 更新冷却时间
            foreach (var skillId in new List<int>(skillCooldowns.Keys))
            {
                if (Time.time >= skillCooldowns[skillId])
                {
                    skillCooldowns.Remove(skillId);
                    // 触发冷却结束事件
                    EventManager.Instance.TriggerEvent(new SkillCooldownEvent
                    {
                        SkillId = skillId,
                        CooldownTime = 0
                    });
                }
            }
        }
    }
} 