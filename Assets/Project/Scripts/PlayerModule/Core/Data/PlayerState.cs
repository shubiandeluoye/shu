using SkillModule.Core;

namespace PlayerModule.Data
{
    public interface IPlayerSystem
    {
        void Initialize(IPlayerManager manager);
        void Dispose();
    }

    public interface IPlayerManager
    {
        void PublishEvent<T>(string eventName, T eventData);
        void RegisterSystem(IPlayerSystem system);
    }

    public class PlayerState
    {
        public int Health { get; set; }
        public Vector3 Position { get; set; }
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