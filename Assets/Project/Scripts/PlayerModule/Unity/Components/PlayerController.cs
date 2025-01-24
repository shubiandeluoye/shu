using UnityEngine;
using PlayerModule.Core.Systems;
using PlayerModule.Data;

namespace PlayerModule.Unity.Components
{
    public class PlayerController : MonoBehaviour, IPlayerManager
    {
        [Header("配置")]
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private Transform shootPoint;

        [Header("系统引用")]
        private HealthSystem healthSystem;
        private MovementSystem movementSystem;
        private ShootingSystem shootingSystem;

        [Header("Unity系统")]
        private UnityMovementSystem unityMovement;
        private UnityShootingSystem unityShooting;

        private void Awake()
        {
            // 创建核心系统
            healthSystem = new HealthSystem(playerConfig.HealthConfig);
            movementSystem = new MovementSystem(playerConfig.MovementConfig);
            shootingSystem = new ShootingSystem(playerConfig.ShootingConfig);

            // 获取Unity系统组件
            unityMovement = GetComponent<UnityMovementSystem>();
            unityShooting = GetComponent<UnityShootingSystem>();

            // 初始化系统
            InitializeSystems();
        }

        private void InitializeSystems()
        {
            // 初始化核心系统
            healthSystem.Initialize(this);
            movementSystem.Initialize(this);
            shootingSystem.Initialize(this);

            // 初始化Unity系统
            if (unityMovement != null)
                unityMovement.Initialize(movementSystem);
            
            if (unityShooting != null)
                unityShooting.Initialize(shootingSystem, shootPoint);
        }

        public void PublishEvent<T>(string eventName, T eventData)
        {
            // 处理事件
            switch (eventName)
            {
                case "PlayerHealthChanged":
                    HandleHealthChanged(eventData as PlayerHealthChangedEvent?);
                    break;
                case "PlayerMoved":
                    HandlePlayerMoved(eventData as PlayerMovedEvent?);
                    break;
                case "PlayerShoot":
                    HandlePlayerShoot(eventData as PlayerShootEvent?);
                    break;
                // ... 其他事件处理
            }
        }

        private void HandleHealthChanged(PlayerHealthChangedEvent? evt)
        {
            if (!evt.HasValue) return;
            // 处理生命值变化
        }

        private void HandlePlayerMoved(PlayerMovedEvent? evt)
        {
            if (!evt.HasValue) return;
            // 处理移动
        }

        private void HandlePlayerShoot(PlayerShootEvent? evt)
        {
            if (!evt.HasValue) return;
            // 处理射击
        }

        private void OnDestroy()
        {
            // 清理资源
            healthSystem?.Dispose();
            movementSystem?.Dispose();
            shootingSystem?.Dispose();
        }
    }
} 