using MapModule.Core.Data;
using MapModule.Core.Utils;

namespace MapModule.Core.Shapes
{
    public class CircleShape : BaseShape
    {
        private int collectedCount;

        public CircleShape(IMapManager manager) : base(manager)
        {
        }

        public override void Initialize(ShapeConfig config)
        {
            base.Initialize(config);
            collectedCount = 0;
        }

        protected override void OnShapeHit(int skillId, Vector3D hitPoint)
        {
            collectedCount++;
            
            manager.PublishEvent(MapEvents.BulletCollected, new BulletCollectedEvent
            {
                Position = hitPoint,
                CurrentCount = collectedCount,
                MaxCount = config.BulletCapacity
            });

            if (collectedCount >= config.BulletCapacity)
            {
                OnDisappear();
            }
        }

        protected override void ValidateConfig(ShapeConfig config)
        {
            if (config.BulletCapacity <= 0)
            {
                throw new System.ArgumentException("Circle shape bullet capacity must be greater than 0");
            }
        }

        public override ShapeState GetState()
        {
            return new ShapeState
            {
                Type = ShapeType.Circle,
                Position = position,
                IsActive = isActive,
                CurrentBulletCount = collectedCount
            };
        }
    }
} 