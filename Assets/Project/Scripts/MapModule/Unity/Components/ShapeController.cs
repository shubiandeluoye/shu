using UnityEngine;
using MapModule.Core.Data;
using MapModule.Core.Utils;

namespace MapModule.Unity.Components
{
    public class ShapeController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private ParticleSystem destroyEffect;

        private ShapeType currentType;
        private bool isActive;

        public void Initialize(ShapeType type)
        {
            currentType = type;
            isActive = true;
            UpdateVisual();
        }

        public void UpdateState(ShapeState state)
        {
            transform.position = ToUnityVector3(state.Position);
            transform.rotation = Quaternion.Euler(0, 0, state.CurrentRotation);
            isActive = state.IsActive;

            // 根据形状类型更新特定表现
            switch (state.Type)
            {
                case ShapeType.Rectangle:
                    UpdateRectangleVisual(state.GridState);
                    break;
                case ShapeType.Circle:
                    UpdateCircleVisual(state.CurrentBulletCount);
                    break;
                // ... 其他形状的特殊表现
            }
        }

        public void OnHit(Vector3D hitPoint)
        {
            if (hitEffect != null)
            {
                hitEffect.transform.position = ToUnityVector3(hitPoint);
                hitEffect.Play();
            }
        }

        public void OnDestroy()
        {
            if (destroyEffect != null)
            {
                var effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }

        private void UpdateVisual()
        {
            // 根据形状类型设置外观
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = GetShapeSprite(currentType);
            }
        }

        private Sprite GetShapeSprite(ShapeType type)
        {
            // 从资源管理器获取对应形状的Sprite
            return null; // TODO: 实现资源加载
        }

        private static Vector3 ToUnityVector3(Vector3D v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        private void UpdateRectangleVisual(bool[,] gridState)
        {
            // 更新网格显示
        }

        private void UpdateCircleVisual(int bulletCount)
        {
            // 更新子弹计数显示
        }
    }
} 