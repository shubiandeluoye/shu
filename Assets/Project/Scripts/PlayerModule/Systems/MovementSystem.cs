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
        private Transform transform;

        public bool IsStunned => isStunned;

        public MovementSystem(MovementConfig config, Rigidbody rigidbody)
        {
            this.config = config;
            this.rb = rigidbody;
            Debug.Log($"[MovementSystem] Initialized with Rigidbody: {(rb != null ? "Success" : "Failed")}");
            this.transform = rigidbody.transform;
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
            if (rb == null) return;

            // 将输入方向转换为8方向
            Vector3 eightDirMovement = ConvertToEightDirections(direction);
            
            // 设置速度（不强制设置y值，让它自然下落）
            Vector3 targetVelocity = eightDirMovement * config.MoveSpeed;
            targetVelocity.y = rb.velocity.y;  // 保持当前的Y轴速度
            rb.velocity = targetVelocity;

            // 只在有输入时才旋转
            if (eightDirMovement != Vector3.zero)
            {
                // 计算目标旋转（只绕Y轴）
                Quaternion targetRotation = Quaternion.LookRotation(eightDirMovement);
                transform.rotation = targetRotation;
            }
        }

        private Vector3 ConvertToEightDirections(Vector3 input)
        {
            if (input.magnitude < 0.01f) return Vector3.zero;

            // 计算角度
            float angle = Mathf.Atan2(input.z, input.x);
            // 将角度转换为8个方向中的一个（每45度一个方向）
            angle = Mathf.Round(angle / (Mathf.PI / 4)) * (Mathf.PI / 4);
            
            // 转换回向量（只在XZ平面上）
            return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
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