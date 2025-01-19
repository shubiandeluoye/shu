using UnityEngine;
using DG.Tweening;
using Core.ObjectPool;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Handles behavior of individual glass fragments
    /// </summary>
    [RequireComponent(typeof(PoolObject))]
    public class GlassFragment : MonoBehaviour
    {
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
        private PoolObject poolObject;
        private Material materialInstance;
        private Sequence fadeSequence;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
            poolObject = GetComponent<PoolObject>();

            // Create unique material instance to avoid affecting other fragments
            if (meshRenderer != null && meshRenderer.material != null)
            {
                materialInstance = new Material(meshRenderer.material);
                meshRenderer.material = materialInstance;
            }
        }

        private void OnDisable()
        {
            // Kill any active tweens when disabled
            fadeSequence?.Kill();
            
            // Reset physics state
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Reset material properties
            if (materialInstance != null)
            {
                Color color = materialInstance.color;
                color.a = 1f;
                materialInstance.color = color;
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
            fadeSequence = DOTween.Sequence()
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
                    if (poolObject != null)
                    {
                        poolObject.RecycleNow();
                    }
                });
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (rb != null && collision.relativeVelocity.magnitude > minCollisionVelocity)
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
        }
    }
}
