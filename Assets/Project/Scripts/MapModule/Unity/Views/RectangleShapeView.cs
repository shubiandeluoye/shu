using UnityEngine;
using MapModule.Core.Data;
using System.Collections.Generic;

namespace MapModule.Unity.Views
{
    public class RectangleShapeView : BaseShapeView
    {
        [SerializeField] private GameObject gridCellPrefab;
        [SerializeField] private Transform gridContainer;

        private List<SpriteRenderer> gridCells = new();
        private Vector2Int currentGridSize;

        public override void Initialize(ShapeType type, ShapeTypeSO config)
        {
            base.Initialize(type, config);
            CreateGrid(new Vector2Int(5, 8)); // 默认大小，后续通过状态更新
        }

        public override void UpdateState(ShapeState state)
        {
            base.UpdateState(state);

            if (state.GridState != null)
            {
                UpdateGridVisual(state.GridState);
            }
        }

        private void CreateGrid(Vector2Int size)
        {
            ClearGrid();
            currentGridSize = size;

            float cellWidth = typeConfig.ToShapeTypeConfig().Size.X / size.x;
            float cellHeight = typeConfig.ToShapeTypeConfig().Size.Y / size.y;

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var cell = Instantiate(gridCellPrefab, gridContainer).GetComponent<SpriteRenderer>();
                    cell.transform.localPosition = new Vector3(
                        (x - size.x/2f + 0.5f) * cellWidth,
                        (y - size.y/2f + 0.5f) * cellHeight,
                        0
                    );
                    cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
                    gridCells.Add(cell);
                }
            }
        }

        private void UpdateGridVisual(bool[,] gridState)
        {
            int index = 0;
            for (int y = 0; y < currentGridSize.y; y++)
            {
                for (int x = 0; x < currentGridSize.x; x++)
                {
                    if (index < gridCells.Count)
                    {
                        gridCells[index].gameObject.SetActive(gridState[x, y]);
                    }
                    index++;
                }
            }
        }

        private void ClearGrid()
        {
            foreach (var cell in gridCells)
            {
                if (cell != null)
                    Destroy(cell.gameObject);
            }
            gridCells.Clear();
        }

        private void OnDestroy()
        {
            ClearGrid();
        }
    }
} 