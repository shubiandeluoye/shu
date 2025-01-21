using UnityEngine;

namespace MapModule.Shapes
{
    public interface IShape
    {
        void Initialize(ShapeConfig config);
        void HandleSkillHit(int skillId, Vector3 hitPoint);
        ShapeState GetState();
        ShapeType GetShapeType();
        void SetPosition(Vector3 position);
        void Reset();
        bool IsActive();
    }
} 