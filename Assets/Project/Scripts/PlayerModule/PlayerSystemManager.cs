using UnityEngine;
using Core.Singleton;
using Core.Network;
using Core.EventSystem;
using PlayerModule.Data;
using Fusion;
using PlayerModule.Systems;  
using UnityEngine.InputSystem;
using MyProject04.Input;  // GameInputActions的命名空间
using System;
using System.Threading;

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
    public class PlayerSystemManager : NetworkBehaviour, GameInputActions.IPlayerActions, GameInputActions.ITouchActions
    {
        #region 内部系统引用
        private MovementSystem movementSystem;
        private HealthSystem healthSystem;
        private ShootingSystem shootingSystem;
        private NetworkObject networkObject;
        private Rigidbody rb;
        private bool isGrounded = false;
        private GameInputActions gameInputActions;
        private bool systemsInitialized = false;
        private bool networkInitialized = false;
        private Vector2 lastMoveInput;
        #endregion

        #region 配置参数
        [Header("玩家基础配置")]
        [SerializeField] private PlayerConfig config;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private bool isOfflineTest = true;
        [SerializeField] private MovementConfig movementConfig;
        [Header("Mobile Input")]
        [SerializeField] private bool useMobileInput = false;
        #endregion

        #region 网络状态
        [Networked] private NetworkBool IsInitialized { get; set; }
        [Networked] private int CurrentHealth { get; set; }
        [Networked] private Vector2 CurrentPosition { get; set; }
        #endregion

        private void Awake()
        {
            // 获取必要组件
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("[Player] Rigidbody not found!");
                return;
            }

            if (movementConfig == null)
            {
                Debug.LogError("[Player] MovementConfig not assigned!");
                return;
            }

            // 添加设备类型事件监听
            EventManager.Instance.AddListener<DeviceTypeEvent>(OnDeviceTypeChanged);

            // 初始化系统
            movementSystem = new MovementSystem(config.MovementConfig, rb);
            InitializeInputSystem();
            systemsInitialized = true;
        }

        private void InitializeInputSystem()
        {
            try 
            {
                gameInputActions = new GameInputActions();
                gameInputActions.Player.SetCallbacks(this);
                if (useMobileInput)
                {
                    gameInputActions.Touch.SetCallbacks(this);
                    gameInputActions.Touch.Enable();
                }
                gameInputActions.Player.Enable();
                Debug.Log("[Input] Input system initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Input] Failed to initialize: {e.Message}");
            }
        }

        private void Start()
        {
            float startTime = Time.realtimeSinceStartup;
            Debug.Log($"[Player] Start Begin - RealTime: {startTime:F4}s");
            
            if (isOfflineTest)
            {
                Debug.Log($"[Player] Offline Test Mode - Calling Spawned manually at {Time.realtimeSinceStartup:F4}s");
                Spawned();
            }
            
            Debug.Log($"[Player] Start Complete - RealTime: {Time.realtimeSinceStartup:F4}s, Duration: {(Time.realtimeSinceStartup - startTime):F4}s");

            InitializeSystems();
        }

        public override void Spawned()
        {
            try 
            {
                Debug.Log($"[Player] Spawned Start - RealTime: {Time.realtimeSinceStartup:F4}s");
                
                // 在调用base.Spawned()之前先检查所有必需的组件
                if (rb == null)
                {
                    Debug.LogError("[Player] Rigidbody is null in Spawned!");
                    return;
                }
                
                if (config == null)
                {
                    Debug.LogError("[Player] PlayerConfig is null in Spawned!");
                    return;
                }
                
                base.Spawned();
                
                if (Object != null && Object.HasStateAuthority)
                {
                    IsInitialized = true;
                    Debug.Log($"[Player] Network Initialized - HasStateAuthority: {Object.HasStateAuthority}");
                }
                
                InitializeSystems();
                Debug.Log($"[Player] Spawned Complete - Network: {networkInitialized}, Systems: {systemsInitialized}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Player] Error in Spawned: {e.Message} at {Time.realtimeSinceStartup:F4}s\n{e.StackTrace}");
            }
        }

        private void InitializeSystems()
        {
            try
            {
                // 检查是否在主线程
                if (Thread.CurrentThread.ManagedThreadId != 1)
                {
                    Debug.LogError($"[Systems] Must initialize on main thread! Current ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                    return;
                }
                
                systemsInitialized = true;
                Debug.Log($"[Systems] Initialization Complete - ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Systems] Failed to initialize: {e.Message}");
            }
            
            RegisterEventListeners();
            Debug.Log($"[Systems] InitializeSystems Complete - Time: {Time.time}");
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

            CurrentHealth = config.HealthConfig.MaxHealth;
            CurrentPosition = spawnPosition;
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
            Debug.Log($"[Movement] HasStateAuthority: {Object.HasStateAuthority}"); // 检查网络权限
            
            if (!Object.HasStateAuthority) return;

            if (inputData.HasMovementInput)
            {
                Debug.Log($"[Movement] Sending movement to MovementSystem - Direction: {inputData.MovementDirection}"); // 检查传递给移动系统的方向
                movementSystem.HandleMovement(inputData.MovementDirection);
            }
        }

        private void HandleShootInput(ShootInputType inputType)
        {
            if (!Object.HasStateAuthority) return;

            shootingSystem.Shoot();
        }

        private void ToggleShootAngle()
        {
            if (!Object.HasStateAuthority) return;
        }
        #endregion

        #region 事件处理
        private void OnPlayerDamage(PlayerDamageEvent evt)
        {
            if (!Object.HasStateAuthority) return;

            healthSystem.ModifyHealth(-evt.Damage, ModifyHealthType.Damage);
            if (evt.HasKnockback)
            {
                movementSystem.ApplyKnockback(evt.DamageDirection, config.MovementConfig.KnockbackDrag);
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
            // 移除所有事件监听
            EventManager.Instance.RemoveListener<PlayerDamageEvent>(OnPlayerDamage);
            EventManager.Instance.RemoveListener<PlayerStunEvent>(OnPlayerStun);
            EventManager.Instance.RemoveListener<PlayerOutOfBoundsEvent>(OnPlayerOutOfBounds);
            EventManager.Instance.RemoveListener<PlayerHealEvent>(OnPlayerHeal);
            EventManager.Instance.RemoveListener<DeviceTypeEvent>(OnDeviceTypeChanged);  // 记得移除设备类型事件监听
        }

        // 简化碰撞检测
        private void OnCollisionEnter(Collision collision)
        {
            if (!isGrounded && collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }

        // 键盘输入实现
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!systemsInitialized || movementSystem == null) return;
            
            Vector2 input = context.ReadValue<Vector2>();
            Vector3 moveDirection = new Vector3(input.x, 0, input.y);
            
            if (movementSystem != null)
            {
                movementSystem.HandleMovement(moveDirection);
                Debug.Log($"[Movement] Input: {input}, Direction: {moveDirection}");
            }
        }

        // 触摸输入实现
        public void OnPrimaryFingerPosition(InputAction.CallbackContext context)
        {
            if (!useMobileInput) return;
            Vector2 touchPosition = context.ReadValue<Vector2>();
            HandleTouchInput(touchPosition);
        }

        public void OnSecondaryFingerPosition(InputAction.CallbackContext context)
        {
            if (!useMobileInput) return;
            Vector2 touchPosition = context.ReadValue<Vector2>();
            HandleSecondaryTouch(touchPosition);
        }

        private void HandleTouchInput(Vector2 touchPosition)
        {
            // 实现虚拟摇杆逻辑
        }

        private void HandleSecondaryTouch(Vector2 touchPosition)
        {
            // 处理第二触摸点
        }

        private void FixedUpdate()
        {
            // 在物理更新中检查状态
            if (rb != null && movementSystem != null)
            {
                Debug.Log($"[Physics] Position: {transform.position}, Velocity: {rb.velocity}, LastInput: {lastMoveInput}");
            }
        }

        private void OnValidate()
        {
            // 在编辑器中检查组件
            if (rb == null) rb = GetComponent<Rigidbody>();
            Debug.Log($"[Setup] Rigidbody Reference: {(rb != null ? "Found" : "Missing")}");
            
            if (rb != null)
            {
                Debug.Log($"[Setup] Rigidbody Settings - IsKinematic: {rb.isKinematic}, UseGravity: {rb.useGravity}, Constraints: {rb.constraints}");
            }
        }

        // 其他IPlayerActions接口方法
        public void OnStraightShoot(InputAction.CallbackContext context)
        {
            if (context.performed) HandleShootInput(ShootInputType.Straight);
        }

        public void OnLeftAngleShoot(InputAction.CallbackContext context)
        {
            if (context.performed) HandleShootInput(ShootInputType.Left);
        }

        public void OnRightAngleShoot(InputAction.CallbackContext context)
        {
            if (context.performed) HandleShootInput(ShootInputType.Right);
        }

        public void OnToggleAngle(InputAction.CallbackContext context)
        {
            if (context.performed) ToggleShootAngle();
        }

        public void OnToggleBulletLevel(InputAction.CallbackContext context)
        {
            if (!context.performed || shootingSystem == null) return;
            
            PlayerModule.Data.BulletType nextType = shootingSystem.CurrentBulletType switch
            {
                PlayerModule.Data.BulletType.Small => PlayerModule.Data.BulletType.Medium,
                PlayerModule.Data.BulletType.Medium => PlayerModule.Data.BulletType.Large,
                PlayerModule.Data.BulletType.Large => PlayerModule.Data.BulletType.Small,
                _ => PlayerModule.Data.BulletType.Small
            };
            
            shootingSystem.SwitchBulletType(nextType);
            Debug.Log($"[Shooting] Bullet type switched to: {nextType}");
        }

        public void OnFireLevel3Bullet(InputAction.CallbackContext context)
        {
            if (!context.performed || shootingSystem == null) return;
            
            shootingSystem.SwitchBulletType(PlayerModule.Data.BulletType.Large);
            shootingSystem.Shoot();
            Debug.Log("[Shooting] Fired Level 3 bullet");
        }

        private void Update()
        {
            // 在Update中检查输入状态
            if (gameInputActions != null)  // 只检查gameInputActions
            {
                var moveAction = gameInputActions.Player.Move;
                if (moveAction != null && moveAction.enabled)
                {
                    Vector2 currentValue = moveAction.ReadValue<Vector2>();
                    if (currentValue.magnitude > 0.01f)  // 使用magnitude来检查非零值
                    {
                        Debug.Log($"[Input] Current Move Value in Update: {currentValue}, Magnitude: {currentValue.magnitude:F3}");
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (gameInputActions != null) gameInputActions.Enable();
        }

        private void OnDisable()
        {
            if (gameInputActions != null) gameInputActions.Disable();
        }

        // 处理设备类型变更
        private void OnDeviceTypeChanged(DeviceTypeEvent evt)
        {
            useMobileInput = evt.IsMobileDevice;
            Debug.Log($"[Input] Device type set to: {(useMobileInput ? "Mobile" : "PC")}");
        }
    }
} 