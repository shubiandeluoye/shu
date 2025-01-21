using UnityEngine;
using System.Diagnostics;
using MapModule.Shapes;

namespace MapModule.Utils
{
    public static class MapDebugger
    {
        private static bool isDebugEnabled = false;

        [Conditional("UNITY_EDITOR")]
        public static void EnableDebug(bool enable)
        {
            isDebugEnabled = enable;
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogShapeInfo(string message, ShapeType type, Vector3 position)
        {
            if (!isDebugEnabled) return;
            UnityEngine.Debug.Log($"[Map][Shape:{type}] {message} at {position}");
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawShapeBounds(ShapeType type, Vector3 position, Vector2 size)
        {
            if (!isDebugEnabled) return;
            
            Color debugColor = GetDebugColorForShape(type);
            switch (type)
            {
                case ShapeType.Circle:
                    float radius = size.x / 2;
                    int segments = 8;
                    float angleStep = 360f / segments;
                    for (int i = 0; i < segments; i++)
                    {
                        float angle1 = angleStep * i * Mathf.Deg2Rad;
                        float angle2 = angleStep * (i + 1) * Mathf.Deg2Rad;
                        Vector3 pos1 = position + new Vector3(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius, 0);
                        Vector3 pos2 = position + new Vector3(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius, 0);
                        UnityEngine.Debug.DrawLine(pos1, pos2, debugColor, 0.1f);
                    }
                    break;
                    
                case ShapeType.Rectangle:
                    Vector3 extents = new Vector3(size.x / 2, size.y / 2, 0);
                    UnityEngine.Debug.DrawLine(position + new Vector3(-extents.x, -extents.y), position + new Vector3(extents.x, -extents.y), debugColor, 0.1f);
                    UnityEngine.Debug.DrawLine(position + new Vector3(extents.x, -extents.y), position + new Vector3(extents.x, extents.y), debugColor, 0.1f);
                    UnityEngine.Debug.DrawLine(position + new Vector3(extents.x, extents.y), position + new Vector3(-extents.x, extents.y), debugColor, 0.1f);
                    UnityEngine.Debug.DrawLine(position + new Vector3(-extents.x, extents.y), position + new Vector3(-extents.x, -extents.y), debugColor, 0.1f);
                    break;
            }
        }

        private static Color GetDebugColorForShape(ShapeType type)
        {
            return type switch
            {
                ShapeType.Circle => Color.green,
                ShapeType.Rectangle => Color.blue,
                ShapeType.Triangle => Color.yellow,
                ShapeType.Trapezoid => Color.magenta,
                _ => Color.white
            };
        }
    }
}