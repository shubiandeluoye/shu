using UnityEngine;
using UnityEngine.InputSystem;
using PlayerModule.Data;

namespace PlayerModule.Unity.Components
{
    public class PlayerInput : MonoBehaviour
    {
        private UnityMovementSystem movementSystem;
        private UnityShootingSystem shootingSystem;
        private Vector2 moveInput;

        private void Awake()
        {
            movementSystem = GetComponent<UnityMovementSystem>();
            shootingSystem = GetComponent<UnityShootingSystem>();
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
            if (movementSystem != null)
            {
                movementSystem.Move(new Vector3(moveInput.x, 0, moveInput.y));
            }
        }

        public void OnStraightShoot(InputValue value)
        {
            if (shootingSystem != null)
            {
                shootingSystem.OnStraightShoot(
                    new InputAction.CallbackContext());
            }
        }

        public void OnLeftShoot(InputValue value)
        {
            if (shootingSystem != null)
            {
                shootingSystem.OnAngleShoot(
                    new InputAction.CallbackContext(), 30f);
            }
        }

        public void OnRightShoot(InputValue value)
        {
            if (shootingSystem != null)
            {
                shootingSystem.OnAngleShoot(
                    new InputAction.CallbackContext(), -30f);
            }
        }
    }
} 