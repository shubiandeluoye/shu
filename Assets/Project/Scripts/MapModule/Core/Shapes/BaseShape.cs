using MapModule.Core.Data;
using MapModule.Core.Utils;

namespace MapModule.Core.Shapes
{
    public abstract class BaseShape : IShape
    {
        protected ShapeConfig config;
        protected Vector3D position;
        protected bool isActive;
        protected IMapManager manager;

        public BaseShape(IMapManager manager)
        {
            this.manager = manager;
        }

        public virtual void Initialize(ShapeConfig config)
        {
            this.config = config;
            ValidateConfig(config);
            isActive = true;
        }

        public virtual void SetPosition(Vector3D position)
        {
            this.position = position;
        }

        public virtual void HandleSkillHit(int skillId, Vector3D hitPoint)
        {
            if (!ValidateHitPoint(hitPoint)) return;
            OnShapeHit(skillId, hitPoint);
        }

        protected virtual void OnDisappear()
        {
            isActive = false;
            manager.PublishEvent(MapEvents.ShapeDestroyed, new ShapeDestroyedEvent 
            { 
                Type = GetShapeType(),
                Position = position
            });
        }

        protected abstract void OnShapeHit(int skillId, Vector3D hitPoint);
        protected abstract void ValidateConfig(ShapeConfig config);

        public virtual void Reset()
        {
            isActive = true;
        }

        protected virtual bool ValidateHitPoint(Vector3D hitPoint)
        {
            return isActive;
        }

        public virtual ShapeType GetShapeType()
        {
            return config.Type;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public abstract ShapeState GetState();

        public virtual void Update(float deltaTime)
        {
            // 基类不做任何更新
        }
    }
} 