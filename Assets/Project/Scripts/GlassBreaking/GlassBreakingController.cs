using UnityEngine;
using System.Collections;
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
        [Header("基础设置")]
        [Range(0, 1)] public float transparency = 0.8f;
        public bool useCircularBreak = true;
        public bool useLinearBreak { get => !useCircularBreak; set => useCircularBreak = !value; }
        public float breakRadius = 1.5f;
        public int maxFragments = 50;
        public float cullingDistance = 10f;

        [Header("对象池设置")]
        public string fragmentPoolId = "GlassFragmentsPool";
        public string particlePoolId = "GlassParticlesPool";

        [Header("引用设置")]
        public Material glassMaterial;
        public GameObject[] fragmentPrefabs;
        public GameObject breakParticlePrefab;

        private MeshRenderer meshRenderer;
        private bool isBroken = false;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (glassMaterial != null)
            {
                SetupGlassMaterial();
            }
        }

        private void SetupGlassMaterial()
        {
            Material instanceMaterial = new Material(glassMaterial);
            instanceMaterial.SetFloat("_Transparency", transparency);
            meshRenderer.material = instanceMaterial;
        }

        public void BreakGlass(Vector3 impactPoint)
        {
            if (isBroken) return;
            isBroken = true;

            SpawnBreakEffects(impactPoint);
            SpawnGlassFragments(impactPoint);
            
            meshRenderer.enabled = false;
            GetComponent<Collider>().enabled = false;
        }

        private void SpawnBreakEffects(Vector3 position)
        {
            if (ObjectPool.Instance != null && breakParticlePrefab != null)
            {
                GameObject particleObj = ObjectPool.Instance.SpawnFromPool(
                    particlePoolId, 
                    position, 
                    Quaternion.identity
                );

                if (particleObj != null)
                {
                    var particles = particleObj.GetComponent<ParticleSystem>();
                    if (particles != null)
                    {
                        var main = particles.main;
                        var shape = particles.shape;

                        if (useCircularBreak)
                        {
                            shape.shapeType = ParticleSystemShapeType.Circle;
                            shape.radius = breakRadius;
                        }
                        else
                        {
                            shape.shapeType = ParticleSystemShapeType.Rectangle;
                            shape.scale = new Vector3(breakRadius * 2f, 0.1f, 0.1f);
                        }

                        particles.Play();
                        
                        // 延迟回收粒子系统
                        ObjectPool.Instance.ReturnToPoolDelayed(
                            particlePoolId,
                            particleObj,
                            particles.main.duration + particles.main.startLifetime.constantMax
                        );
                    }
                }
            }
        }

        private void SpawnGlassFragments(Vector3 impactPoint)
        {
            int fragmentCount = Mathf.Min(maxFragments, 50);
            float angleStep = 360f / fragmentCount;

            for (int i = 0; i < fragmentCount; i++)
            {
                float angle = i * angleStep;
                Vector3 direction = useCircularBreak ? 
                    Quaternion.Euler(0, angle, 0) * Vector3.forward : 
                    Vector3.right;

                Vector3 position = impactPoint + direction * Random.Range(0f, breakRadius);
                
                SpawnFragment(position, direction);
            }
        }

        private void SpawnFragment(Vector3 position, Vector3 direction)
        {
            if (ObjectPool.Instance != null && fragmentPrefabs.Length > 0)
            {
                GameObject prefab = fragmentPrefabs[Random.Range(0, fragmentPrefabs.Length)];
                GameObject fragment = ObjectPool.Instance.SpawnFromPool(fragmentPoolId, position, Random.rotation);

                if (fragment != null)
                {
                    var glassFragment = fragment.GetComponent<GlassFragment>();
                    if (glassFragment != null)
                    {
                        glassFragment.OnSpawn();
                    }

                    var rb = fragment.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = direction * Random.Range(2f, 5f);
                        rb.angularVelocity = Random.insideUnitSphere * 5f;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (meshRenderer != null && meshRenderer.material != null)
            {
                Destroy(meshRenderer.material);
            }
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
            }
        }

        // Example input handling - can be modified based on requirements
        private void OnMouseDown()
        {
            BreakGlass(transform.position);
        }

        public void SetTransparency(float value)
        {
            transparency = Mathf.Clamp01(value);
            if (meshRenderer != null && meshRenderer.material != null)
            {
                meshRenderer.material.SetFloat("_Transparency", transparency);
            }
        }
    }
}
