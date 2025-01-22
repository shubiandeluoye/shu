using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Core.ObjectPool;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Handles behavior of individual glass fragments
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class GlassFragment : MonoBehaviour, IPoolable, IPoolableConfig
    {
        [Header("碎片设置")]
        public float lifetimeLimit = 3f;
        public int maxUseCount = 5;
        public bool autoRecycle = true;
        public float recycleDelay = 0.5f;

        [Header("Fragment Properties")]
        [Tooltip("Initial force applied to fragment")]
        public float initialForce = 5f;
        [Tooltip("Random torque force range")]
        public float randomTorque = 2f;
        [Tooltip("Time before fragment starts fading")]
        public float fadeDelay = 2f;
        [Tooltip("Duration of fade out animation")]
        public float fadeDuration = 1f;
        [Tooltip("Random delay before applying physics (0-1s)")]
        public float maxPhysicsDelay = 0.5f;
        [Tooltip("Air resistance factor")]
        public float airResistance = 0.2f;
        
        [Header("Physics Properties")]
        [Tooltip("Minimum velocity for collision sound")]
        public float minCollisionVelocity = 1f;
        [Tooltip("Scale factor for collision force")]
        public float collisionForceScale = 0.5f;

        private MeshRenderer meshRenderer;
        private Rigidbody rb;
        private Material materialInstance;
        private float spawnTime;
        private int useCount;
        private PoolableStatus status = PoolableStatus.None;

        // IPoolableConfig 实现
        public float LifetimeLimit { get; set; }
        public int MaxUseCount { get; set; }
        public bool AutoRecycle { get; set; }
        public float RecycleDelay { get; set; }

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
            
            // 初始化配置
            LifetimeLimit = lifetimeLimit;
            MaxUseCount = maxUseCount;
            AutoRecycle = autoRecycle;
            RecycleDelay = recycleDelay;
        }

        // IPoolable 实现
        public void OnSpawn()
        {
            status = PoolableStatus.Spawned;
            spawnTime = Time.time;
            useCount++;
            
            if (meshRenderer != null && meshRenderer.material != null)
            {
                materialInstance = new Material(meshRenderer.material);
                meshRenderer.material = materialInstance;
            }

            gameObject.SetActive(true);
        }

        public void OnRecycle()
        {
            status = PoolableStatus.Recycled;
            
            if (materialInstance != null)
            {
                Destroy(materialInstance);
            }

            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            gameObject.SetActive(false);
        }

        public bool CanBeSpawned()
        {
            return useCount < MaxUseCount;
        }

        public bool CanBeRecycled()
        {
            return status == PoolableStatus.Active && 
                   (Time.time - spawnTime >= LifetimeLimit || AutoRecycle);
        }

        public void OnPoolCreate()
        {
            status = PoolableStatus.Created;
            Reset();
        }

        public void OnPoolDestroy()
        {
            status = PoolableStatus.Destroyed;
            if (materialInstance != null)
            {
                Destroy(materialInstance);
            }
        }

        public void Reset()
        {
            useCount = 0;
            spawnTime = 0;
            status = PoolableStatus.None;
            
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        public PoolableStatus GetStatus()
        {
            return status;
        }

        public float GetLifetime()
        {
            return Time.time - spawnTime;
        }

        public int GetUseCount()
        {
            return useCount;
        }

        private void OnEnable()
        {
            // 重置状态
            if (meshRenderer != null && meshRenderer.material != null)
            {
                // 创建材质实例
                materialInstance = new Material(meshRenderer.material);
                meshRenderer.material = materialInstance;
            }

            // 启动生命周期计时
            StartCoroutine(LifetimeRoutine());
        }

        private void OnDisable()
        {
            // 清理材质实例
            if (materialInstance != null)
            {
                Destroy(materialInstance);
            }
            
            // 重置物理状态
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Called when the fragment is spawned from the pool
        /// </summary>
        public void OnSpawned()
        {
            if (rb != null)
            {
                StartCoroutine(InitializePhysics());
            }
        }

        private IEnumerator InitializePhysics()
        {
            if (rb != null)
            {
                // Random delay for more natural breaking effect
                float delay = Random.value * maxPhysicsDelay;
                yield return new WaitForSeconds(delay);

                // Apply random initial force
                Vector3 randomDirection = Random.insideUnitSphere.normalized;
                randomDirection.y = Mathf.Abs(randomDirection.y); // Ensure upward component
                rb.AddForce(randomDirection * initialForce, ForceMode.Impulse);
                
                // Apply random torque
                Vector3 randomTorqueDir = Random.insideUnitSphere * randomTorque;
                rb.AddTorque(randomTorqueDir, ForceMode.Impulse);

                // Configure physics properties
                rb.drag = airResistance;
                rb.angularDrag = airResistance * 2f;
            }

            // Set up fade sequence
            Sequence fadeSequence = DOTween.Sequence()
                .AppendInterval(fadeDelay)
                .Append(DOTween.To(
                    () => materialInstance.color.a,
                    (value) =>
                    {
                        Color color = materialInstance.color;
                        color.a = value;
                        materialInstance.color = color;
                    },
                    0f,
                    fadeDuration
                ))
                .OnComplete(() =>
                {
                    if (rb != null)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                });
        }

        private IEnumerator LifetimeRoutine()
        {
            // 等待生命周期结束
            yield return new WaitForSeconds(LifetimeLimit - fadeDuration);

            // 开始淡出
            float elapsedTime = 0;
            while (elapsedTime < fadeDuration && materialInstance != null)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                materialInstance.SetFloat("_Transparency", alpha);
                yield return null;
            }

            // 返回对象池
            OnRecycle();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > minCollisionVelocity)
            {
                // Calculate collision force
                float collisionForce = collision.relativeVelocity.magnitude * collisionForceScale;
                
                // Apply force at collision point
                Vector3 collisionPoint = collision.contacts[0].point;
                Vector3 collisionNormal = collision.contacts[0].normal;
                rb.AddForceAtPosition(collisionNormal * collisionForce, collisionPoint, ForceMode.Impulse);

                // TODO: Add sound effects based on collision force
                // TODO: Spawn particles at collision point
            }
        }

        private float nextPhysicsUpdate;
        private GlassPerformanceManager performanceManager;
        
        private void Start()
        {
            performanceManager = FindObjectOfType<GlassPerformanceManager>();
            nextPhysicsUpdate = Time.time;
        }
        
        private void FixedUpdate()
        {
            if (rb != null && performanceManager != null)
            {
                // Check LOD level and update physics accordingly
                LODLevel level = performanceManager.GetLODLevel(transform.position);
                performanceManager.UpdateFragmentPhysics(this, level);
                
                // Only apply forces if not kinematic and time to update
                if (!rb.isKinematic && Time.time >= nextPhysicsUpdate)
                {
                    // Apply air resistance
                    Vector3 velocity = rb.velocity;
                    float velocityMagnitude = velocity.magnitude;
                    
                    if (velocityMagnitude > 0.01f)
                    {
                        Vector3 resistanceForce = -velocity.normalized * (velocityMagnitude * velocityMagnitude * airResistance);
                        rb.AddForce(resistanceForce * Time.fixedDeltaTime, ForceMode.Force);
                    }
                    
                    // Schedule next physics update
                    nextPhysicsUpdate = Time.time + performanceManager.physicsUpdateInterval;
                }
            }
        }

        private void OnValidate()
        {
            // Ensure we have required components
            if (GetComponent<Rigidbody>() == null)
            {
                Debug.LogWarning("GlassFragment requires a Rigidbody component!");
            }
            if (GetComponent<MeshRenderer>() == null)
            {
                Debug.LogWarning("GlassFragment requires a MeshRenderer component!");
            }

            // 确保淡出时间不超过生命周期
            if (fadeDuration > LifetimeLimit)
            {
                fadeDuration = LifetimeLimit;
            }
        }
    }
}
