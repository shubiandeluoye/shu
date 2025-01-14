using UnityEngine;
using System.Collections;
using Core.ObjectPool;
using Gameplay.Core;

namespace Gameplay.Bullet
{
    public class BulletController : MonoBehaviour
    {
        [Header("Bullet Properties")]
        [SerializeField] private float speed = 8f;
        [SerializeField] private float damage = 1f;
        [SerializeField] private Color bulletColor = new Color(1f, 0.5f, 0f);

        private int bounceCount = 0;
        private const int MAX_BOUNCES = 3;
        private Vector2 direction;
        private SpriteRenderer spriteRenderer;
        private CircleCollider2D circleCollider;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            
            circleCollider = GetComponent<CircleCollider2D>();
            if (circleCollider == null)
                circleCollider = gameObject.AddComponent<CircleCollider2D>();

            spriteRenderer.color = bulletColor;
            circleCollider.radius = 0.5f;
        }

        public void Initialize(Vector2 startDirection, float angle)
        {
            direction = Quaternion.Euler(0, 0, angle) * startDirection;
            bounceCount = 0;
            // 移除了 GetObject 调用，因为这应该由外部处理
        }

        private void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (bounceCount >= MAX_BOUNCES)
            {
                ReturnToPool();
                return;
            }

            Vector2 normal = collision.contacts[0].normal;
            direction = Vector2.Reflect(direction, normal);
            bounceCount++;

            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            ObjectPool.Instance.ReturnObject(gameObject);
        }

        private void OnDisable()
        {
            bounceCount = 0;
            direction = Vector2.zero;
        }

        public Vector2 GetDirection() => direction;
        public int GetBounceCount() => bounceCount;
    }
}