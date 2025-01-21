using UnityEngine;
using Core.EventSystem;
using MapModule.Utils;
namespace MapModule.Shapes
{
    /// <summary>
    /// 圆形
    /// 功能：
    /// 1. 收集子弹（21个）
    /// 2. 收集完成后消失
    /// </summary>
    public class CircleShape : BaseShape
    {
        private int collectedCount;

        public override void Initialize(ShapeConfig config)
        {
            base.Initialize(config);
            collectedCount = 0;
        }

        public override void HandleSkillHit(int skillId, Vector3 hitPoint)
        {
            if (!ValidateHitPoint(hitPoint)) return;

            Vector2 localHitPoint = transform.InverseTransformPoint(hitPoint);
            if (ShapeUtils.CheckCollision(localHitPoint, ShapeType.Circle, Vector2.zero, config.Size))
            {
                collectedCount++;
                
                // 触发收集事件
                EventManager.Instance.TriggerEvent(new BulletCollectedEvent
                {
                    Position = hitPoint,
                    CurrentCount = collectedCount,
                    MaxCount = config.BulletCapacity
                });

                // 检查是否收集完成
                if (collectedCount >= config.BulletCapacity)
                {
                    OnDisappear();
                }
            }
        }

        protected override void OnShapeUpdate()
        {
            // 圆形不需要每帧更新
        }

        protected override void ValidateConfig(ShapeConfig config)
        {
            if (config.BulletCapacity <= 0)
            {
                Debug.LogError("Circle shape bullet capacity must be greater than 0");
            }
        }

        public override ShapeState GetState()
        {
            return new ShapeState
            {
                Type = ShapeType.Circle,
                Position = transform.position,
                IsActive = isActive,
                CurrentBulletCount = collectedCount
            };
        }
    }
} 