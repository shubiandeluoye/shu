using UnityEngine;
using PlayerModule.Core.Systems;
using PlayerModule.Data;

namespace PlayerModule.Unity.Systems
{
    public class UnityMovementSystem : MonoBehaviour
    {
        private MovementSystem coreSystem;
        private Rigidbody rb;
        
        [SerializeField] private float groundCheckDistance = 2f;
        [SerializeField] private float desiredHeight = 1.0f;
        [SerializeField] private float downForce = 30f;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            SetupRigidbody();
        }

        public void Initialize(MovementSystem system)
        {
            coreSystem = system;
            // 订阅核心系统的移动事件
            // 处理物理表现
        }

        private void SetupRigidbody()
        {
            if (rb != null)
            {
                rb.freezeRotation = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.useGravity = true;
            }
        }

        private void Update()
        {
            if (coreSystem != null)
            {
                coreSystem.Update(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            HandleGroundCheck();
            UpdatePhysics();
        }

        private void HandleGroundCheck()
        {
            if (rb == null) return;

            RaycastHit hit;
            Vector3 rayStart = rb.position + Vector3.up * 0.1f;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance))
            {
                float currentHeight = rb.position.y;
                if (currentHeight > desiredHeight)
                {
                    float forceMagnitude = (currentHeight - desiredHeight) * downForce;
                    rb.AddForce(Vector3.down * forceMagnitude, ForceMode.Acceleration);
                }
            }
        }

        private void UpdatePhysics()
        {
            if (rb == null) return;
            
            Vector3 corePosition = coreSystem.GetPosition();
            Vector3 coreVelocity = coreSystem.GetVelocity();
            
            // 平滑更新位置和速度
            rb.MovePosition(corePosition);
            rb.velocity = coreVelocity;
        }
    }
} 