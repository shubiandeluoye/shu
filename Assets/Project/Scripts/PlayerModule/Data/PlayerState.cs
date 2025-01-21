using UnityEngine;

namespace PlayerModule.Data
{
    public class PlayerState
    {
        public int Health { get; set; }
        public Vector2 Position { get; set; }
        public float ShootAngle { get; set; }
        public bool IsStunned { get; set; }
        public BulletType CurrentBulletType { get; set; }
    }

    public enum BulletType
    {
        Small,
        Medium,
        Large
    }

    public enum ShootInputType
    {
        Straight,
        Left,
        Right
    }

    public enum ModifyHealthType
    {
        Damage,
        Heal,
        LifeSteal,
        Sacrifice
    }

    public enum DeathReason
    {
        HealthDepleted,
        OutOfBounds,
        Disconnected
    }
} 