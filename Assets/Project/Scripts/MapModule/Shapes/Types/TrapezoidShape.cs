using UnityEngine;
using Core.EventSystem;
using MapModule.Utils;
namespace MapModule.Shapes
{
    /// <summary>
    /// 等腰梯形
    /// 功能：
    /// 1. 上底进入->下底射出，下底进入->上底射出（加速）
    /// 2. 每0.35秒射出一次
    /// 3. 被击中时旋转
    /// 4. 30秒后消失
    /// </summary>
    public class TrapezoidShape : BaseShape
    {
        private float lastSkillTime;
        private float currentRotation;
        private float rotationDirection;
        private float remainingTime;
        private const float SKILL_INTERVAL = 0.35f;
        private const float SPEED_MULTIPLIER = 1.5f;

        public override void Initialize(ShapeConfig config)
        {
            base.Initialize(config);
            lastSkillTime = 0;
            currentRotation = 0;
            rotationDirection = 0;
            remainingTime = 30f; // 30秒持续时间
        }

        public override void HandleSkillHit(int skillId, Vector3 hitPoint)
        {
            if (!ValidateHitPoint(hitPoint)) return;

            Vector2 localHitPoint = transform.InverseTransformPoint(hitPoint);
            if (ShapeUtils.CheckCollision(localHitPoint, ShapeType.Trapezoid, Vector2.zero, config.Size))
            {
                bool isTopHit = localHitPoint.y > 0;
                bool isBottomHit = localHitPoint.y < 0;

                // 处理射击逻辑
                if ((isTopHit || isBottomHit) && Time.time - lastSkillTime >= SKILL_INTERVAL)
                {
                    lastSkillTime = Time.time;
                    
                    Vector3 shootPosition = transform.TransformPoint(new Vector3(
                        0,
                        isTopHit ? -config.Size.y/2 : config.Size.y/2,
                        0
                    ));

                    // 触发技能事件
                    EventManager.Instance.TriggerEvent(new ShapeActionEvent
                    {
                        Type = ShapeType.Trapezoid,
                        ActionType = ShapeActionType.Shoot,
                        Position = shootPosition,
                        ActionData = new ShootData
                        {
                            SkillId = skillId,
                            IsAccelerated = isBottomHit
                        }
                    });
                }
                
                // 处理旋转
                rotationDirection = localHitPoint.x < 0 ? 1 : -1;
                
                EventManager.Instance.TriggerEvent(new ShapeActionEvent
                {
                    Type = ShapeType.Trapezoid,
                    ActionType = ShapeActionType.Rotate,
                    Position = transform.position,
                    ActionData = rotationDirection
                });
            }
        }

        protected override void OnShapeUpdate()
        {
            if (!isActive) return;

            // 更新旋转
            if (rotationDirection != 0)
            {
                currentRotation += rotationDirection * config.RotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
            }

            // 更新剩余时间
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                OnDisappear();
            }
        }

        protected override void ValidateConfig(ShapeConfig config)
        {
            if (config.BulletDelay <= 0)
            {
                Debug.LogError("Trapezoid shape bullet delay must be greater than 0");
            }
            if (config.RotationSpeed <= 0)
            {
                Debug.LogError("Trapezoid shape rotation speed must be greater than 0");
            }
        }

        public override ShapeState GetState()
        {
            return new ShapeState
            {
                Type = ShapeType.Trapezoid,
                Position = transform.position,
                IsActive = isActive,
                CurrentRotation = currentRotation,
                IsRotating = rotationDirection != 0,
                LastBulletTime = lastSkillTime,
                RemainingTime = remainingTime
            };
        }
    }

    // 射击数据结构
    public struct ShootData
    {
        public int SkillId;
        public bool IsAccelerated;
    }
} 