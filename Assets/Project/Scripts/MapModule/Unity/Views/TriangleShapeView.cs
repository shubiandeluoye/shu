using UnityEngine;
using MapModule.Core.Data;

namespace MapModule.Unity.Views
{
    public class TriangleShapeView : BaseShapeView
    {
        [SerializeField] private ParticleSystem rotateEffect;
        private bool isRotating;

        public override void UpdateState(ShapeState state)
        {
            base.UpdateState(state);
            
            if (state.IsRotating != isRotating)
            {
                isRotating = state.IsRotating;
                if (isRotating && rotateEffect != null)
                {
                    rotateEffect.Play();
                }
                else if (!isRotating && rotateEffect != null)
                {
                    rotateEffect.Stop();
                }
            }
        }
    }
} 