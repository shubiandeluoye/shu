using UnityEngine;
using System.Collections.Generic;
using Core.Singleton;
using UIModule.Components;

namespace UIModule.Core
{
    /// <summary>
    /// UI模块管理器：统一管理所有UI模块和组件
    /// </summary>
    public class UIModuleManager : MonoSingleton<UIModuleManager>
    {
        // 暂时注释掉这些UI组件，等需要时再添加
        /*
        [Header("Common UIs")]
        [SerializeField] private LoadingUI loadingUI;
        [SerializeField] private MessageBoxUI messageBoxUI;
        [SerializeField] private ToastUI toastUI;
        */

        private Dictionary<string, UIBase> uiComponents;
        private Stack<UIBase> uiStack;
        private HashSet<string> persistentUIs;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            uiComponents = new Dictionary<string, UIBase>();
            uiStack = new Stack<UIBase>();
            persistentUIs = new HashSet<string>();

            // RegisterCommonUIs();  // 暂时注释掉
        }

        /*
        private void RegisterCommonUIs()
        {
            // 只注册基础通用UI组件
            RegisterUI("Loading", loadingUI, true);
            RegisterUI("MessageBox", messageBoxUI, true);
            RegisterUI("Toast", toastUI, true);
        }
        */

        private void RegisterUI(string key, UIBase ui, bool isPersistent = false)
        {
            if (ui != null && !uiComponents.ContainsKey(key))
            {
                uiComponents.Add(key, ui);
                if (isPersistent)
                {
                    persistentUIs.Add(key);
                }
                ui.Hide();
            }
        }

        #region Public Methods
        public T GetUI<T>(string key) where T : UIBase
        {
            if (uiComponents.TryGetValue(key, out UIBase ui))
            {
                return ui as T;
            }
            return null;
        }

        public void ShowUI(string key)
        {
            if (uiComponents.TryGetValue(key, out UIBase ui))
            {
                if (!persistentUIs.Contains(key))
                {
                    uiStack.Push(ui);
                }
                ui.Show();
            }
        }

        public void HideUI(string key)
        {
            if (uiComponents.TryGetValue(key, out UIBase ui))
            {
                ui.Hide();
                if (!persistentUIs.Contains(key) && uiStack.Count > 0)
                {
                    if (uiStack.Peek() == ui)
                    {
                        uiStack.Pop();
                    }
                }
            }
        }

        /*
        // 快捷方法：暂时注释掉
        public void ShowLoading(string message = "加载中...") { }
        public void HideLoading() { }
        public void ShowMessage(string message, string confirmText = "确定") { }
        public void ShowConfirm(string message, System.Action onConfirm, System.Action onCancel = null) { }
        public void ShowToast(string message, float duration = 2f) { }
        */
        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 清理资源
            uiComponents.Clear();
            uiStack.Clear();
            persistentUIs.Clear();
        }
    }
} 