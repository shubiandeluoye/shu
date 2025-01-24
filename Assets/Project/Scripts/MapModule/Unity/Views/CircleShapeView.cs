using UnityEngine;
using MapModule.Core.Data;
using TMPro;

namespace MapModule.Unity.Views
{
    public class CircleShapeView : BaseShapeView
    {
        [SerializeField] private TextMeshPro bulletCountText;
        [SerializeField] private ParticleSystem collectEffect;

        public override void Initialize(ShapeType type, ShapeTypeSO config)
        {
            base.Initialize(type, config);
            UpdateBulletCount(0);
        }

        public override void UpdateState(ShapeState state)
        {
            base.UpdateState(state);
            UpdateBulletCount(state.CurrentBulletCount);
        }

        public override void OnHit(Vector3 hitPoint)
        {
            base.OnHit(hitPoint);
            if (collectEffect != null)
            {
                collectEffect.transform.position = hitPoint;
                collectEffect.Play();
            }
        }

        private void UpdateBulletCount(int count)
        {
            if (bulletCountText != null)
            {
                bulletCountText.text = count.ToString();
            }
        }
    }
} 