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
        private Vector2 currentPosition;
        private Vector2 currentVelocity;
        private float stunEndTime;
        private bool isStunned;

        public bool IsStunned => isStunned;

        public MovementSystem(MovementConfig config)
        {
            this.config = config;
        }

        public void Initialize(Vector2 startPosition)
        {
            currentPosition = startPosition;
            currentVelocity = Vector2.zero;
            isStunned = false;
            stunEndTime = 0f;
        }

        public void HandleMovement(Vector2 direction)
        {
            if (isStunned) return;

            // 标准化8方向输入
            Vector2 normalizedDir = NormalizeToEightDirections(direction);
            
            // 计算移动
            currentVelocity = normalizedDir * config.MoveSpeed;
            Vector2 newPosition = currentPosition + currentVelocity * Time.deltaTime;

            // 检查边界
            if (IsWithinBounds(newPosition))
            {
                currentPosition = newPosition;
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

        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (!isStunned)
            {
                currentVelocity += direction.normalized * force;
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
                currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, config.KnockbackDrag * Time.deltaTime);
            }

            // 更新位置
            if (currentVelocity.magnitude > 0)
            {
                Vector2 newPosition = currentPosition + currentVelocity * Time.deltaTime;
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
                    currentVelocity = Vector2.zero;
                }
            }
        }

        private bool IsWithinBounds(Vector2 position)
        {
            return position.x >= config.Bounds.xMin && position.x <= config.Bounds.xMax &&
                   position.y >= config.Bounds.yMin && position.y <= config.Bounds.yMax;
        }

        private bool IsOutOfBounds(Vector2 position)
        {
            return position.y < -50f; // 掉落高度检测
        }

        public Vector2 GetPosition() => currentPosition;
        public Vector2 GetVelocity() => currentVelocity;
    }
} 