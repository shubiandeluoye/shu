using System;

namespace MapModule.Core.Systems
{
    public class CentralAreaSystem : IDisposable
    {
        private readonly MapConfig config;
        private readonly Vector2D centerOffset;
        private Rectangle centralBounds;
        private Rectangle activeBounds;
        private bool isInitialized;
        private bool isActive;
        private IMapManager manager;

        public CentralAreaSystem(MapConfig config, IMapManager manager)
        {
            this.config = config;
            this.manager = manager;
            this.centerOffset = new Vector2D(0, (config.CentralAreaSize.Y - config.ActiveAreaSize.Y) / 2);
            Initialize();
        }

        private void Initialize()
        {
            // 计算中心区域边界
            centralBounds = new Rectangle(
                -config.CentralAreaSize.X / 2,
                -config.CentralAreaSize.Y / 2,
                config.CentralAreaSize.X,
                config.CentralAreaSize.Y
            );

            // 计算活动区域边界
            activeBounds = new Rectangle(
                -config.ActiveAreaSize.X / 2,
                -config.ActiveAreaSize.Y / 2 + centerOffset.Y,
                config.ActiveAreaSize.X,
                config.ActiveAreaSize.Y
            );

            isInitialized = true;
        }

        public Vector3D GetRandomPosition(float verticalOffset = 0)
        {
            if (!isInitialized) return Vector3D.Zero;

            float x = 0; // 固定在中心线上
            float y = MathUtils.GetRandomRange(
                activeBounds.Y + verticalOffset,
                activeBounds.Y + activeBounds.Height + verticalOffset
            );

            // 限制在有效范围内
            y = MathUtils.Clamp(y, 
                activeBounds.Y - config.VerticalFloatRange,
                activeBounds.Y + activeBounds.Height + config.VerticalFloatRange
            );

            return new Vector3D(x, y, 0);
        }

        public bool IsInCentralArea(Vector3D position)
        {
            if (!isInitialized) return false;
            return centralBounds.Contains(position.X, position.Y);
        }

        public bool IsInActiveArea(Vector3D position)
        {
            if (!isInitialized) return false;
            return activeBounds.Contains(position.X, position.Y);
        }

        public void UpdateAreaState(bool active)
        {
            if (isActive == active) return;
            
            isActive = active;
            manager.PublishEvent("AreaStateChanged", new AreaStateChangedEvent
            {
                CentralAreaSize = config.CentralAreaSize,
                ActiveAreaSize = config.ActiveAreaSize,
                IsActive = isActive
            });
        }

        public Rectangle GetCentralBounds() => centralBounds;
        public Rectangle GetActiveBounds() => activeBounds;

        public void Dispose()
        {
            isInitialized = false;
            isActive = false;
        }
    }

    // 纯C#数学结构
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

    public struct Rectangle
    {
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(float x, float y)
        {
            return x >= X && x <= X + Width &&
                   y >= Y && y <= Y + Height;
        }
    }

    public static class MathUtils
    {
        private static Random random = new Random();

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