using UnityEngine;
using SkillModule.Core; // 使用我们之前定义的Vector3

namespace PlayerModule.Data
{
    public struct PlayerDamageEvent
    {
        public int Damage { get; set; }
        public Vector3 DamageDirection { get; set; }
        public bool HasKnockback { get; set; }
    }

    public struct PlayerStunEvent
    {
        public float Duration { get; set; }
    }

    public struct PlayerOutOfBoundsEvent
    {
        public int PlayerId { get; set; }
    }

    public struct PlayerHealthChangedEvent
    {
        public int CurrentHealth { get; set; }
        public int PreviousHealth { get; set; }
        public int ChangeAmount { get; set; }
        public ModifyHealthType ModifyType { get; set; }
    }

    public struct PlayerDeathEvent
    {
        public Vector3 DeathPosition { get; set; }
        public DeathReason DeathReason { get; set; }
    }

    public struct PlayerHealEvent
    {
        public int HealAmount { get; set; }
        public ModifyHealthType HealType { get; set; }
        public object Source { get; set; }
    }

    public struct PlayerInputData
    {
        public bool HasMovementInput { get; set; }
        public Vector3 MovementDirection { get; set; }
        public bool HasShootInput { get; set; }
        public ShootInputType ShootInputType { get; set; }
        public bool IsAngleTogglePressed { get; set; }
    }

    public struct PlayerShootEvent
    {
        public int SkillId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float[] Parameters { get; set; }
    }
}

namespace PlayerModule.Core.Data
{
    public static class PlayerEvents
    {
        // 生命值相关
        public const string HealthChanged = "PlayerHealthChanged";
        public const string PlayerDied = "PlayerDied";
        public const string PlayerRevived = "PlayerRevived";

        // 移动相关
        public const string PlayerMoved = "PlayerMoved";
        public const string PlayerStunned = "PlayerStunned";
        public const string PlayerKnockback = "PlayerKnockback";

        // 射击相关
        public const string PlayerShoot = "PlayerShoot";
        public const string BulletTypeChanged = "BulletTypeChanged";
        public const string WeaponChanged = "WeaponChanged";

        // 状态相关
        public const string StateChanged = "PlayerStateChanged";
        public const string OutOfBounds = "PlayerOutOfBounds";

        // 玩家管理相关
        public const string PlayerCreated = "PlayerCreated";
        public const string PlayerRemoved = "PlayerRemoved";
        public const string PlayerInitialized = "PlayerInitialized";
        public const string PlayerUpdated = "PlayerUpdated";
    }
}

// 添加新的事件数据结构
public struct PlayerCreatedEvent
{
    public int PlayerId { get; set; }
    public PlayerConfig Config { get; set; }
}

public struct PlayerRemovedEvent
{
    public int PlayerId { get; set; }
}

public struct PlayerUpdatedEvent
{
    public int PlayerId { get; set; }
    public PlayerState State { get; set; }
    public float DeltaTime { get; set; }
} 