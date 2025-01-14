using UnityEngine;

namespace Gameplay.Core
{
    /// <summary>
    /// 游戏常量配置
    /// </summary>
    public static class GameplayConstants
    {
        // 玩家相关
        public static class Player
        {
            public const float DEFAULT_MOVE_SPEED = 5f;
            public const float DEFAULT_MAX_HEALTH = 100f;
            public const float SHOOT_COOLDOWN = 0.5f;
        }

        // 子弹相关
        public static class Bullet
        {
            public const int MAX_BOUNCES = 3;
            public const float DEFAULT_SPEED = 8f;
            public const float DEFAULT_DAMAGE = 1f;
            public const float LIFETIME_MIN = 8f;
            public const float LIFETIME_MAX = 10f;
        }

        // 对象池配置
        public static class Pool
        {
            public const int INITIAL_POOL_SIZE = 20;
            public const int MAX_POOL_SIZE = 50;
        }

        // 中心区域配置
        public static class CentralArea
        {
            public const float RADIUS = 1f;
            public const int REQUIRED_BULLETS = 21;
            public const float MIN_FIRE_INTERVAL = 5f;
            public const float MAX_FIRE_INTERVAL = 8f;
            public const float AUTO_EXPLODE_TIME = 30f;
        }

        // 游戏区域边界
        public static class Bounds
        {
            public const float PLAY_AREA_WIDTH = 7f;
            public const float PLAY_AREA_HEIGHT = 7f;
            public const float HALF_WIDTH = PLAY_AREA_WIDTH / 2;
            public const float HALF_HEIGHT = PLAY_AREA_HEIGHT / 2;
        }
    }
} 