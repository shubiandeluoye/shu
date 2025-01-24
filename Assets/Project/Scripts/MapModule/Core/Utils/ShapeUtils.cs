using System;

namespace MapModule.Core.Utils
{
    public static class ShapeUtils
    {
        public static bool IsPointInShape(Vector2D point, Vector2D[] vertices)
        {
            int i, j;
            bool result = false;
            for (i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
            {
                if ((vertices[i].Y > point.Y) != (vertices[j].Y > point.Y) &&
                    (point.X < (vertices[j].X - vertices[i].X) * (point.Y - vertices[i].Y) / 
                    (vertices[j].Y - vertices[i].Y) + vertices[i].X))
                {
                    result = !result;
                }
            }
            return result;
        }

        public static Vector2D[] GenerateRegularPolygon(int sides, float radius)
        {
            Vector2D[] points = new Vector2D[sides];
            float angleStep = 360f / sides;
            
            for (int i = 0; i < sides; i++)
            {
                float angle = i * angleStep * MathUtils.Deg2Rad;
                points[i] = new Vector2D(
                    MathUtils.Cos(angle) * radius,
                    MathUtils.Sin(angle) * radius
                );
            }
            
            return points;
        }

        public static bool CheckCollision(Vector2D point, ShapeType type, Vector2D shapePosition, Vector2D shapeSize)
        {
            Vector2D localPoint = new Vector2D(point.X - shapePosition.X, point.Y - shapePosition.Y);
            
            return type switch
            {
                ShapeType.Circle => MathUtils.Distance(Vector2D.Zero, localPoint) <= shapeSize.X / 2,
                ShapeType.Rectangle => MathUtils.Abs(localPoint.X) <= shapeSize.X / 2 && 
                                     MathUtils.Abs(localPoint.Y) <= shapeSize.Y / 2,
                ShapeType.Triangle => IsPointInShape(localPoint, GenerateRegularPolygon(3, shapeSize.X / 2)),
                ShapeType.Trapezoid => IsPointInShape(localPoint, GenerateTrapezoidPoints(shapeSize)),
                _ => false
            };
        }

        private static Vector2D[] GenerateTrapezoidPoints(Vector2D size)
        {
            return new[]
            {
                new Vector2D(-size.X / 2, -size.Y / 2),
                new Vector2D(size.X / 2, -size.Y / 2),
                new Vector2D(size.X / 4, size.Y / 2),
                new Vector2D(-size.X / 4, size.Y / 2)
            };
        }
    }
} 