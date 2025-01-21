using UnityEngine;
using Core.ObjectPool;
using SkillModule.Skills;  // 引用 BulletController 所在的命名空间

namespace MapModule.Shapes
{
    /// <summary>
    /// 形状基类
    /// 定义所有形状的基本属性和行为
    /// </summary>
    public abstract class BaseShape : MonoBehaviour, IShape, IPoolable
    {
        protected ShapeConfig config;
        protected Transform shapeTransform;
        protected Collider2D shapeCollider;
        protected bool isActive;
        protected Vector3 targetPosition;
        private PoolableStatus status = PoolableStatus.None;
        private float spawnTime;
        private int useCount;

        protected virtual void Awake()
        {
            shapeTransform = transform;
            shapeCollider = GetComponent<Collider2D>();
        }

        public virtual void Initialize(ShapeConfig config)
        {
            this.config = config;
            ValidateConfig(config);
            isActive = true;
        }

        public virtual void SetPosition(Vector3 position)
        {
            targetPosition = position;
            transform.position = position;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;
            var bulletController = other.GetComponent<BulletController>();
            if (bulletController != null)
            {
                // 暂时使用默认值，后续再添加 skillId
                HandleSkillHit(101, other.transform.position);  // 101 表示第一个技能
            }
        }

        public virtual void HandleSkillHit(int skillId, Vector3 hitPoint)
        {
            if (!ValidateHitPoint(hitPoint)) return;
            // 直接处理技能效果
            OnShapeHit(skillId, hitPoint);
        }

        protected virtual void OnDisappear()
        {
            isActive = false;
            Destroy(gameObject);
        }

        protected abstract void OnShapeUpdate();
        protected abstract void ValidateConfig(ShapeConfig config);
        protected virtual void OnShapeHit(int skillId, Vector3 hitPoint) { }

        public virtual void Reset()
        {
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            isActive = true;
        }

        protected bool ValidateHitPoint(Vector3 hitPoint)
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

        public ShapeConfig GetConfig()
        {
            return config;
        }

        public virtual ShapeState GetState()
        {
            return new ShapeState
            {
                Type = GetShapeType(),
                Position = transform.position,
                IsActive = isActive
            };
        }

        protected virtual void Update()
        {
            if (!isActive) return;
            OnShapeUpdate();
        }

        protected virtual void SomeMethod()
        {
            // 基类的默认实现
        }

        // IPoolable 实现
        public virtual void OnSpawn()
        {
            isActive = true;
            status = PoolableStatus.Spawned;
            spawnTime = Time.time;
            useCount++;
        }

        public virtual void OnRecycle()
        {
            isActive = false;
            status = PoolableStatus.Recycled;
            Reset();
        }

        public virtual bool CanBeSpawned()
        {
            return status != PoolableStatus.Spawned && status != PoolableStatus.Active;
        }

        public virtual bool CanBeRecycled()
        {
            return isActive;
        }

        public virtual void OnPoolCreate()
        {
            status = PoolableStatus.Created;
        }

        public virtual void OnPoolDestroy()
        {
            status = PoolableStatus.Destroyed;
        }

        public PoolableStatus GetStatus()
        {
            return status;
        }

        public float GetLifetime()
        {
            return Time.time - spawnTime;
        }

        public int GetUseCount()
        {
            return useCount;
        }
    }
} 