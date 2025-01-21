using UnityEngine;

namespace PlayerModule.Data
{
    public struct PlayerDamageEvent
    {
        public int Damage;
        public Vector2 DamageDirection;
        public bool HasKnockback;
    }

    public struct PlayerStunEvent
    {
        public float Duration;
    }

    public struct PlayerOutOfBoundsEvent
    {
        public int PlayerId;
    }

    public struct PlayerHealthChangedEvent
    {
        public int CurrentHealth;
        public int PreviousHealth;
        public int ChangeAmount;
        public ModifyHealthType ModifyType;
    }

    public struct PlayerDeathEvent
    {
        public Vector3 DeathPosition;
        public DeathReason DeathReason;
    }

    public struct PlayerHealEvent
    {
        public int HealAmount;
        public ModifyHealthType HealType;
        public GameObject Healer;
    }

    public struct PlayerInputData
    {
        public bool HasMovementInput;
        public Vector2 MovementDirection;
        public bool HasShootInput;
        public ShootInputType ShootInputType;
        public bool IsAngleTogglePressed;
    }

    public struct PlayerShootEvent
    {
        public int SkillId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float[] Parameters { get; set; }
    }
} 