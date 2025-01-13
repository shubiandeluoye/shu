using UnityEngine;
using System.Collections;

/// <summary>
/// Controls bullet behavior including movement, collision, and bouncing
/// </summary>
public class BulletController : MonoBehaviour
{
    [Header("Bullet Properties")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private Color bulletColor = new Color(1f, 0.5f, 0f); // Orange

    private int bounceCount = 0;
    private const int MAX_BOUNCES = 3;
    private Vector2 direction;
    private PoolObject poolObject;
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

        poolObject = GetComponent<PoolObject>();
        
        // Set up visual properties
        spriteRenderer.color = bulletColor;
        circleCollider.radius = 0.5f;
    }

    public void Initialize(Vector2 startDirection, float angle)
    {
        direction = Quaternion.Euler(0, 0, angle) * startDirection;
        bounceCount = 0;
        
        // Random lifetime between 8-10 seconds
        float lifetime = Random.Range(8f, 10f);
        poolObject.Initialize("Bullet", ObjectPool.Instance, lifetime);
    }

    private void Update()
    {
        // Move bullet
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bounceCount >= MAX_BOUNCES)
        {
            ReturnToPool();
            return;
        }

        // Calculate bounce direction
        Vector2 normal = collision.contacts[0].normal;
        direction = Vector2.Reflect(direction, normal);
        bounceCount++;

        // Handle player hit
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (poolObject != null)
            ObjectPool.Instance.ReturnToPool("Bullet", gameObject);
    }

    private void OnDisable()
    {
        // Reset state when returned to pool
        bounceCount = 0;
        direction = Vector2.zero;
    }
}
