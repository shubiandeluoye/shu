using UnityEngine;
using Core.Singleton;
using UnityEngine.InputSystem;

namespace Core.InputSystem
{
    public class InputManager : Singleton<InputManager>
    {
        private GameInputActions inputActions;

        protected override void Awake()
        {
            base.Awake();
            inputActions = new GameInputActions();
            inputActions.Enable();
        }

        public Vector2 GetMoveDirection()
        {
            return inputActions.Player.Move.ReadValue<Vector2>();
        }

        public bool IsActionPressed(string actionName)
        {
            switch (actionName)
            {
                case "ShootStraight":
                    return inputActions.Player.StraightShoot.WasPressedThisFrame();
                case "LeftAngleShoot":
                    return inputActions.Player.LeftAngleShoot.WasPressedThisFrame();
                case "RightAngleShoot":
                    return inputActions.Player.RightAngleShoot.WasPressedThisFrame();
                default:
                    return false;
            }
        }

        public float GetShootAngle()
        {
            if (inputActions.Player.LeftAngleShoot.WasPressedThisFrame())
                return -45f;
            if (inputActions.Player.RightAngleShoot.WasPressedThisFrame())
                return 45f;
            return 0f;
        }

        #region IPlayerActions Implementation
        public void OnMove(InputAction.CallbackContext context) { }
        public void OnStraightShoot(InputAction.CallbackContext context) { }
        public void OnLeftAngleShoot(InputAction.CallbackContext context) { }
        public void OnRightAngleShoot(InputAction.CallbackContext context) { }
        public void OnToggleAngle(InputAction.CallbackContext context) { }
        public void OnToggleBulletLevel(InputAction.CallbackContext context) { }
        public void OnFireLevel3Bullet(InputAction.CallbackContext context) { }
        #endregion

        private void OnDestroy()
        {
            inputActions?.Dispose();
        }
    }
} 