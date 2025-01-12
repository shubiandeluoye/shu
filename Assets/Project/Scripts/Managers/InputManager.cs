using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Core;

namespace Core.Managers
{
    public class InputManager : Singleton<InputManager>
    {
        #pragma warning disable 0067
        public event Action OnShootSmall;
        public event Action OnShootMedium;
        public event Action OnShootLarge;
        #pragma warning restore 0067

        public event Action<Vector2> OnMove;
        public event Action OnAngleChange;
        public event Action<int> OnDirectionChange;

        private PlayerInput playerInput;
        private Dictionary<string, InputActionMap> actionMaps;

        protected override void Awake()
        {
            base.Awake();
            InitializeInput();
        }

        private void InitializeInput()
        {
            actionMaps = new Dictionary<string, InputActionMap>();
            
            var pcMap = new InputActionMap("PCControls");
            pcMap.AddAction("Move", binding: "<Keyboard>/w,<Keyboard>/s,<Keyboard>/a,<Keyboard>/d");
            pcMap.AddAction("ShootStraight", binding: "<Keyboard>/j");
            pcMap.AddAction("ShootLeft", binding: "<Keyboard>/k");
            pcMap.AddAction("ShootRight", binding: "<Keyboard>/n");
            pcMap.AddAction("SwitchBullet", binding: "<Keyboard>/q,<Keyboard>/e");
            pcMap.AddAction("ChangeAngle", binding: "<Keyboard>/m");
            actionMaps["PC"] = pcMap;

            var mobileMap = new InputActionMap("MobileControls");
            mobileMap.AddAction("Move", binding: "<Gamepad>/leftStick");
            actionMaps["Mobile"] = mobileMap;

            SetControlScheme("PC");
        }

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
