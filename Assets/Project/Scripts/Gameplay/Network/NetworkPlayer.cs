using UnityEngine;
using Fusion;

/// <summary>
/// Handles network synchronization for player state and actions
/// </summary>
public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public Vector3 Position { get; set; }
    [Networked] public float Health { get; set; }
    [Networked] public int Score { get; set; }
    [Networked] public NetworkBool IsOutOfBounds { get; set; }
    
    private NetworkRunner _runner;
    private PlayerController _controller;
    private const float INITIAL_HEALTH = 100f;
    
    public override void Spawned()
    {
        _runner = Object.Runner;
        _controller = GetComponent<PlayerController>();
        
        if (Object.HasStateAuthority)
        {
            Health = INITIAL_HEALTH;
            Score = 0;
            Position = transform.position;
            
            EventManager.Instance.TriggerEvent(new PlayerSpawnedEvent 
            { 
                PlayerId = Object.Id.PlayerId,
                Position = Position
            });
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // Get and apply network input
            if (GetInput(out NetworkInputData input))
            {
                Vector3 movement = new Vector3(input.HorizontalInput, input.VerticalInput, 0)
                    .normalized * _controller.moveSpeed * Runner.DeltaTime;
                    
                Vector3 newPosition = transform.position + movement;
                
                // Check bounds and update position
                IsOutOfBounds = _controller.CheckOutOfBounds(newPosition);
                newPosition = _controller.ClampPosition(newPosition);
                
                transform.position = newPosition;
                Position = newPosition;
                
                // Handle shooting input
                if (input.ShootPressed)
                {
                    RPC_Shoot(input.ShootAngle);
                }
            }
        }
        else
        {
            // Interpolate position for non-authority objects
            transform.position = Vector3.Lerp(transform.position, Position, Runner.DeltaTime * 10f);
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_Shoot(float angle)
    {
        if (!Object.HasStateAuthority) return;
        
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        var bullet = Runner.Spawn(_runner.GetComponent<NetworkPrefabsConfig>().bulletPrefab,
            transform.position + (Vector3)(direction * 0.5f),
            Quaternion.Euler(0, 0, angle));
            
        var bulletController = bullet.GetComponent<BulletController>();
        bulletController.Initialize(direction);
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!Object.HasStateAuthority) return;
        
        if (other.CompareTag("Bullet"))
        {
            Health -= 1f; // 1 point damage
            Runner.Despawn(other.GetComponent<NetworkObject>());
            
            EventManager.Instance.TriggerEvent(new PlayerDamagedEvent 
            { 
                PlayerId = Object.Id.PlayerId,
                Damage = 1f,
                RemainingHealth = Health
            });
            
            if (Health <= 0)
            {
                EventManager.Instance.TriggerEvent(new GameEndEvent 
                { 
                    LoserId = Object.Id.PlayerId,
                    Reason = "Player eliminated"
                });
            }
        }
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        EventManager.Instance.TriggerEvent(new PlayerDespawnedEvent 
        { 
            PlayerId = Object.Id.PlayerId
        });
    }
}
