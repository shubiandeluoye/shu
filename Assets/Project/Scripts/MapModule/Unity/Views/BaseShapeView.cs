using UnityEngine;
using MapModule.Core.Data;
using MapModule.Unity.Configs;

namespace MapModule.Unity.Views
{
    public abstract class BaseShapeView : MonoBehaviour, IShapeView
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Transform visualRoot;
        
        protected ShapeType shapeType;
        protected ShapeTypeSO typeConfig;
        protected ParticleSystem hitEffect;
        protected ParticleSystem destroyEffect;

        public virtual void Initialize(ShapeType type, ShapeTypeSO config)
        {
            shapeType = type;
            typeConfig = config;

            // 设置基础视觉效果
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = config.GetSprite();
                spriteRenderer.color = config.GetColor();
            }

            // 创建特效
            if (config.GetHitEffect() != null)
            {
                hitEffect = Instantiate(config.GetHitEffect(), transform);
                hitEffect.Stop();
            }

            if (config.GetDestroyEffect() != null)
            {
                destroyEffect = Instantiate(config.GetDestroyEffect(), transform);
                destroyEffect.Stop();
            }
        }

        public virtual void UpdateState(ShapeState state)
        {
            transform.position = new Vector3(state.Position.X, state.Position.Y, state.Position.Z);
            transform.rotation = Quaternion.Euler(0, 0, state.CurrentRotation);
            gameObject.SetActive(state.IsActive);
        }

        public virtual void OnHit(Vector3 hitPoint)
        {
            if (hitEffect != null)
            {
                hitEffect.transform.position = hitPoint;
                hitEffect.Play();
            }
        }

        public virtual void OnDestroy()
        {
            if (destroyEffect != null)
            {
                var effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }

        protected virtual void OnValidate()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (visualRoot == null)
                visualRoot = transform;
        }
    }
} 