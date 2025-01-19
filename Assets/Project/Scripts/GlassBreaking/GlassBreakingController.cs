using UnityEngine;
using DG.Tweening;
using Core.ObjectPool;
using Core.EventSystem;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Controls glass breaking behavior including transparency, break patterns, and fragment management.
    /// 
    /// Features:
    /// - Adjustable transparency (0-1 range)
    /// - Multiple break patterns (circular/linear)
    /// - Physics-based fragments with realistic behavior
    /// - Performance optimizations for different platforms
    /// - Object pooling for efficient memory management
    /// 
    /// Usage:
    /// 1. Add GlassBreakingPrefab to scene
    /// 2. Configure transparency, break pattern, and performance settings
    /// 3. Break glass via BreakGlass() method or mouse click
    /// 
    /// Example:
    /// ```csharp
    /// var controller = GetComponent<GlassBreakingController>();
    /// controller.transparency = 0.8f;
    /// controller.useCircularBreak = true;
    /// controller.BreakGlass();
    /// ```
    /// </summary>
    public class GlassBreakingController : MonoBehaviour
    {
        [Header("Glass Properties")]
        [Range(0f, 1f)]
        [Tooltip("Glass transparency (0 = fully transparent, 1 = fully opaque)")]
        public float transparency = 1f;

        [Header("Breaking Configuration")]
        [Tooltip("Use circular break pattern")]
        public bool useCircularBreak = true;
        [Tooltip("Use linear break pattern")]
        public bool useLinearBreak;
        [Range(0.1f, 5f)]
        [Tooltip("Radius of the break effect")]
        public float breakRadius = 1f;
        [Range(3, 10)]
        [Tooltip("Number of break points for pattern generation")]
        public int breakPointsCount = 5;
        
        [Header("Performance")]
        [SerializeField]
        private GlassPerformanceManager performanceManager;

        [Header("Fragment Management")]
        [SerializeField]
        private GlassPoolConfiguration poolConfig;
        [Tooltip("Maximum distance for fragment culling")]
        public float cullingDistance = 10f;
        
        [Header("Fragment Distribution")]
        [Range(0, 1)]
        public float largeFragmentRatio = 0.3f;
        [Range(0, 1)]
        public float mediumFragmentRatio = 0.5f;
        [Range(0, 1)]
        public float smallFragmentRatio = 0.2f;

        [Header("References")]
        [SerializeField]
        private MeshRenderer glassRenderer;
        [SerializeField]
        private ObjectPool objectPool;

        private Material glassMaterial;
        private bool isBroken;

        private void Awake()
        {
            // Create a unique instance of the material to avoid affecting other objects
            if (glassRenderer != null && glassRenderer.material != null)
            {
                glassMaterial = new Material(glassRenderer.material);
                glassRenderer.material = glassMaterial;
            }

            // Get performance manager if not set
            if (performanceManager == null)
            {
                performanceManager = FindObjectOfType<GlassPerformanceManager>();
            }
        }

        private void Start()
        {
            // Initialize transparency
            SetTransparency(transparency);
        }

        /// <summary>
        /// Sets the transparency of the glass material
        /// </summary>
        /// <param name="value">Transparency value (0-1)</param>
        public void SetTransparency(float value, bool animate = false, float duration = 0.5f)
        {
            transparency = Mathf.Clamp01(value);
            if (glassMaterial != null)
            {
                if (animate)
                {
                    // Animate transparency using DOTween
                    Color currentColor = glassMaterial.color;
                    Color targetColor = currentColor;
                    targetColor.a = transparency;
                    
                    DOTween.To(
                        () => glassMaterial.color,
                        color => glassMaterial.color = color,
                        targetColor,
                        duration
                    ).SetEase(Ease.InOutQuad);
                }
                else
                {
                    // Instant transparency change
                    Color color = glassMaterial.color;
                    color.a = transparency;
                    glassMaterial.color = color;
                }
            }
        }

        public void AnimateTransparencyPunch(float strength = 0.2f, float duration = 0.3f)
        {
            if (glassMaterial != null)
            {
                float originalAlpha = glassMaterial.color.a;
                
                // Create a punch effect sequence
                Sequence punchSequence = DOTween.Sequence();
                
                punchSequence.Append(
                    DOTween.To(
                        () => glassMaterial.color.a,
                        value =>
                        {
                            Color color = glassMaterial.color;
                            color.a = value;
                            glassMaterial.color = color;
                        },
                        originalAlpha - strength,
                        duration * 0.5f
                    ).SetEase(Ease.OutQuad)
                );
                
                punchSequence.Append(
                    DOTween.To(
                        () => glassMaterial.color.a,
                        value =>
                        {
                            Color color = glassMaterial.color;
                            color.a = value;
                            glassMaterial.color = color;
                        },
                        originalAlpha,
                        duration * 0.5f
                    ).SetEase(Ease.InQuad)
                );
            }
        }

        /// <summary>
        /// Initiates the glass breaking effect
        /// </summary>
        public void BreakGlass()
        {
            if (isBroken) return;
            isBroken = true;

            // Create breaking effect sequence
            Sequence breakSequence = DOTween.Sequence();
            
            // Add punch effects
            breakSequence.AppendCallback(() => AnimateTransparencyPunch(0.3f, 0.2f));
            
            // Scale punch effect
            Vector3 originalScale = transform.localScale;
            breakSequence.Append(
                transform.DOScale(originalScale * 1.05f, 0.1f)
                    .SetEase(Ease.OutQuad)
            );
            breakSequence.Append(
                transform.DOScale(originalScale, 0.1f)
                    .SetEase(Ease.InQuad)
            );
            
            // Fade out original glass
            breakSequence.Append(
                DOTween.To(
                    () => transparency,
                    value => SetTransparency(value),
                    0f,
                    0.5f
                ).SetEase(Ease.InQuad)
            );
            
            // Spawn fragments
            breakSequence.AppendCallback(SpawnFragments);
        }

        private void SpawnFragments()
        {
            if (objectPool == null)
            {
                Debug.LogError("[GlassBreakingController] ObjectPool reference is missing!");
                return;
            }

            Vector3[] spawnPoints = useCircularBreak ? 
                GenerateCircularBreakPoints() : 
                GenerateLinearBreakPoints();

            int maxAllowedFragments = performanceManager != null ? performanceManager.GetMaxFragments() : 50;
            int fragmentCount = Mathf.Min(spawnPoints.Length, maxAllowedFragments);
            for (int i = 0; i < fragmentCount; i++)
            {
                // Determine fragment size based on ratios
                float random = Random.value;
                FragmentSize fragmentSize;
                
                if (random < largeFragmentRatio)
                    fragmentSize = FragmentSize.Large;
                else if (random < largeFragmentRatio + mediumFragmentRatio)
                    fragmentSize = FragmentSize.Medium;
                else
                    fragmentSize = FragmentSize.Small;

                string poolId = poolConfig.GetPoolIdForSize(fragmentSize);
                GameObject fragment = objectPool.SpawnFromPool(
                    poolId,
                    transform.TransformPoint(spawnPoints[i]),
                    Random.rotation
                );

                if (fragment != null)
                {
                    // Fragment initialization will be handled by the fragment script
                    fragment.SendMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
                }
            }

            // Spawn particle effects
            SpawnBreakEffects();

            // Disable the original glass mesh
            if (glassRenderer != null)
            {
                glassRenderer.enabled = false;
            }
        }

        private void SpawnBreakEffects()
        {
            if (objectPool != null)
            {
                // Spawn break particle system
                GameObject particleSystem = objectPool.SpawnFromPool(
                    "GlassParticleSystems",
                    transform.position,
                    Quaternion.identity
                );

                if (particleSystem != null)
                {
                    var particles = particleSystem.GetComponent<ParticleSystem>();
                    if (particles != null)
                    {
                        // Configure particle system based on break type
                        var main = particles.main;
                        if (useCircularBreak)
                        {
                            // Circular burst configuration
                            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
                            var shape = particles.shape;
                            shape.shapeType = ParticleSystemShapeType.Circle;
                            shape.radius = breakRadius;
                        }
                        else
                        {
                            // Linear burst configuration
                            main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
                            var shape = particles.shape;
                            shape.shapeType = ParticleSystemShapeType.Line;
                            shape.scale = new Vector3(breakRadius * 2f, 0.1f, 0.1f);
                        }

                        particles.Play();

                        // Auto-recycle after duration
                        var poolObject = particleSystem.GetComponent<PoolObject>();
                        if (poolObject != null)
                        {
                            float duration = main.duration + main.startLifetime.constantMax;
                            StartCoroutine(RecycleAfterDelay(poolObject, duration));
                        }
                    }
                }
            }
        }

        private System.Collections.IEnumerator RecycleAfterDelay(PoolObject poolObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (poolObject != null)
            {
                poolObject.RecycleNow();
            }
        }

        private Vector3[] GenerateCircularBreakPoints()
        {
            Vector3[] points = new Vector3[breakPointsCount * 3]; // 3 layers of points
            float angleStep = 360f / breakPointsCount;

            for (int layer = 0; layer < 3; layer++)
            {
                float radius = breakRadius * ((layer + 1) / 3f);
                for (int i = 0; i < breakPointsCount; i++)
                {
                    float angle = i * angleStep;
                    float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    points[layer * breakPointsCount + i] = new Vector3(x, y, 0);
                }
            }

            return points;
        }

        private Vector3[] GenerateLinearBreakPoints()
        {
            Vector3[] points = new Vector3[breakPointsCount * 3]; // 3 rows
            float step = breakRadius * 2 / (breakPointsCount - 1);

            for (int row = 0; row < 3; row++)
            {
                float yOffset = (row - 1) * step * 0.5f;
                for (int i = 0; i < breakPointsCount; i++)
                {
                    float x = -breakRadius + (i * step);
                    points[row * breakPointsCount + i] = new Vector3(x, yOffset, 0);
                }
            }

            return points;
        }

        // Debug visualization
        private void OnDrawGizmosSelected()
        {
            if (!isBroken)
            {
                Gizmos.color = Color.yellow;
                if (useCircularBreak)
                {
                    Gizmos.DrawWireSphere(transform.position, breakRadius);
                }
                else if (useLinearBreak)
                {
                    Vector3 center = transform.position;
                    Gizmos.DrawLine(
                        center + Vector3.left * breakRadius,
                        center + Vector3.right * breakRadius
                    );
                }
            }
        }

        // Example input handling - can be modified based on requirements
        private void OnMouseDown()
        {
            BreakGlass();
        }
    }
}
