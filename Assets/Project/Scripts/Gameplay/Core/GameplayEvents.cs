using UnityEngine;

/// <summary>
/// Contains all gameplay-related event classes
/// </summary>
public class GameplayEvents
{
    public class PlayerSpawnedEvent
    {
        public int PlayerId { get; set; }
        public Vector3 Position { get; set; }
    }

    public class PlayerDamagedEvent
    {
        public int PlayerId { get; set; }
        public float Damage { get; set; }
        public float RemainingHealth { get; set; }
    }

    public class GameStartEvent
    {
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
    }

    public class GameEndEvent
    {
        public int WinnerId { get; set; }
        public int LoserId { get; set; }
        public string Reason { get; set; }
    }

    public class PlayerOutOfBoundsEvent
    {
        public int PlayerId { get; set; }
        public Vector2 Position { get; set; }
    }

    public class CentralAreaChargingEvent
    {
        public Vector2 Position { get; set; }
        public int BulletCount { get; set; }
    }

    public class CentralAreaExplodedEvent
    {
        public Vector2 Position { get; set; }
        public int BulletCount { get; set; }
        public float ChargeTime { get; set; }
    }
}
