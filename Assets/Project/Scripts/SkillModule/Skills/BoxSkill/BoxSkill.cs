using SkillModule.Core;
using SkillModule.Events;

namespace SkillModule.Skills
{
    public class BoxSkill : BaseSkill
    {
        private BoxConfig boxConfig;

        public BoxSkill(BoxConfig config) : base(config)
        {
            boxConfig = config;
        }

        protected override void OnSkillStart()
        {
            // 发布创建盒子事件
            eventSystem.Publish("BoxCreated", new BoxSkillData
            {
                SkillId = SkillId,
                Position = skillContext.Position,
                Direction = skillContext.Direction,
                BoxSize = boxConfig.BoxSize,
                Durability = boxConfig.BoxDurability,
                CanBePushed = boxConfig.CanBePushed,
                PushForce = boxConfig.PushForce
            });
        }
    }

    public class BoxConfig : SkillConfig
    {
        public float BoxSize { get; set; }
        public float BoxDurability { get; set; }
        public bool CanBePushed { get; set; }
        public float PushForce { get; set; }
        public float PlaceDistance { get; set; }
    }

    public struct BoxSkillData
    {
        public int SkillId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float BoxSize { get; set; }
        public float Durability { get; set; }
        public bool CanBePushed { get; set; }
        public float PushForce { get; set; }
    }
} 