using System.Collections.Generic;
using PlayerModule.Data;
using PlayerModule.Core.Data;

namespace PlayerModule.Core.Systems
{
    public class PlayerManager
    {
        private static PlayerManager instance;
        public static PlayerManager Instance => instance ??= new PlayerManager();

        private readonly Dictionary<int, PlayerState> players = new Dictionary<int, PlayerState>();
        private readonly PlayerEventSystem eventSystem;

        private PlayerManager()
        {
            eventSystem = PlayerEventSystem.Instance;
        }

        public void RegisterPlayer(int playerId, PlayerConfig config)
        {
            if (players.ContainsKey(playerId)) return;

            var state = new PlayerState();
            players[playerId] = state;

            // 创建并初始化各个系统
            var healthSystem = new HealthSystem(config.HealthConfig);
            var movementSystem = new MovementSystem(config.MovementConfig);
            var shootingSystem = new ShootingSystem(config.ShootingConfig);

            // 发布玩家创建事件
            eventSystem.Publish(PlayerEvents.PlayerCreated, new PlayerCreatedEvent 
            { 
                PlayerId = playerId,
                Config = config
            });
        }

        public void UnregisterPlayer(int playerId)
        {
            if (!players.ContainsKey(playerId)) return;

            // 发布玩家移除事件
            eventSystem.Publish(PlayerEvents.PlayerRemoved, new PlayerRemovedEvent 
            { 
                PlayerId = playerId 
            });

            players.Remove(playerId);
        }

        public PlayerState GetPlayerState(int playerId)
        {
            return players.TryGetValue(playerId, out var state) ? state : null;
        }

        public void UpdatePlayer(int playerId, float deltaTime)
        {
            if (!players.TryGetValue(playerId, out var state)) return;

            // 更新各个系统
            // 这里的具体实现取决于您的需求
        }

        public void HandleInput(int playerId, PlayerInputData input)
        {
            if (!players.TryGetValue(playerId, out var state)) return;

            // 处理输入
            // 这里的具体实现取决于您的需求
        }

        public void Clear()
        {
            players.Clear();
            eventSystem.Clear();
        }
    }
} 