using UnityEngine;
using Project.GlassBreaking;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Manages the glass breaking demo scene
    /// </summary>
    public class DemoSceneManager : MonoBehaviour
    {
        [Header("引用")]
        public GlassBreakingController[] glassControllers;
        public Camera mainCamera;

        [Header("设置")]
        public float breakForce = 10f;
        public float raycastDistance = 100f;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // 鼠标左键点击
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, raycastDistance))
                {
                    BreakGlass(hit.point);
                }
            }

            // 切换破碎模式
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var controller in glassControllers)
                {
                    if (controller != null)
                    {
                        controller.useCircularBreak = !controller.useCircularBreak;
                    }
                }
            }
        }

        private void BreakGlass(Vector3 point)
        {
            foreach (var controller in glassControllers)
            {
                if (controller != null)
                {
                    float distance = Vector3.Distance(point, controller.transform.position);
                    if (distance <= breakForce)
                    {
                        controller.BreakGlass(point);
                    }
                }
            }
        }

        // 用于UI调用
        public void ToggleBreakMode()
        {
            foreach (var controller in glassControllers)
            {
                if (controller != null)
                {
                    controller.useCircularBreak = !controller.useCircularBreak;
                }
            }
        }
    }
}
