using UnityEngine;
using Fusion;
using System;

/// <summary>
/// Handles player movement, health, and network synchronization
/// Implements 8-direction movement with wall collision and out-of-bounds detection
/// </summary>
public class PlayerController : NetworkBehaviour, ITestablePlayer
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector2 movementBounds = new Vector2(7f, 7f);
    [SerializeField] private BoxCollider2D playerCollider;

    [Networked] public float Health { get; set; } = 100f;
    [Networked] public NetworkBool IsAlive { get; set; } = true;
    [Networked] public Vector3 Position { get; set; }
    [Networked] public float ShootAngle { get; set; }

    private InputManager inputManager;
    private EventManager eventManager;
    private Vector2 moveDirection;
    private bool isOutOfBounds;

    public Vector3 ITestablePlayer.Position => transform.position;
    float ITestablePlayer.Health => Health;
    bool ITestablePlayer.IsAlive => IsAlive;

    public override void Spawned()
    {
        inputManager = InputManager.Instance;
        eventManager = EventManager.Instance;
        
        // Set initial collider size
        if (playerCollider == null)
            playerCollider = gameObject.AddComponent<BoxCollider2D>();
        playerCollider.size = Vector2.one; // 1x1 size

        // Subscribe to input events
        inputManager.OnMove += HandleMove;
        inputManager.OnShootSmall += HandleShootStraight;
        inputManager.OnDirectionChange += HandleDirectionalShot;
        inputManager.OnAngleChange += HandleAngleToggle;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Unsubscribe from events
        if (inputManager != null)
            inputManager.OnMove -= HandleMove;
    }

    private void HandleMove(Vector2 direction)
    {
        // Local input handling - will be sent through network input
        if (Object.HasInputAuthority)
        {
            var input = new NetworkInputData
            {
                HorizontalInput = direction.x,
                VerticalInput = direction.y
            };
            Runner.SetPlayerInput(input);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // Get networked input
        if (GetInput(out NetworkInputData input))
        {
            // Apply movement
            Vector3 movement = new Vector3(input.HorizontalInput, input.VerticalInput, 0).normalized * moveSpeed * Runner.DeltaTime;
            Vector3 newPosition = transform.position + movement;

            // Check bounds
            bool wasOutOfBounds = isOutOfBounds;
            isOutOfBounds = CheckOutOfBounds(newPosition);

            // Handle wall collisions and bounds
            newPosition = ClampPosition(newPosition);

            // Update networked position with interpolation
            transform.position = newPosition;
            Position = newPosition;

            // Handle shooting input
            if (input.ShootPressed)
            {
                ShootAngle = input.ShootAngle;
                eventManager.TriggerEvent(new PlayerShootEvent 
                { 
                    PlayerId = Object.Id,
                    Angle = input.ShootAngle
                });
            }

            // Trigger out of bounds event if state changed
            if (wasOutOfBounds != isOutOfBounds && isOutOfBounds)
            {
                eventManager.TriggerEvent(new PlayerOutOfBoundsEvent { PlayerId = Object.Id });
            }
        }
    }

    private bool CheckOutOfBounds(Vector3 position)
    {
        // Fourth wall (right side) is out of bounds
        return position.x > movementBounds.x / 2;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        // Clamp to movement bounds, allowing right side to trigger out of bounds
        float clampedX = Mathf.Clamp(position.x, -movementBounds.x / 2, movementBounds.x / 2);
        float clampedY = Mathf.Clamp(position.y, -movementBounds.y / 2, movementBounds.y / 2);
        return new Vector3(clampedX, clampedY, 0);
    }

    public void TakeDamage(float damage)
    {
        if (!Object.HasStateAuthority) return;
        
        Health = Mathf.Max(0, Health - damage);
        IsAlive = Health > 0;

        if (!IsAlive)
        {
            eventManager.TriggerEvent(new PlayerDefeatedEvent { PlayerId = Object.Id });
        }
    }

    // ITestablePlayer implementation
    public void SimulateMove(Vector2 direction)
    {
        HandleMove(direction);
    }

    public void SimulateShoot(float angle)
    {
        ShootAngle = angle;
    }

    public BulletType GetCurrentBulletType()
    {
        return BulletType.Small; // Default to small bullet for now
    }

    public int GetBounceCount()
    {
        return 3; // Maximum bounce count
    }
}

// Event classes
public class PlayerOutOfBoundsEvent
{
    public int PlayerId { get; set; }
}

public class PlayerDefeatedEvent
{
    public int PlayerId { get; set; }
}

public enum BulletType
{
    Small,
    Medium,
    Large
}
