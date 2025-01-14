using UnityEngine;
using Core.InputSystem;
using Core.EventSystem;
using Core.ObjectPool;
using Core.FSM;
using Gameplay.Core;
using System.Collections;
using Gameplay.Bullet;
// TODO: 等 AudioManager 就绪后再添加
// using Core.Audio;
namespace Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private GameObject bulletPrefab;
        
        private float currentHealth;
        private Vector2 moveDirection;
        private bool canShoot = true;
        private float shootAngle;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            // 使用 InputManager 获取输入
            Vector2 moveInput = InputManager.Instance.GetMoveDirection();
            float shootAngle = InputManager.Instance.GetShootAngle();

            // 处理移动
            HandleMovement(moveInput);

            // 处理射击
            HandleShooting(shootAngle);
        }

        private void HandleMovement(Vector2 input)
        {
            // 规范化输入向量，确保对角线移动速度一致
            Vector2 normalizedInput = input.normalized;
            
            // 计算新位置
            Vector3 newPosition = transform.position + new Vector3(
                normalizedInput.x * GameplayConstants.Player.DEFAULT_MOVE_SPEED * Time.deltaTime,
                normalizedInput.y * GameplayConstants.Player.DEFAULT_MOVE_SPEED * Time.deltaTime,
                0
            );

            // 限制在游戏区域内
            newPosition.x = Mathf.Clamp(newPosition.x, 
                -GameplayConstants.Bounds.HALF_WIDTH, 
                GameplayConstants.Bounds.HALF_WIDTH);
            newPosition.y = Mathf.Clamp(newPosition.y, 
                -GameplayConstants.Bounds.HALF_HEIGHT, 
                GameplayConstants.Bounds.HALF_HEIGHT);

            transform.position = newPosition;
        }

        private void HandleShooting(float angle)
        {
            if (!canShoot) return;

            // 检查射击输入
            if (InputManager.Instance.IsActionPressed("ShootStraight"))
            {
                Shoot(Vector2.right, angle);
                StartCoroutine(ShootCooldown());
            }
        }

        private void Shoot(Vector2 direction, float angle)
        {
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);

            if (bullet != null)
            {
                // 设置位置
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.identity;
                
                var bulletController = bullet.GetComponent<BulletController>();
                bulletController.Initialize(direction, angle);
                
                // TODO: 等 AudioManager 就绪后再实现
                // AudioManager.Instance.PlaySound("PlayerShoot");
            }
        }

        private IEnumerator ShootCooldown()
        {
            canShoot = false;
            yield return new WaitForSeconds(GameplayConstants.Player.SHOOT_COOLDOWN);
            canShoot = true;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            EventManager.Instance.TriggerEvent(new GameplayEvents.PlayerDamagedEvent
            {
                PlayerId = gameObject.GetInstanceID(),
                Damage = damage,
                RemainingHealth = currentHealth
            });

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // Handle player death
            gameObject.SetActive(false);
            EventManager.Instance.TriggerEvent(new GameplayEvents.GameEndEvent
            {
                LoserId = gameObject.GetInstanceID(),
                Reason = "Player Eliminated"
            });
        }

        public float CurrentHealth => currentHealth;

        // 为测试添加的方法
        public void SimulateShoot(float angle)
        {
            Shoot(Vector2.right, angle);
        }

        public void SimulateMove(Vector2 input)
        {
            HandleMovement(input);
        }

        public float Health => currentHealth;
    }
}
