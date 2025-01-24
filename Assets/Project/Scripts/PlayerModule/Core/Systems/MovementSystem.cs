using PlayerModule.Data;
using SkillModule.Core;
using SkillModule.Utils;

namespace PlayerModule.Core.Systems
{
    public class MovementSystem : IPlayerSystem
    {
        private readonly MovementConfig config;
        private Vector3 currentVelocity;
        private Vector3 currentPosition;
        private float stunEndTime;
        private IPlayerManager manager;

        public bool IsStunned { get; private set; }

        public MovementSystem(MovementConfig config)
        {
            this.config = config;
            this.currentPosition = Vector3.zero;
        }

        public void Initialize(IPlayerManager manager)
        {
            this.manager = manager;
            IsStunned = false;
            stunEndTime = 0;
        }

        public void Move(Vector3 direction)
        {
            if (IsStunned) return;

            Vector3 targetVelocity = direction * config.MoveSpeed;
            currentVelocity = targetVelocity;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Vector3 newPosition = currentPosition + currentVelocity * SkillUtils.GetDeltaTime();
            
            if (IsWithinBounds(newPosition))
            {
                currentPosition = newPosition;
                manager.PublishEvent("PlayerMoved", new PlayerMovedEvent 
                { 
                    Position = currentPosition,
                    Velocity = currentVelocity
                });
            }
        }

        public void ApplyKnockback(Vector3 direction, float force)
        {
            currentVelocity = direction * force;
        }

        public void ApplyStun(float duration)
        {
            IsStunned = true;
            stunEndTime = SkillUtils.GetCurrentTime() + duration;
        }

        public void Update(float deltaTime)
        {
            if (IsStunned && SkillUtils.GetCurrentTime() >= stunEndTime)
            {
                IsStunned = false;
            }
        }

        private bool IsWithinBounds(Vector3 position)
        {
            return position.x >= config.Bounds.X && 
                   position.x <= config.Bounds.X + config.Bounds.Width &&
                   position.z >= config.Bounds.Y && 
                   position.z <= config.Bounds.Y + config.Bounds.Height;
        }

        public Vector3 GetPosition() => currentPosition;
        public Vector3 GetVelocity() => currentVelocity;

        public void Dispose() { }
    }
} 