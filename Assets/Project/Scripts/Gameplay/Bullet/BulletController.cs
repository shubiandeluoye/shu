using UnityEngine;
using System.Collections;
using Fusion;

/// <summary>
/// Controls bullet behavior including movement, collision, and bouncing
/// Supports network synchronization and specific shooting angles
/// </summary>
public class BulletController : NetworkBehaviour
{
    [Header("Bullet Properties")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private Color bulletColor = new Color(1f, 0.5f, 0f); // Orange

    [Networked] private int BounceCount { get; set; }
    [Networked] private Vector2 Direction { get; set; }
    [Networked] private TickTimer DestroyTimer { get; set; }
    
    private const int MAX_BOUNCES = 3;
    private PoolObject poolObject;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    
    // Network prediction settings
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            DestroyTimer = TickTimer.CreateFromSeconds(Runner, Random.Range(8f, 10f));
        }
        
        var netObj = GetComponent<NetworkObject>();
        if (netObj != null)
            netObj.PredictionMode = NetworkPredictionMode.Full;
    }

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

    public void Initialize(Vector2 startDirection)
    {
        if (!Object.HasStateAuthority) return;
        
        Direction = startDirection.normalized;
        BounceCount = 0;
        DestroyTimer = TickTimer.CreateFromSeconds(Runner, Random.Range(8f, 10f));
        
        if (poolObject != null)
            poolObject.Initialize("Bullet", ObjectPool.Instance);
    }
    
    // Helper methods for testing and angle-based shooting
    public Vector2 GetDirection() => Direction;
    public int GetBounceCount() => BounceCount;
    
    public static Vector2 GetDirectionFromAngle(float angle)
    {
        // Convert angle to radians and calculate direction
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        
        // Move bullet
        transform.position += (Vector3)(Direction * speed * Runner.DeltaTime);
        
        // Check lifetime
        if (DestroyTimer.Expired(Runner))
        {
            if (poolObject != null)
                ObjectPool.Instance.ReturnToPool("Bullet", gameObject);
            else
                Runner.Despawn(Object);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Object.HasStateAuthority) return;
        
        if (BounceCount >= MAX_BOUNCES)
        {
            if (poolObject != null)
                ObjectPool.Instance.ReturnToPool("Bullet", gameObject);
            else
                Runner.Despawn(Object);
            return;
        }

        // Calculate bounce direction
        Vector2 normal = collision.contacts[0].normal;
        Direction = Vector2.Reflect(Direction, normal);
        BounceCount++;

        // Handle player hit
        var networkPlayer = collision.gameObject.GetComponent<NetworkPlayer>();
        if (networkPlayer != null)
        {
            networkPlayer.TakeDamage(damage);
            if (poolObject != null)
                ObjectPool.Instance.ReturnToPool("Bullet", gameObject);
            else
                Runner.Despawn(Object);
        }
    }

    private void ReturnToPool()
    {
        if (poolObject != null)
            ObjectPool.Instance.ReturnToPool("Bullet", gameObject);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Reset networked state
        if (Object.HasStateAuthority)
        {
            BounceCount = 0;
            Direction = Vector2.zero;
        }
    }
}
