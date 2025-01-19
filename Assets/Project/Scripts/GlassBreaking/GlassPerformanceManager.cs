using UnityEngine;
using Core.ObjectPool;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Manages performance optimizations for glass breaking system
    /// </summary>
    public class GlassPerformanceManager : MonoBehaviour
    {
        [Header("Platform Settings")]
        [SerializeField]
        private bool isMobilePlatform;
        
        [Header("LOD Settings")]
        [Tooltip("Distance for full detail")]
        public float closeDistance = 3f;
        [Tooltip("Distance for medium detail")]
        public float mediumDistance = 6f;
        [Tooltip("Distance for minimum detail")]
        public float farDistance = 8f;
        
        [Header("Mobile Optimization")]
        [Tooltip("Physics update interval for distant fragments")]
        public float physicsUpdateInterval = 0.1f;
        [Tooltip("Maximum fragments for mobile")]
        public int mobileMaxFragments = 35;
        
        [Header("References")]
        [SerializeField]
        private Camera mainCamera;
        private GlassPoolConfiguration poolConfig;
        
        private void Awake()
        {
            poolConfig = GetComponent<GlassPoolConfiguration>();
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            // Detect platform
            #if UNITY_ANDROID || UNITY_IOS
            isMobilePlatform = true;
            #else
            isMobilePlatform = false;
            #endif
            
            InitializePlatformSettings();
        }
        
        private void InitializePlatformSettings()
        {
            if (isMobilePlatform)
            {
                // Adjust physics settings for mobile
                Physics.defaultSolverIterations = 4; // Reduce solver iterations
                Physics.defaultSolverVelocityIterations = 1;
                Time.fixedDeltaTime = 0.02f; // 50Hz physics
                
                // Adjust LOD distances for mobile
                closeDistance = 2f;
                mediumDistance = 4f;
                farDistance = 6f;
            }
        }
        
        /// <summary>
        /// Gets the appropriate LOD level based on distance
        /// </summary>
        public LODLevel GetLODLevel(Vector3 position)
        {
            if (mainCamera == null) return LODLevel.Full;
            
            float distance = Vector3.Distance(mainCamera.transform.position, position);
            
            if (distance <= closeDistance)
                return LODLevel.Full;
            else if (distance <= mediumDistance)
                return LODLevel.Medium;
            else if (distance <= farDistance)
                return LODLevel.Minimal;
            else
                return LODLevel.Culled;
        }
        
        /// <summary>
        /// Updates fragment physics based on LOD level
        /// </summary>
        public void UpdateFragmentPhysics(GlassFragment fragment, LODLevel level)
        {
            if (fragment == null) return;
            
            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb == null) return;
            
            switch (level)
            {
                case LODLevel.Full:
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    break;
                    
                case LODLevel.Medium:
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    break;
                    
                case LODLevel.Minimal:
                    rb.isKinematic = true;
                    break;
                    
                case LODLevel.Culled:
                    // Return to pool if too far
                    var poolObject = fragment.GetComponent<PoolObject>();
                    if (poolObject != null)
                    {
                        poolObject.RecycleNow();
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Gets the maximum fragment count based on platform
        /// </summary>
        public int GetMaxFragments()
        {
            return isMobilePlatform ? mobileMaxFragments : 50;
        }
    }
    
    /// <summary>
    /// Level of Detail settings for fragments
    /// </summary>
    public enum LODLevel
    {
        Full,    // Full physics and effects
        Medium,  // Simplified physics
        Minimal, // Static display only
        Culled   // Disabled/recycled
    }
}
