using UnityEngine;

namespace PlayerModule.Data
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/Player/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        public MovementConfig MovementConfig;
        public HealthConfig HealthConfig;
        public ShootingConfig ShootingConfig;
    }

    [System.Serializable]
    public class MovementConfig
    {
        public float MoveSpeed = 5f;
        public float KnockbackDrag = 3f;
        public Rect Bounds = new Rect(-3.5f, -3.5f, 7f, 7f);
    }

    [System.Serializable]
    public class HealthConfig
    {
        public int MaxHealth = 100;
        public float InvincibilityTime = 0.5f;
    }

    [System.Serializable]
    public class ShootingConfig
    {
        public float ShootCooldown = 0.2f;
        public float BulletSpawnOffset = 0.5f;
        public Vector3 ShootPoint = new Vector3(0.5f, 0, 0);
    }
} 