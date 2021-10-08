using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GramGames.CraftingSystem.DataContainers;

public class GridHandler : MonoBehaviour
{
	#region fields

	//temp until refactor generation code
	[SerializeField] List<Transform> _rows = new List<Transform>();

	//caching
	[SerializeField] protected List<GridCell> _emptyCells;
	[SerializeField] protected List<GridCell> _fullCells;
	public int numberOfRows;
	public int numberOfColumns;
	public int food;
	public float horizontalSpacing;
	public float verticalSpacing;
	public List<GridCell> GetFullCells => _fullCells;
	public List<GridCell> GetEmptyCells => _emptyCells;

	[Range(0.0f, 1.0f)]
	public float itemDenisty;

	public GameObject cellPrefab;
	public List<NodeContainer> recipeRange;
    #endregion

    #region initialization
    public void Initialize()
	{
		_rows = new List<Transform>(numberOfRows);
		ClearExistingCells();
		DestroyExistingChildren();
		BuildGrid();
        ConnectCells();
    }

	private void DestroyExistingChildren()
    {
		int childLength = transform.childCount;

		for(int i = childLength-1; i >= 0; i--)
		{
			var child = transform.GetChild(i);
			DestroyImmediate(child.gameObject);
		}
    }

	private void BuildGrid()
	{
		Vector3 cellSize = cellPrefab.GetComponentsInChildren<SpriteRenderer>()[0].bounds.size;
		float firstRowPosY = (numberOfRows * (cellSize.y + verticalSpacing)) / 2;
		float firstCellPosX = -(numberOfColumns * (cellSize.x + horizontalSpacing)) / 2;

		for (int rowIndex = 0; rowIndex < numberOfRows; rowIndex++)
		{
			GameObject row = new GameObject("row" + rowIndex);
			float rowOffsetY = (cellSize.y + verticalSpacing) * rowIndex;
			Vector3 rowPos = new Vector3(0, firstRowPosY - rowOffsetY + transform.position.y, 0);

			row.transform.position = rowPos;
			row.transform.SetParent(transform);
			_rows.Add(row.transform);

			for (int cellIndex = 0; cellIndex < numberOfColumns; cellIndex++)
            {
				float cellOffsetX = (cellSize.x + horizontalSpacing) * cellIndex;
				Vector3 cellPos = new Vector3(firstCellPosX + cellOffsetX + transform.position.x, rowPos.y, 0);
				GameObject cell = Instantiate(cellPrefab, cellPos, row.transform.rotation, row.transform);
				cell.name = "row" + rowIndex + "_cell" + cellIndex;
				_emptyCells.Add(cell.GetComponent<GridCell>());
            }
		}
	}

	private void CreateGridRow(Vector3 gridPos, int rowIndex, Vector3 cellSize)
	{
	}
    private void ConnectCells()
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            //each row
            List<GridCell> currentRow = _rows[i].GetComponentsInChildren<GridCell>().ToList();
            List<GridCell> upperRow = i > 0 ? _rows[i - 1].GetComponentsInChildren<GridCell>().ToList() : null;
            List<GridCell> lowerRow = i + 1 < _rows.Count ? _rows[i + 1].GetComponentsInChildren<GridCell>().ToList() : null;

            for (int j = 0; j < currentRow.Count; j++)
            {
                //each cell
                currentRow[j].SetNeighbor(upperRow?[j], MoveDirection.Up);
                currentRow[j].SetNeighbor(lowerRow?[j], MoveDirection.Down);

                var leftN = j > 0 ? currentRow[j - 1] : null;
                currentRow[j].SetNeighbor(leftN, MoveDirection.Left);

                var rightN = j < currentRow.Count - 1 ? currentRow?[j + 1] : null;
                currentRow[j].SetNeighbor(rightN, MoveDirection.Right);
                currentRow[j].SetHandler(this);

                //cache the cell as empty
                _emptyCells.Add(currentRow[j]);
            }

        }
    }

    private void ClearExistingCells()
	{
		_emptyCells = new List<GridCell>();
		_fullCells = new List<GridCell>();
	}

	private void Awake()
	{
		foreach (var cell in _emptyCells)
		{
			cell.SetHandler(this);
		}
	}

	#endregion

	#region helpers

	public void AddMergeableItemToEmpty(MergableItem item)
	{
		var cell = _emptyCells.FirstOrDefault();
		if (cell != null)
		{
			item.AssignToCell(cell);
		}
	}

	public void ClearCell(GridCell cell)
	{
		if (_fullCells.Contains(cell))
		{
			_fullCells.Remove(cell);
			cell.ClearItem();
		}
		
		if (!_emptyCells.Contains(cell))
			_emptyCells.Add(cell);
	}


	public void ClearFullCells()
    {
		for (int i = _fullCells.Count - 1; i >= 0; i--)
			ClearCell(_fullCells[i]);
	}

	public void SetCellState(GridCell cell, bool empty)
	{
		if (cell == null) return;
		if (empty)
		{
			if (_fullCells.Contains(cell))
			{
				_fullCells.Remove(cell);
			}

			if (_emptyCells.Contains(cell) == false)
			{
				_emptyCells.Add(cell);
			}
		}
		else
		{
			if (_emptyCells.Contains(cell))
			{
				_emptyCells.Remove(cell);
			}

			if (_fullCells.Contains(cell) == false)
			{
				_fullCells.Add(cell);
			}
		}
	}

	#endregion
}
