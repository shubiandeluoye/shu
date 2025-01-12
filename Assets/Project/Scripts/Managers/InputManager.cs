using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Core;

namespace Core.Managers
{
    /// <summary>
    /// 输入管理器
    /// 处理PC和移动平台的输入管理
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        #pragma warning disable 0067
        public event Action OnShootSmall;
        public event Action OnShootMedium;
        public event Action OnShootLarge;
        #pragma warning restore 0067

        // 输入事件
        public event Action<Vector2> OnMove;
        public event Action OnAngleChange;
        public event Action<int> OnDirectionChange; // -1 左, 0 直线, 1 右

        private PlayerInput playerInput;
        private Dictionary<string, InputActionMap> actionMaps;

        protected override void Awake()
        {
            base.Awake();
            InitializeInput();
        }

        private void InitializeInput()
        {
            // 创建不同控制方案的输入动作映射
            actionMaps = new Dictionary<string, InputActionMap>();
            
            // PC控制
            var pcMap = new InputActionMap("PCControls");
            
            // 移动 (WASD)
            pcMap.AddAction("Move", binding: "<Keyboard>/w,<Keyboard>/s,<Keyboard>/a,<Keyboard>/d");
            
            // 射击 (J/K/N)
            pcMap.AddAction("ShootStraight", binding: "<Keyboard>/j");
            pcMap.AddAction("ShootLeft", binding: "<Keyboard>/k");
            pcMap.AddAction("ShootRight", binding: "<Keyboard>/n");
            
            // 子弹类型 (Q/E)
            pcMap.AddAction("SwitchBullet", binding: "<Keyboard>/q,<Keyboard>/e");
            
            // 角度改变 (M)
            pcMap.AddAction("ChangeAngle", binding: "<Keyboard>/m");

            actionMaps["PC"] = pcMap;

            // 移动端控制（使用Input System的屏幕控制）
            var mobileMap = new InputActionMap("MobileControls");
            mobileMap.AddAction("Move", binding: "<Gamepad>/leftStick");
            // 添加其他移动端特定控制...

            actionMaps["Mobile"] = mobileMap;

            // 默认启用PC控制
            SetControlScheme("PC");
        }

        /// <summary>
        /// 设置控制方案
        /// </summary>
        /// <param name="scheme">控制方案名称</param>
        public void SetControlScheme(string scheme)
        {
            foreach (var map in actionMaps.Values)
            {
                map.Disable();
            }

            if (actionMaps.TryGetValue(scheme, out var actionMap))
            {
                actionMap.Enable();
            }
        }

        private void OnEnable()
        {
            // 订阅输入事件
            if (actionMaps.TryGetValue("PC", out var pcMap))
            {
                pcMap["Move"].performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
                pcMap["ShootStraight"].performed += ctx => OnDirectionChange?.Invoke(0);
                pcMap["ShootLeft"].performed += ctx => OnDirectionChange?.Invoke(-1);
                pcMap["ShootRight"].performed += ctx => OnDirectionChange?.Invoke(1);
                pcMap["ChangeAngle"].performed += ctx => OnAngleChange?.Invoke();
            }
        }

        private void OnDisable()
        {
            foreach (var map in actionMaps.Values)
            {
                map.Disable();
            }
        }

        /// <summary>
        /// 检查指定动作是否被按下
        /// </summary>
        /// <param name="actionName">动作名称</param>
        /// <param name="scheme">控制方案名称</param>
        /// <returns>是否按下</returns>
        public bool IsActionPressed(string actionName, string scheme = "PC")
        {
            if (actionMaps.TryGetValue(scheme, out var actionMap))
            {
                return actionMap[actionName].IsPressed();
            }
            return false;
        }
    }
}
