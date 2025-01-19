using UnityEngine;
using Core.ObjectPool;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Manages the glass breaking demo scene
    /// </summary>
    public class DemoSceneManager : MonoBehaviour
    {
        [Header("Glass Prefabs")]
        public GameObject circularBreakGlass;
        public GameObject linearBreakGlass;
        
        [Header("Spawn Settings")]
        public Vector3 circularGlassPosition = new Vector3(-2f, 1f, 0f);
        public Vector3 linearGlassPosition = new Vector3(2f, 1f, 0f);
        
        private void Start()
        {
            SpawnDemoGlass();
        }
        
        private void SpawnDemoGlass()
        {
            // Spawn circular break glass
            if (circularBreakGlass != null)
            {
                GameObject circular = Instantiate(circularBreakGlass, circularGlassPosition, Quaternion.identity);
                var controller = circular.GetComponent<GlassBreakingController>();
                if (controller != null)
                {
                    controller.useCircularBreak = true;
                    controller.useLinearBreak = false;
                }
            }
            
            // Spawn linear break glass
            if (linearBreakGlass != null)
            {
                GameObject linear = Instantiate(linearBreakGlass, linearGlassPosition, Quaternion.identity);
                var controller = linear.GetComponent<GlassBreakingController>();
                if (controller != null)
                {
                    controller.useCircularBreak = false;
                    controller.useLinearBreak = true;
                }
            }
        }
    }
}
