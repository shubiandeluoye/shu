using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.GlassBreaking;

namespace Project.GlassBreaking
{
    /// <summary>
    /// Manages UI for the glass breaking demo scene
    /// </summary>
    public class DemoUI : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI instructionsText;
        public TextMeshProUGUI glassTypeText;
        public Slider transparencySlider;
        public Toggle breakModeToggle;
        public Text statusText;
        
        [Header("Glass References")]
        public GlassBreakingController[] glassControllers;
        
        private void Start()
        {
            InitializeUI();
        }
        
        private void InitializeUI()
        {
            if (instructionsText != null)
            {
                instructionsText.text = "Click on glass to break\n" +
                    "Left: Circular Break Pattern\n" +
                    "Right: Linear Break Pattern\n" +
                    "Slider: Adjust Transparency";
            }
            
            if (transparencySlider != null)
            {
                transparencySlider.value = glassControllers[0]?.transparency ?? 0.8f;
                transparencySlider.onValueChanged.AddListener(OnTransparencyChanged);
            }
            
            if (breakModeToggle != null)
            {
                breakModeToggle.isOn = glassControllers[0]?.useCircularBreak ?? true;
                breakModeToggle.onValueChanged.AddListener(OnBreakModeChanged);
            }
            
            UpdateGlassTypeText();
            UpdateStatusText();
        }
        
        private void OnTransparencyChanged(float value)
        {
            foreach (var controller in glassControllers)
            {
                if (controller != null)
                {
                    controller.SetTransparency(value);
                }
            }
            UpdateStatusText();
        }
        
        private void OnBreakModeChanged(bool isCircular)
        {
            foreach (var controller in glassControllers)
            {
                if (controller != null)
                {
                    controller.useCircularBreak = isCircular;
                }
            }
            UpdateStatusText();
        }
        
        private void UpdateGlassTypeText()
        {
            if (glassTypeText != null)
            {
                glassTypeText.text = "Platform: " + (Application.isMobilePlatform ? "Mobile" : "PC") +
                    "\nMax Fragments: " + (Application.isMobilePlatform ? "35" : "50");
            }
        }
        
        private void UpdateStatusText()
        {
            if (statusText != null && glassControllers.Length > 0)
            {
                var controller = glassControllers[0];
                string mode = controller.useCircularBreak ? "圆形" : "线性";
                statusText.text = $"透明度: {controller.transparency:F2}\n破碎模式: {mode}";
            }
        }
    }
}
