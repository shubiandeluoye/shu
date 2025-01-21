using UnityEngine;
using Core.EventSystem;
using MapModule.Shapes;

namespace MapModule.Systems
{
    /// <summary>
    /// 中心区域系统
    /// 负责：
    /// 1. 管理中心区域的范围
    /// 2. 处理位置检测
    /// 3. 提供有效位置计算
    /// </summary>
    public class CentralAreaSystem : System.IDisposable
    {
        private readonly MapConfig config;
        private readonly Vector2 centerOffset;
        private Bounds centralBounds;
        private Bounds activeBounds;
        private bool isInitialized;
        private bool isActive;

        public CentralAreaSystem(MapConfig config)
        {
            this.config = config;
            this.centerOffset = new Vector2(0, (config.CentralAreaSize.y - config.ActiveAreaSize.y) / 2);
            Initialize();
        }

        private void Initialize()
        {
            // 计算中心区域边界
            centralBounds = new Bounds(
                Vector3.zero,
                new Vector3(config.CentralAreaSize.x, config.CentralAreaSize.y, 1f)
            );

            // 计算活动区域边界
            activeBounds = new Bounds(
                new Vector3(0, centerOffset.y, 0),
                new Vector3(config.ActiveAreaSize.x, config.ActiveAreaSize.y, 1f)
            );

            isInitialized = true;
        }

        public Vector3 GetRandomPosition(float verticalOffset = 0)
        {
            if (!isInitialized) return Vector3.zero;

            float x = 0; // 固定在中心线上
            float y = Random.Range(
                activeBounds.min.y + verticalOffset,
                activeBounds.max.y + verticalOffset
            );

            // 限制在有效范围内
            y = Mathf.Clamp(y, 
                activeBounds.min.y - config.VerticalFloatRange,
                activeBounds.max.y + config.VerticalFloatRange
            );

            return new Vector3(x, y, 0);
        }

        public bool IsInCentralArea(Vector3 position)
        {
            if (!isInitialized) return false;
            return centralBounds.Contains(position);
        }

        public bool IsInActiveArea(Vector3 position)
        {
            if (!isInitialized) return false;
            return activeBounds.Contains(position);
        }

        public void UpdateAreaState(bool active)
        {
            if (isActive == active) return;
            
            isActive = active;
            EventManager.Instance.TriggerEvent(new ShapeActionEvent
            {
                Type = ShapeType.None,
                ActionType = ShapeActionType.None,
                Position = Vector3.zero,
                ActionData = new
                {
                    CentralAreaSize = config.CentralAreaSize,
                    ActiveAreaSize = config.ActiveAreaSize,
                    IsActive = isActive
                }
            });
        }

        public Bounds GetCentralBounds() => centralBounds;
        public Bounds GetActiveBounds() => activeBounds;

        public void Dispose()
        {
            isInitialized = false;
            isActive = false;
        }
    }
} 