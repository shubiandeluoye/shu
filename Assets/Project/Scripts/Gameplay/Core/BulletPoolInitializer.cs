using UnityEngine;

/// <summary>
/// Initializes the bullet object pool with configured settings
/// </summary>
public class BulletPoolInitializer : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private const int INITIAL_POOL_SIZE = 20;
    private const int MAX_POOL_SIZE = 50;

    private void Start()
    {
        if (bulletPrefab == null)
        {
            // Create bullet prefab dynamically if not assigned
            bulletPrefab = new GameObject("BulletPrefab");
            bulletPrefab.AddComponent<BulletPrefabSetup>();
        }

        // Initialize bullet pool with max size
        ObjectPool.Instance.CreatePool("Bullet", bulletPrefab, INITIAL_POOL_SIZE, MAX_POOL_SIZE);
        Debug.Log($"[BulletPoolInitializer] Created bullet pool with initial size {INITIAL_POOL_SIZE}, max size {MAX_POOL_SIZE}");
    }
}
