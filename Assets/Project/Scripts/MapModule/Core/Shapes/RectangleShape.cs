using MapModule.Core.Data;
using MapModule.Core.Utils;

namespace MapModule.Core.Shapes
{
    public class RectangleShape : BaseShape
    {
        private bool[,] gridState;
        private float remainingTime;

        public RectangleShape(IMapManager manager) : base(manager)
        {
        }

        public override void Initialize(ShapeConfig config)
        {
            base.Initialize(config);
            
            // 初始化网格
            gridState = new bool[(int)config.GridSize.X, (int)config.GridSize.Y];
            for (int x = 0; x < config.GridSize.X; x++)
                for (int y = 0; y < config.GridSize.Y; y++)
                    gridState[x, y] = true;

            remainingTime = config.Duration;
        }

        protected override void OnShapeHit(int skillId, Vector3D hitPoint)
        {
            // 计算本地坐标
            float localX = hitPoint.X - position.X + config.Size.X / 2;
            float localY = hitPoint.Y - position.Y + config.Size.Y / 2;

            // 计算网格位置
            int gridX = (int)(localX / (config.Size.X / config.GridSize.X));
            int gridY = (int)(localY / (config.Size.Y / config.GridSize.Y));

            if (IsValidGridPosition(gridX, gridY) && gridState[gridX, gridY])
            {
                DisableGridCell(gridX, gridY);
            }
        }

        private bool IsValidGridPosition(int x, int y)
        {
            return x >= 0 && x < config.GridSize.X && 
                   y >= 0 && y < config.GridSize.Y;
        }

        private void DisableGridCell(int x, int y)
        {
            gridState[x, y] = false;
            
            // 计算实际位置
            float cellWidth = config.Size.X / config.GridSize.X;
            float cellHeight = config.Size.Y / config.GridSize.Y;
            
            Vector3D cellPosition = new Vector3D(
                position.X - config.Size.X / 2 + (x + 0.5f) * cellWidth,
                position.Y - config.Size.Y / 2 + (y + 0.5f) * cellHeight,
                0
            );

            manager.PublishEvent(MapEvents.GridCellDestroyed, new GridCellDestroyedEvent
            {
                Position = cellPosition,
                Size = new Vector2D(cellWidth, cellHeight)
            });
        }

        public void Update(float deltaTime)
        {
            if (!isActive) return;
            
            remainingTime -= deltaTime;
            if (remainingTime <= 0)
            {
                OnDisappear();
            }
        }

        protected override void ValidateConfig(ShapeConfig config)
        {
            if (config.GridSize.X <= 0 || config.GridSize.Y <= 0)
            {
                throw new System.ArgumentException("Grid size must be positive");
            }
            if (config.Duration <= 0)
            {
                throw new System.ArgumentException("Duration must be positive");
            }
        }

        public override ShapeState GetState()
        {
            return new ShapeState
            {
                Type = ShapeType.Rectangle,
                Position = position,
                IsActive = isActive,
                GridState = gridState,
                RemainingTime = remainingTime
            };
        }
    }

    public struct GridCellDestroyedEvent
    {
        public Vector3D Position { get; set; }
        public Vector2D Size { get; set; }
    }
} 