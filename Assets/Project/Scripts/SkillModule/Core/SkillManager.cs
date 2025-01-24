using System.Collections.Generic;
using SkillModule.Types;
using SkillModule.Events;

namespace SkillModule.Core
{
    public class SkillManager
    {
        private static SkillManager instance;
        public static SkillManager Instance => instance ??= new SkillManager();

        private Dictionary<int, BaseSkill> skills = new Dictionary<int, BaseSkill>();
        private Dictionary<int, SkillConfig> skillConfigs = new Dictionary<int, SkillConfig>();
        private SkillEventSystem eventSystem => SkillEventSystem.Instance;

        private SkillManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            eventSystem.Subscribe(SkillEventSystem.EventNames.SkillStart, OnSkillStart);
            eventSystem.Subscribe(SkillEventSystem.EventNames.SkillEnd, OnSkillEnd);
            eventSystem.Subscribe(SkillEventSystem.EventNames.SkillCooldown, OnSkillCooldown);
        }

        public void RegisterSkill(SkillConfig config)
        {
            if (config != null && config.IsValid())
            {
                skillConfigs[config.SkillId] = config;
            }
        }

        public void CreateSkill(int skillId, object owner)
        {
            if (skillConfigs.TryGetValue(skillId, out var config))
            {
                var skill = CreateSkillInstance(config);
                if (skill != null)
                {
                    skills[skillId] = skill;
                }
            }
        }

        private BaseSkill CreateSkillInstance(SkillConfig config)
        {
            // 根据配置创建对应类型的技能实例
            // 这里需要实现具体的技能创建逻辑
            return null;
        }

        public bool UseSkill(int skillId, SkillContext context)
        {
            if (skills.TryGetValue(skillId, out var skill))
            {
                return skill.Execute(context.Position, context.Direction);
            }
            return false;
        }

        public SkillConfig GetSkillConfig(int skillId)
        {
            skillConfigs.TryGetValue(skillId, out var config);
            return config;
        }

        private void OnSkillStart(object eventData)
        {
            if (eventData is SkillEventData data)
            {
                // 处理技能开始事件
            }
        }

        private void OnSkillEnd(object eventData)
        {
            if (eventData is SkillEventData data)
            {
                // 处理技能结束事件
            }
        }

        private void OnSkillCooldown(object eventData)
        {
            if (eventData is SkillEventData data)
            {
                // 处理技能冷却事件
            }
        }
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 zero => new Vector3(0, 0, 0);
    }
} 