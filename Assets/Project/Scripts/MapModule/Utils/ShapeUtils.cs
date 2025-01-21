using UnityEngine;
using MapModule.Shapes;

namespace MapModule.Utils
{
    public static class ShapeUtils
    {
        public static bool IsPointInShape(Vector2 point, Vector2[] vertices)
        {
            int i, j;
            bool result = false;
            for (i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
            {
                if ((vertices[i].y > point.y) != (vertices[j].y > point.y) &&
                    (point.x < (vertices[j].x - vertices[i].x) * (point.y - vertices[i].y) / 
                    (vertices[j].y - vertices[i].y) + vertices[i].x))
                {
                    result = !result;
                }
            }
            return result;
        }

        public static Vector2[] GenerateRegularPolygon(int sides, float radius)
        {
            Vector2[] points = new Vector2[sides];
            float angleStep = 360f / sides;
            
            for (int i = 0; i < sides; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                points[i] = new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );
            }
            
            return points;
        }

        public static bool CheckCollision(Vector2 point, ShapeType type, Vector2 shapePosition, Vector2 shapeSize)
        {
            Vector2 localPoint = point - shapePosition;
            
            switch (type)
            {
                case ShapeType.Circle:
                    return Vector2.Distance(Vector2.zero, localPoint) <= shapeSize.x / 2;
                    
                case ShapeType.Rectangle:
                    return Mathf.Abs(localPoint.x) <= shapeSize.x / 2 && 
                           Mathf.Abs(localPoint.y) <= shapeSize.y / 2;
                    
                case ShapeType.Triangle:
                    Vector2[] trianglePoints = GenerateRegularPolygon(3, shapeSize.x / 2);
                    return IsPointInShape(localPoint, trianglePoints);
                    
                case ShapeType.Trapezoid:
                    Vector2[] trapPoints = new Vector2[4]
                    {
                        new Vector2(-shapeSize.x / 2, -shapeSize.y / 2),
                        new Vector2(shapeSize.x / 2, -shapeSize.y / 2),
                        new Vector2(shapeSize.x / 4, shapeSize.y / 2),
                        new Vector2(-shapeSize.x / 4, shapeSize.y / 2)
                    };
                    return IsPointInShape(localPoint, trapPoints);
                    
                default:
                    return false;
            }
        }
    }
} 