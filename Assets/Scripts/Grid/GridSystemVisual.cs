using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridSystemVisual : MonoBehaviour
{
  public static GridSystemVisual Instance { get; private set; }

  [Serializable]
  public struct GridVisualTypeMaterial
  {
    public GridVisualType gridVisualType;
    public Material material;
  }

  public enum GridVisualType
  {
    White,
    Blue,
    Red,
    RedSoft,
    Yellow
  }

  [SerializeField] private Transform gridSystemVisualSinglePrefab;
  [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

  private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

  private void Awake()
  {
    if (Instance != null)
    {
      Debug.Log("Found more than one GridSystemVisual: " + transform + " - " + Instance);
      Destroy(gameObject);
      return;
    }
    Instance = this;
  }

  private void Start()
  {
    gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];


    for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
    {
      for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
      {
        GridPosition gridPosition = new GridPosition(x, z);
        Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
        gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
      }
    }

    HideAllGridPositions();

    UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
    LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

    UpdateGridVisual();
  }

  private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
  {
    UpdateGridVisual();
  }

  private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
  {
    UpdateGridVisual();
  }

  public void HideAllGridPositions()
  {
    foreach (GridSystemVisualSingle visual in gridSystemVisualSingleArray)
    {
      visual.Hide();
    }
  }

  private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
  {
    List<GridPosition> gridPositionList = new List<GridPosition>();
    for (int x = -range; x <= range; x++)
    {
      for (int z = -range; z <= range; z++)
      {
        GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

        if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
        {
          continue;
        }

        int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
        if (testDistance > range)
        {
          continue;
        }

        gridPositionList.Add(testGridPosition);
      }
    }

    ShowGridPositionList(gridPositionList, gridVisualType);
  }

  public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
  {
    foreach (GridPosition position in gridPositionList)
    {
      gridSystemVisualSingleArray[position.x, position.z].
        Show(GetGridVisualTypeMaterial(gridVisualType));
    }
  }

  public void UpdateGridVisual()
  {
    HideAllGridPositions();


    BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

    GridVisualType gridVisualType = GridVisualType.White;
    switch (selectedAction)
    {
      case MoveAction moveAction:
        gridVisualType = GridVisualType.White;
        break;
      case SpinAction spinAction:
        gridVisualType = GridVisualType.Blue;
        break;
      case ShootAction shootAction:
        gridVisualType = GridVisualType.Red;

        Unit unit = UnitActionSystem.Instance.GetSelectedUnit();
        ShowGridPositionRange(unit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
        break;
    }

    ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
  }

  private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
  {
    foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
    {
      if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
      {
        return gridVisualTypeMaterial.material;
      }
    }

    Debug.LogError("Could not find grid visual type material for " + gridVisualType);
    return null;
  }
}
