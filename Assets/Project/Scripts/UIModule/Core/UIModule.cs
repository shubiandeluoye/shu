using UnityEngine;
using System.Collections.Generic;

namespace UIModule.Core
{
    /// <summary>
    /// UI模块基类，用于管理一组相关的UI组件
    /// </summary>
    public abstract class UIModule : MonoBehaviour
    {
        protected Dictionary<string, UIBase> uiComponents;
        protected bool isModuleActive;
        
        // 模块优先级，用于控制显示顺序
        public virtual int Priority => 0;
        
        // 模块类型
        public abstract UIModuleType ModuleType { get; }

        protected virtual void Awake()
        {
            uiComponents = new Dictionary<string, UIBase>();
            Initialize();
        }

        protected virtual void Initialize()
        {
            RegisterComponents();
        }

        /// <summary>
        /// 注册UI组件
        /// </summary>
        protected abstract void RegisterComponents();

        /// <summary>
        /// 添加UI组件
        /// </summary>
        protected void AddComponent(string key, UIBase component)
        {
            if (!uiComponents.ContainsKey(key))
            {
                uiComponents.Add(key, component);
            }
        }

        /// <summary>
        /// 获取UI组件
        /// </summary>
        public T GetComponent<T>(string key) where T : UIBase
        {
            return uiComponents.TryGetValue(key, out UIBase component) ? component as T : null;
        }

        public virtual void ShowModule()
        {
            isModuleActive = true;
            foreach (var component in uiComponents.Values)
            {
                component.Show();
            }
            OnModuleShow();
        }

        public virtual void HideModule()
        {
            isModuleActive = false;
            foreach (var component in uiComponents.Values)
            {
                component.Hide();
            }
            OnModuleHide();
        }

        protected virtual void OnModuleShow() { }
        protected virtual void OnModuleHide() { }

        public virtual void UpdateModule()
        {
            if (!isModuleActive) return;
            foreach (var component in uiComponents.Values)
            {
                component.UpdateUI();
            }
        }

        protected virtual void OnDestroy()
        {
            uiComponents.Clear();
        }
    }

    // UI模块类型
    public enum UIModuleType
    {
        Common,     // 通用模块
        Menu,       // 菜单模块
        Battle,     // 战斗模块
        // 后续可以添加更多模块类型
    }
} 