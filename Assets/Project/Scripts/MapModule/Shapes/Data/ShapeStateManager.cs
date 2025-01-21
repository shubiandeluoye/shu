using UnityEngine;
using System.Collections.Generic;
using MapModule.Shapes;

namespace MapModule.Shapes
{
    public class ShapeStateManager
    {
        private Dictionary<int, ShapeState> shapeStates = new Dictionary<int, ShapeState>();

        public void RegisterState(ShapeType type, ShapeState state)
        {
            shapeStates.Add((int)type, state);
        }

        public ShapeState GetState(ShapeType type)
        {
            return shapeStates.TryGetValue((int)type, out var state) ? state : null;
        }

        public void Clear()
        {
            shapeStates.Clear();
        }
    }
} 