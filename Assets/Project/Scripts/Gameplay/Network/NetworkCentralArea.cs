using UnityEngine;
using Fusion;

/// <summary>
/// Networked implementation of the central area mechanics
/// </summary>
public class NetworkCentralArea : NetworkBehaviour
{
    [Networked] public int CollectedCount { get; set; }
    [Networked] public CentralAreaState CurrentState { get; set; }
    [Networked] public TickTimer StateTimer { get; set; }
    
    private const float MIN_CHARGE_TIME = 5f;
    private const float MAX_CHARGE_TIME = 8f;
    private const float AUTO_EXPLODE_TIME = 30f;
    
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            CurrentState = CentralAreaState.Collecting;
            StateTimer = TickTimer.CreateFromSeconds(Runner, AUTO_EXPLODE_TIME);
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        
        switch (CurrentState)
        {
            case CentralAreaState.Collecting:
                if (CollectedCount >= 21)
                {
                    RPC_ChangeState(CentralAreaState.Charging);
                }
                else if (StateTimer.Expired(Runner))
                {
                    RPC_ChangeState(CentralAreaState.Firing);
                }
                break;
                
            case CentralAreaState.Charging:
                if (StateTimer.Expired(Runner))
                {
                    RPC_ChangeState(CentralAreaState.Firing);
                }
                break;
                
            case CentralAreaState.Firing:
                if (StateTimer.Expired(Runner))
                {
                    RPC_ChangeState(CentralAreaState.Collecting);
                }
                break;
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ChangeState(CentralAreaState newState)
    {
        var oldState = CurrentState;
        CurrentState = newState;
        
        switch (newState)
        {
            case CentralAreaState.Collecting:
                CollectedCount = 0;
                StateTimer = TickTimer.CreateFromSeconds(Runner, AUTO_EXPLODE_TIME);
                break;
                
            case CentralAreaState.Charging:
                float chargeTime = Random.Range(MIN_CHARGE_TIME, MAX_CHARGE_TIME);
                StateTimer = TickTimer.CreateFromSeconds(Runner, chargeTime);
                break;
                
            case CentralAreaState.Firing:
                StateTimer = TickTimer.CreateFromSeconds(Runner, 1f); // Short firing duration
                FireCollectedBullets();
                break;
        }
        
        EventManager.Instance.TriggerEvent(new CentralAreaStateChangedEvent 
        { 
            OldState = oldState,
            NewState = newState,
            StateTime = Runner.SimulationTime,
            CollectedCount = CollectedCount
        });
    }
    
    private void FireCollectedBullets()
    {
        if (!Object.HasStateAuthority) return;
        
        float angleStep = 360f / CollectedCount;
        for (int i = 0; i < CollectedCount; i++)
        {
            float angle = i * angleStep;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
            
            var bullet = Runner.Spawn(Runner.GetComponent<NetworkPrefabsConfig>().bulletPrefab,
                transform.position + (Vector3)(direction * 0.5f),
                Quaternion.Euler(0, 0, angle));
                
            var bulletController = bullet.GetComponent<BulletController>();
            bulletController.Initialize(direction);
        }
        
        CollectedCount = 0;
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!Object.HasStateAuthority || CurrentState != CentralAreaState.Collecting) return;
        
        if (other.CompareTag("Bullet"))
        {
            CollectedCount++;
            Runner.Despawn(other.GetComponent<NetworkObject>());
        }
    }
}
