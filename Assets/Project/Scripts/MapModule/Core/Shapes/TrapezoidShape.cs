using MapModule.Core.Data;
using MapModule.Core.Utils;

namespace MapModule.Core.Shapes
{
    public class TrapezoidShape : BaseShape
    {
        private float lastSkillTime;
        private float currentRotation;
        private float rotationDirection;
        private float remainingTime;

        public TrapezoidShape(IMapManager manager) : base(manager)
        {
        }

        public override void Initialize(ShapeConfig config)
        {
            base.Initialize(config);
            lastSkillTime = 0;
            currentRotation = 0;
            rotationDirection = 0;
            remainingTime = config.Duration;
        }

        protected override void OnShapeHit(int skillId, Vector3D hitPoint)
        {
            // 计算本地坐标
            float localX = hitPoint.X - position.X;
            float localY = hitPoint.Y - position.Y;
            bool isTopHit = localY > 0;
            bool isBottomHit = localY < 0;

            // 处理射击逻辑
            if ((isTopHit || isBottomHit) && TimeUtils.GetCurrentTime() - lastSkillTime >= config.BulletDelay)
            {
                lastSkillTime = TimeUtils.GetCurrentTime();
                
                Vector3D shootPosition = new Vector3D(
                    position.X,
                    position.Y + (isTopHit ? -config.Size.Y/2 : config.Size.Y/2),
                    0
                );

                manager.PublishEvent(MapEvents.ShapeChanged, new ShapeChangedEvent
                {
                    Type = ShapeType.Trapezoid,
                    Position = shootPosition,
                    ActionType = ShapeActionType.Shoot,
                    ActionData = new ShootData
                    {
                        SkillId = skillId,
                        IsAccelerated = isBottomHit
                    }
                });
            }
            
            // 处理旋转
            rotationDirection = localX < 0 ? 1 : -1;
            
            manager.PublishEvent(MapEvents.ShapeChanged, new ShapeChangedEvent
            {
                Type = ShapeType.Trapezoid,
                Position = position,
                Rotation = currentRotation,
                RotationDirection = rotationDirection
            });
        }

        public void Update(float deltaTime)
        {
            if (!isActive) return;

            // 更新旋转
            if (rotationDirection != 0 && config.EnableRotation)
            {
                currentRotation += rotationDirection * config.RotationSpeed * deltaTime;
                currentRotation = currentRotation % 360;
                if (currentRotation < 0) currentRotation += 360;
            }

            // 更新剩余时间
            remainingTime -= deltaTime;
            if (remainingTime <= 0)
            {
                OnDisappear();
            }
        }

        protected override void ValidateConfig(ShapeConfig config)
        {
            if (config.BulletDelay <= 0)
            {
                throw new System.ArgumentException("Bullet delay must be positive");
            }
            if (config.TopWidth <= 0 || config.BottomWidth <= 0)
            {
                throw new System.ArgumentException("Width must be positive");
            }
            if (config.Duration <= 0)
            {
                throw new System.ArgumentException("Duration must be positive");
            }
        }

        public override ShapeState GetState()
        {
            return new ShapeState
            {
                Type = ShapeType.Trapezoid,
                Position = position,
                IsActive = isActive,
                CurrentRotation = currentRotation,
                IsRotating = rotationDirection != 0,
                LastBulletTime = lastSkillTime,
                RemainingTime = remainingTime
            };
        }
    }

    public struct ShootData
    {
        public int SkillId { get; set; }
        public bool IsAccelerated { get; set; }
    }
} 