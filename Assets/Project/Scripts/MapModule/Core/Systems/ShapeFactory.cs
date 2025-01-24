using System;
using System.Collections.Generic;
using MapModule.Core.Data;
using MapModule.Core.Shapes;

namespace MapModule.Core.Systems
{
    public class ShapeFactory
    {
        private readonly Dictionary<ShapeType, Queue<IShape>> shapePools;
        private readonly IMapManager manager;
        private const int INITIAL_POOL_SIZE = 5;

        public ShapeFactory(IMapManager manager)
        {
            this.manager = manager;
            shapePools = new Dictionary<ShapeType, Queue<IShape>>();
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (ShapeType type in Enum.GetValues(typeof(ShapeType)))
            {
                if (type == ShapeType.None) continue;
                shapePools[type] = new Queue<IShape>();
                PrewarmPool(type, INITIAL_POOL_SIZE);
            }
        }

        private void PrewarmPool(ShapeType type, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var shape = CreateNewShape(type);
                shapePools[type].Enqueue(shape);
            }
        }

        private IShape CreateNewShape(ShapeType type)
        {
            return type switch
            {
                ShapeType.Circle => new CircleShape(manager),
                ShapeType.Rectangle => new RectangleShape(manager),
                ShapeType.Triangle => new TriangleShape(manager),
                ShapeType.Trapezoid => new TrapezoidShape(manager),
                _ => throw new ArgumentException($"Unknown shape type: {type}")
            };
        }

        public IShape GetShape(ShapeType type)
        {
            if (!shapePools.ContainsKey(type))
            {
                throw new ArgumentException($"No pool for shape type: {type}");
            }

            IShape shape;
            if (shapePools[type].Count > 0)
            {
                shape = shapePools[type].Dequeue();
            }
            else
            {
                shape = CreateNewShape(type);
            }

            return shape;
        }

        public void ReturnShape(IShape shape)
        {
            if (shape == null) return;

            var type = shape.GetShapeType();
            if (!shapePools.ContainsKey(type))
            {
                shapePools[type] = new Queue<IShape>();
            }

            shape.Reset();
            shapePools[type].Enqueue(shape);
        }

        public void Clear()
        {
            shapePools.Clear();
        }
    }
} 