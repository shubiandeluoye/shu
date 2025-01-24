using UnityEngine;
using Core.Singleton;
using Core.Network;
using Core.EventSystem;
using PlayerModule.Data;
using Fusion;
using PlayerModule.Systems;  
using UnityEngine.InputSystem;

namespace PlayerModule
{
    /// <summary>
    /// 玩家系统总管理器
    /// 负责：
    /// 1. 玩家的生成和初始化
    /// 2. 管理玩家状态
    /// 3. 处理玩家输入
    /// 4. 网络同步
    /// </summary>
    public class PlayerSystemManager : NetworkBehaviour
    {
        #region 内部系统引用
        private MovementSystem movementSystem;
        private HealthSystem healthSystem;
        private ShootingSystem shootingSystem;
        private NetworkObject networkObject;
        private Rigidbody rb;
        private bool isGrounded = false;
        #endregion

        #region 配置参数
        [Header("玩家基础配置")]
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform shootPoint;
        #endregion

        #region 网络状态
        [Networked] private NetworkBool IsInitialized { get; set; }
        [Networked] private int CurrentHealth { get; set; }
        [Networked] private Vector2 CurrentPosition { get; set; }
        [Networked] private float CurrentAngle { get; set; }
        #endregion

        public override void Spawned()
        {
            base.Spawned();
            InitializeSystems();
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            // 初始状态：只锁定旋转，允许自由落地
            rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                            RigidbodyConstraints.FreezeRotationZ;
            
            // 初始化输入系统
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.onActionTriggered += OnInputAction;
            }
        }

        private void InitializeSystems()
        {
            movementSystem = new MovementSystem(playerConfig.MovementConfig);
            healthSystem = new HealthSystem(playerConfig.HealthConfig);
            shootingSystem = new ShootingSystem(playerConfig.ShootingConfig, gameObject);

            RegisterEventListeners();
        }

        private void RegisterEventListeners()
        {
            EventManager.Instance.AddListener<PlayerDamageEvent>(OnPlayerDamage);
            EventManager.Instance.AddListener<PlayerStunEvent>(OnPlayerStun);
            EventManager.Instance.AddListener<PlayerOutOfBoundsEvent>(OnPlayerOutOfBounds);
            EventManager.Instance.AddListener<PlayerHealEvent>(OnPlayerHeal);
        }

        #region 公共接口
        public void InitializePlayer(Vector3 spawnPosition)
        {
            if (!Object.HasStateAuthority) return;

            CurrentHealth = playerConfig.HealthConfig.MaxHealth;
            CurrentPosition = spawnPosition;
            CurrentAngle = 0f;
            IsInitialized = true;

            movementSystem.Initialize(spawnPosition);
            healthSystem.Initialize();
            shootingSystem.Initialize(shootPoint);
        }

        public void HandleInput(PlayerInputData inputData)
        {
            if (!IsInitialized) return;

            if (inputData.HasMovementInput)
            {
                HandleMovementInput(inputData);
            }

            if (inputData.HasShootInput)
            {
                HandleShootInput(inputData.ShootInputType);
            }

            if (inputData.IsAngleTogglePressed)
            {
                ToggleShootAngle();
            }
        }

        public PlayerState GetPlayerState()
        {
            return new PlayerState
            {
                Health = CurrentHealth,
                Position = CurrentPosition,
                ShootAngle = CurrentAngle,
                IsStunned = movementSystem.IsStunned,
                CurrentBulletType = shootingSystem.CurrentBulletType
            };
        }

        public void ApplyHeal(int healAmount)
        {
            if (!Object.HasStateAuthority) return;
            healthSystem.ModifyHealth(healAmount, ModifyHealthType.Heal);
        }
        #endregion

        #region 内部方法
        private void HandleMovementInput(PlayerInputData inputData)
        {
            if (!Object.HasStateAuthority) return;

            if (inputData.HasMovementInput)
            {
                movementSystem.HandleMovement(inputData.MovementDirection);
            }
        }

        private void HandleShootInput(ShootInputType inputType)
        {
            if (!Object.HasStateAuthority) return;

            float angle = GetShootAngle(inputType);
            shootingSystem.Shoot(angle);
        }

        private float GetShootAngle(ShootInputType inputType)
        {
            switch (inputType)
            {
                case ShootInputType.Straight:
                    return 0f;
                case ShootInputType.Left:
                    return CurrentAngle == 30f ? -30f : -45f;
                case ShootInputType.Right:
                    return CurrentAngle == 30f ? 30f : 45f;
                default:
                    return 0f;
            }
        }

        private void ToggleShootAngle()
        {
            if (!Object.HasStateAuthority) return;
            CurrentAngle = CurrentAngle == 30f ? 45f : 30f;
        }
        #endregion

        #region 事件处理
        private void OnPlayerDamage(PlayerDamageEvent evt)
        {
            if (!Object.HasStateAuthority) return;

            healthSystem.ModifyHealth(-evt.Damage, ModifyHealthType.Damage);
            if (evt.HasKnockback)
            {
                movementSystem.ApplyKnockback(evt.DamageDirection, playerConfig.MovementConfig.KnockbackDrag);
            }
        }

        private void OnPlayerStun(PlayerStunEvent evt)
        {
            if (!Object.HasStateAuthority) return;
            movementSystem.ApplyStun(evt.Duration);
        }

        private void OnPlayerOutOfBounds(PlayerOutOfBoundsEvent evt)
        {
            if (!Object.HasStateAuthority) return;
            
            EventManager.Instance.TriggerEvent(new PlayerDeathEvent
            {
                DeathPosition = CurrentPosition,
                DeathReason = DeathReason.OutOfBounds
            });
        }

        private void OnPlayerHeal(PlayerHealEvent evt)
        {
            if (!Object.HasStateAuthority) return;
            ApplyHeal(evt.HealAmount);
        }
        #endregion

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            EventManager.Instance.RemoveListener<PlayerDamageEvent>(OnPlayerDamage);
            EventManager.Instance.RemoveListener<PlayerStunEvent>(OnPlayerStun);
            EventManager.Instance.RemoveListener<PlayerOutOfBoundsEvent>(OnPlayerOutOfBounds);
            EventManager.Instance.RemoveListener<PlayerHealEvent>(OnPlayerHeal);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 检测是否碰到地面
            if (!isGrounded && collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
                // 落地后：锁定Y轴位置和XZ轴旋转
                rb.constraints = RigidbodyConstraints.FreezePositionY | 
                               RigidbodyConstraints.FreezeRotationX | 
                               RigidbodyConstraints.FreezeRotationZ;
            }
        }

        private void OnInputAction(InputAction.CallbackContext context)
        {
            if (!IsInitialized) return;

            // 处理移动输入
            if (context.action.name == "Move")
            {
                Vector2 input = context.ReadValue<Vector2>();
                Vector3 moveDirection = new Vector3(input.x, 0, input.y);
                HandleMovementInput(new PlayerInputData 
                { 
                    HasMovementInput = true,
                    MovementDirection = moveDirection 
                });
            }
            
            // ... 其他输入处理
        }
    }
} 