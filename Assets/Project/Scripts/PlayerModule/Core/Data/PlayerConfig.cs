using UnityEngine;

namespace PlayerModule.Data
{
    public class PlayerConfig
    {
        public MovementConfig MovementConfig { get; set; } = new MovementConfig();
        public HealthConfig HealthConfig { get; set; } = new HealthConfig();
        public ShootingConfig ShootingConfig { get; set; } = new ShootingConfig();
    }

    public class MovementConfig
    {
        public float MoveSpeed { get; set; } = 5f;
        public float KnockbackDrag { get; set; } = 3f;
        public Rectangle Bounds { get; set; } = new Rectangle(-3.5f, -3.5f, 7f, 7f);
    }

    public class HealthConfig
    {
        public int MaxHealth { get; set; } = 100;
        public float InvincibilityTime { get; set; } = 0.5f;
    }

    public class ShootingConfig
    {
        public float[] ShootAngles { get; set; } = { 0f, 30f, -30f, 45f, -45f };
        public float ShootCooldown { get; set; } = 0.2f;
        public float BulletSpawnOffset { get; set; } = 0.5f;
    }

    public struct Rectangle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
} 