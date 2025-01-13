using UnityEngine;

/// <summary>
/// Network-specific event classes
/// </summary>
public class NetworkEvents
{
    public class PlayerSpawnedEvent
    {
        public int PlayerId { get; set; }
        public Vector3 Position { get; set; }
    }
    
    public class PlayerDespawnedEvent
    {
        public int PlayerId { get; set; }
    }
    
    public class PlayerDamagedEvent
    {
        public int PlayerId { get; set; }
        public float Damage { get; set; }
        public float RemainingHealth { get; set; }
    }
    
    public class CentralAreaStateChangedEvent
    {
        public CentralAreaState OldState { get; set; }
        public CentralAreaState NewState { get; set; }
        public float StateTime { get; set; }
        public int CollectedCount { get; set; }
    }
}

/// <summary>
/// Central area state enumeration
/// </summary>
public enum CentralAreaState
{
    Collecting,
    Charging,
    Firing
}
