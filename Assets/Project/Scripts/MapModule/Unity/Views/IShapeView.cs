using UnityEngine;
using MapModule.Core.Data;

namespace MapModule.Unity.Views
{
    public interface IShapeView
    {
        void Initialize(ShapeType type, ShapeTypeSO typeConfig);
        void UpdateState(ShapeState state);
        void OnHit(Vector3 hitPoint);
        void OnDestroy();
    }
} 