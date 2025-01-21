using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIModule.Core
{
    /// <summary>
    /// UI基础组件接口
    /// </summary>
    public interface IUIComponent
    {
        void Show();
        void Hide();
        void UpdateUI();
    }

    /// <summary>
    /// UI基类，所有UI组件的基础
    /// </summary>
    public abstract class UIBase : MonoBehaviour, IUIComponent
    {
        protected CanvasGroup canvasGroup;
        protected RectTransform rectTransform;
        protected bool isInitialized;
        
        // 添加UI状态
        protected bool isVisible;
        
        // 添加动画支持
        protected virtual float showDuration => 0.3f;
        protected virtual float hideDuration => 0.2f;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (isInitialized) return;
            
            // 确保有 CanvasGroup 组件
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
                
            isInitialized = true;
            OnInitialized();
        }

        // 子类可以重写此方法进行初始化
        protected virtual void OnInitialized() { }

        public virtual void Show()
        {
            if (!isInitialized) Initialize();
            
            gameObject.SetActive(true);
            isVisible = true;
            
            if (canvasGroup != null)
            {
                // 可以在这里添加显示动画
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            
            OnShow();
        }

        public virtual void Hide()
        {
            isVisible = false;
            
            if (canvasGroup != null)
            {
                // 可以在这里添加隐藏动画
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            
            gameObject.SetActive(false);
            OnHide();
        }

        // 子类可以重写这些方法来处理显示/隐藏逻辑
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        public abstract void UpdateUI();

        protected virtual void OnDestroy()
        {
            // 清理资源
        }
    }
} 