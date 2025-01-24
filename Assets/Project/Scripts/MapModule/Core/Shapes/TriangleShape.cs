using MapModule.Core.Data;
using MapModule.Core.Utils;

namespace MapModule.Core.Shapes
{
    public class TriangleShape : BaseShape
    {
        private float currentRotation;
        private float rotationDirection;

        public TriangleShape(IMapManager manager) : base(manager)
        {
        }

        public override void Initialize(ShapeConfig config)
        {
            base.Initialize(config);
            currentRotation = 0;
            rotationDirection = 0;
        }

        protected override void OnShapeHit(int skillId, Vector3D hitPoint)
        {
            // 计算本地坐标
            float localX = hitPoint.X - position.X;
            
            // 根据击中位置决定旋转方向
            rotationDirection = localX < 0 ? 1 : -1;
            
            manager.PublishEvent(MapEvents.ShapeChanged, new ShapeChangedEvent
            {
                Type = ShapeType.Triangle,
                Position = position,
                Rotation = currentRotation,
                RotationDirection = rotationDirection
            });
        }

        public void Update(float deltaTime)
        {
            if (!isActive || !config.EnableRotation) return;
            
            if (rotationDirection != 0)
            {
                currentRotation += rotationDirection * config.RotationSpeed * deltaTime;
                // 标准化角度到 0-360
                currentRotation = currentRotation % 360;
                if (currentRotation < 0) currentRotation += 360;
            }
        }

        protected override void ValidateConfig(ShapeConfig config)
        {
            if (config.RotationSpeed <= 0)
            {
                throw new System.ArgumentException("Rotation speed must be positive");
            }
        }

        public override ShapeState GetState()
        {
            return new ShapeState
            {
                Type = ShapeType.Triangle,
                Position = position,
                IsActive = isActive,
                CurrentRotation = currentRotation,
                IsRotating = rotationDirection != 0
            };
        }
    }
} 