using UnityEngine;
using MapModule.Core.Data;

namespace MapModule.Unity.Views
{
    public class TrapezoidShapeView : BaseShapeView
    {
        [SerializeField] private ParticleSystem shootEffect;
        [SerializeField] private Transform topShootPoint;
        [SerializeField] private Transform bottomShootPoint;

        public override void Initialize(ShapeType type, ShapeTypeSO config)
        {
            base.Initialize(type, config);
            if (shootEffect != null)
            {
                shootEffect.Stop();
            }
        }

        public override void UpdateState(ShapeState state)
        {
            base.UpdateState(state);
            
            if (state.ActionType == ShapeActionType.Shoot)
            {
                var shootData = state.ActionData as ShootData;
                if (shootData != null && shootEffect != null)
                {
                    shootEffect.transform.position = shootData.IsAccelerated ? 
                        bottomShootPoint.position : topShootPoint.position;
                    
                    var main = shootEffect.main;
                    main.simulationSpeed = shootData.IsAccelerated ? 1.5f : 1f;
                    
                    shootEffect.Play();
                }
            }
        }
    }
} 