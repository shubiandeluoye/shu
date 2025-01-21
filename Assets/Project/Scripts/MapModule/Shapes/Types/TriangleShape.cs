using UnityEngine;
using Core.EventSystem;
using MapModule.Utils;

namespace MapModule.Shapes
{
    /// <summary>
    /// 三角形
    /// 功能：
    /// 1. 左侧被击中顺时针旋转
    /// 2. 右侧被击中逆时针旋转
    /// </summary>
    public class TriangleShape : BaseShape
    {
        private float currentRotation;
        private float rotationDirection;

        public override void Initialize(ShapeConfig config)
        {
            base.Initialize(config);
            currentRotation = 0;
            rotationDirection = 0;
        }

        public override void HandleSkillHit(int skillId, Vector3 hitPoint)
        {
            if (!ValidateHitPoint(hitPoint)) return;

            Vector2 localHitPoint = transform.InverseTransformPoint(hitPoint);
            if (ShapeUtils.CheckCollision(localHitPoint, ShapeType.Triangle, Vector2.zero, config.Size))
            {
                // 根据击中位置决定旋转方向
                rotationDirection = localHitPoint.x < 0 ? 1 : -1;
                
                // 触发旋转事件
                EventManager.Instance.TriggerEvent(new ShapeActionEvent
                {
                    Type = ShapeType.Triangle,
                    ActionType = ShapeActionType.Rotate,
                    Position = transform.position,
                    ActionData = rotationDirection
                });
            }
        }

        protected override void OnShapeUpdate()
        {
            if (!isActive) return;
            
            if (rotationDirection != 0)
            {
                currentRotation += rotationDirection * config.RotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
            }
        }

        protected override void ValidateConfig(ShapeConfig config)
        {
            if (config.RotationSpeed <= 0)
            {
                Debug.LogError("Triangle shape rotation speed must be greater than 0");
            }
        }

        public override ShapeState GetState()
        {
            return new ShapeState
            {
                Type = ShapeType.Triangle,
                Position = transform.position,
                IsActive = isActive,
                CurrentRotation = currentRotation,
                IsRotating = rotationDirection != 0
            };
        }
    }
} 