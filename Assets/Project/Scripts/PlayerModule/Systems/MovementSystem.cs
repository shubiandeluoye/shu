using UnityEngine;
using Core.Network;
using PlayerModule.Data;
using Core.EventSystem; 


namespace PlayerModule
{
    /// <summary>
    /// 移动系统
    /// 负责：
    /// 1. 8方向移动
    /// 2. 眩晕处理
    /// 3. 击退效果
    /// 4. 边界检查
    /// </summary>
    public class MovementSystem
    {
        private readonly MovementConfig config;
        private Vector3 currentPosition;
        private Vector3 currentVelocity;
        private float stunEndTime;
        private bool isStunned;
        private Rigidbody rb;

        public bool IsStunned => isStunned;

        public MovementSystem(MovementConfig config)
        {
            this.config = config;
        }

        public void Initialize(Vector3 startPosition)
        {
            currentPosition = startPosition;
            currentVelocity = Vector3.zero;
            isStunned = false;
            stunEndTime = 0f;
        }

        public void HandleMovement(Vector3 direction)
        {
            if (isStunned) return;

            // 确保在XZ平面上移动
            Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
            currentVelocity = flatDirection * config.MoveSpeed;
            
            // 使用Rigidbody移动
            if (rb != null)
            {
                rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z);
            }
        }

        private Vector2 NormalizeToEightDirections(Vector2 input)
        {
            // 将输入转换为8个固定方向之一
            float angle = Mathf.Atan2(input.y, input.x);
            angle = Mathf.Round(angle / (Mathf.PI / 4)) * (Mathf.PI / 4);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        }

        public void ApplyStun(float duration)
        {
            isStunned = true;
            stunEndTime = Time.time + duration;
        }

        public void ApplyKnockback(Vector3 direction, float force)
        {
            if (!isStunned)
            {
                // 确保在XZ平面上击退
                Vector3 knockbackDir = new Vector3(direction.x, 0, direction.z).normalized;
                currentVelocity += knockbackDir * force;
            }
        }

        public void Update()
        {
            // 更新眩晕状态
            if (isStunned && Time.time >= stunEndTime)
            {
                isStunned = false;
            }

            // 应用击退减速
            if (currentVelocity.magnitude > 0.1f)
            {
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, config.KnockbackDrag * Time.deltaTime);
            }

            // 更新位置
            if (currentVelocity.magnitude > 0)
            {
                Vector3 newPosition = currentPosition + currentVelocity * Time.deltaTime;
                if (IsWithinBounds(newPosition))
                {
                    currentPosition = newPosition;
                }
                else
                {
                    // 触发出界事件
                    if (IsOutOfBounds(newPosition))
                    {
                        EventManager.Instance.TriggerEvent(new PlayerOutOfBoundsEvent());
                    }
                    // 停止移动
                    currentVelocity = Vector3.zero;
                }
            }
        }

        private bool IsWithinBounds(Vector3 position)
        {
            // 使用XZ平面检查边界
            return position.x >= config.Bounds.xMin && position.x <= config.Bounds.xMax &&
                   position.z >= config.Bounds.yMin && position.z <= config.Bounds.yMax;
        }

        private bool IsOutOfBounds(Vector3 position)
        {
            return position.y < -50f; // 掉落高度检测
        }

        public Vector3 GetPosition() => currentPosition;
        public Vector3 GetVelocity() => currentVelocity;
    }
} 