using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        
        [Header("Glass References")]
        public GlassBreakingController circularGlass;
        public GlassBreakingController linearGlass;
        
        private void Start()
        {
            SetupUI();
        }
        
        private void SetupUI()
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
                transparencySlider.onValueChanged.AddListener(OnTransparencyChanged);
            }
            
            UpdateGlassTypeText();
        }
        
        private void OnTransparencyChanged(float value)
        {
            if (circularGlass != null)
            {
                circularGlass.SetTransparency(value);
            }
            if (linearGlass != null)
            {
                linearGlass.SetTransparency(value);
            }
        }
        
        private void UpdateGlassTypeText()
        {
            if (glassTypeText != null)
            {
                glassTypeText.text = "Platform: " + (Application.isMobilePlatform ? "Mobile" : "PC") +
                    "\nMax Fragments: " + (Application.isMobilePlatform ? "35" : "50");
            }
        }
    }
}
