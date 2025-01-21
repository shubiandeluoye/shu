using UnityEngine;
using System;
using MapModule.Shapes;
using Core.EventSystem;
using MapModule.Utils;


namespace MapModule.Shapes
{
    /// <summary>
    /// 矩形
    /// 功能：
    /// 1. 维护5*8网格
    /// 2. 子弹击中使对应格子消失
    /// 3. 30秒后消失
    /// </summary>
    public class RectShape : BaseShape
    {
        private float remainingTime;
        private bool[,] gridState;
        private Vector2 gridCellSize;

        public override void Initialize(ShapeConfig config)
        {
            base.Initialize(config);
            
            // 初始化网格
            gridState = new bool[config.GridSize.x, config.GridSize.y];
            for (int x = 0; x < config.GridSize.x; x++)
                for (int y = 0; y < config.GridSize.y; y++)
                    gridState[x, y] = true;

            // 计算每个格子的大小
            gridCellSize = new Vector2(
                config.Size.x / config.GridSize.x,
                config.Size.y / config.GridSize.y
            );

            remainingTime = 30f; // 30秒持续时间
        }

        public override void HandleSkillHit(int skillId, Vector3 hitPoint)
        {
            if (!ValidateHitPoint(hitPoint)) return;
            
            Vector2 localHitPoint = transform.InverseTransformPoint(hitPoint);
            if (ShapeUtils.CheckCollision(localHitPoint, ShapeType.Rectangle, Vector2.zero, config.Size))
            {
                // 计算击中的网格位置
                int gridX = Mathf.FloorToInt((localHitPoint.x + config.Size.x / 2) / gridCellSize.x);
                int gridY = Mathf.FloorToInt((localHitPoint.y + config.Size.y / 2) / gridCellSize.y);

                // 确保在网格范围内
                if (IsValidGridPosition(gridX, gridY) && gridState[gridX, gridY])
                {
                    DisableGridCell(gridX, gridY);
                }
            }
        }

        private bool IsValidGridPosition(int x, int y)
        {
            return x >= 0 && x < config.GridSize.x && y >= 0 && y < config.GridSize.y;
        }

        private void DisableGridCell(int x, int y)
        {
            gridState[x, y] = false;
            
            // 更新网格视觉表现
            UpdateGridVisual(x, y);
        }

        private void UpdateGridVisual(int x, int y)
        {
            // 计算实际位置
            Vector2 cellPosition = new Vector2(
                (x - config.GridSize.x / 2f + 0.5f) * gridCellSize.x,
                (y - config.GridSize.y / 2f + 0.5f) * gridCellSize.y
            );

            // 触发消失效果事件
            EventManager.Instance.TriggerEvent(new GridCellDisableEvent
            {
                Position = transform.TransformPoint(cellPosition),
                Size = gridCellSize
            });
        }

        private void TriggerGridCellDisable()
        {
            if (!IsActive()) return;
            
            EventManager.Instance.TriggerEvent(new ShapeActionEvent
            {
                Type = GetShapeType(),
                ActionType = ShapeActionType.GridDestroy,
                Position = transform.position,
                ActionData = new { Size = transform.localScale }
            });
        }

        public override ShapeState GetState()
        {
            return new ShapeState
            {
                Type = ShapeType.Rectangle,
                Position = transform.position,
                IsActive = IsActive(),
                GridState = gridState
            };
        }

        protected override void ValidateConfig(ShapeConfig config)
        {
            if (config.GridSize.x <= 0 || config.GridSize.y <= 0)
            {
                throw new System.ArgumentException("Grid size must be positive");
            }
            if (config.Size.x <= 0 || config.Size.y <= 0)
            {
                throw new System.ArgumentException("Shape size must be positive");
            }
        }

        protected override void OnShapeUpdate()
        {
            if (!isActive) return;
            
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                OnDisappear();
            }
        }
    }
} 