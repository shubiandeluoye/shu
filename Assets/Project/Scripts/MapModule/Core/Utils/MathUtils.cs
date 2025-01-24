using System;

namespace MapModule.Core.Utils
{
    public struct Vector2D
    {
        public float X { get; }
        public float Y { get; }

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2D Zero => new Vector2D(0, 0);
    }

    public struct Vector3D
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Vector3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3D Zero => new Vector3D(0, 0, 0);
    }

    public static class MathUtils
    {
        private static Random random = new Random();

        public const float PI = 3.14159265359f;
        public const float Deg2Rad = PI / 180f;
        public const float Rad2Deg = 180f / PI;

        public static float Sin(float x) => (float)System.Math.Sin(x);
        public static float Cos(float x) => (float)System.Math.Cos(x);
        public static float Abs(float x) => x < 0 ? -x : x;
        
        public static float Distance(Vector2D a, Vector2D b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (float)System.Math.Sqrt(dx * dx + dy * dy);
        }

        public static float GetRandomRange(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
} 