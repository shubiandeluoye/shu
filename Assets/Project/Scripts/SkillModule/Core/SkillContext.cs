using UnityEngine;

namespace SkillModule.Core
{
    /// <summary>
    /// 技能执行上下文
    /// 包含技能执行时需要的所有信息
    /// </summary>
    public class SkillContext
    {
        public object Source { get; set; }
        public object Target { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float[] Parameters { get; set; }

        public SkillContext() { }

        public SkillContext(object source)
        {
            Source = source;
        }

        public SkillContext WithTarget(object target)
        {
            Target = target;
            return this;
        }

        public SkillContext WithPosition(Vector3 position)
        {
            Position = position;
            return this;
        }

        public SkillContext WithDirection(Vector3 direction)
        {
            Direction = direction;
            return this;
        }

        public SkillContext WithParameters(params float[] parameters)
        {
            Parameters = parameters;
            return this;
        }
    }
} 