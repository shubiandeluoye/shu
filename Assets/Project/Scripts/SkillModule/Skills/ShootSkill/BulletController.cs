using UnityEngine;
using Core.EventSystem;

namespace SkillModule.Skills
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BulletController : MonoBehaviour
    {
        // 在 BulletController 内部定义 DamageEvent
        private struct DamageEvent
        {
            public GameObject Source;      // 伤害来源
            public GameObject Target;      // 伤害目标
            public float Damage;          // 伤害值
            public Vector3 Position;      // 击中位置
            public Vector3 Direction;     // 击中方向
        }

        private Rigidbody2D rb;
        private float damage;
        private float lifetime;
        private float elapsedTime;
        private bool isActive = true;
        private EventManager eventManager;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            eventManager = EventManager.Instance;
        }

        public void Initialize(Vector2 velocity, float damage, float lifetime)
        {
            this.damage = damage;
            this.lifetime = lifetime;
            this.elapsedTime = 0f;
            this.isActive = true;

            rb.velocity = velocity;
            transform.right = velocity.normalized;
        }

        private void Update()
        {
            if (!isActive) return;

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= lifetime)
            {
                DestroyBullet();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;

            // 检查层级
            if (!IsValidTarget(other.gameObject)) return;

            // 发送伤害事件
            eventManager.TriggerEvent(new DamageEvent
            {
                Source = gameObject,
                Target = other.gameObject,
                Damage = damage,
                Direction = rb.velocity.normalized,
                Position = transform.position
            });

            DestroyBullet();
        }

        private bool IsValidTarget(GameObject target)
        {
            // 这里可以添加目标验证逻辑
            return true;
        }

        private void SpawnHitEffect()
        {
            // 这里可以实现击中特效
            // 比如粒子效果、声音等
        }

        private void DestroyBullet()
        {
            isActive = false;
            Destroy(gameObject);
        }
    }
} 