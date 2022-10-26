using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
  public static LevelGrid Instance { get; private set; }

  [SerializeField] private Transform gridDebugObjectPrefab;
  private GridSystem gridSystem;

  private void Awake()
  {
    // Singleton setup
    if (Instance != null)
    {
      Debug.LogError($"Found more than one LevelGrid: {transform} - {Instance}");
      Destroy(gameObject);
      return;
    }
    Instance = this;

    // Create the grid system
    gridSystem = new GridSystem(10, 10, 2f);
    gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
  }

  public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
  {
    gridSystem.GetGridObject(gridPosition).AddUnit(unit);
  }

  public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
  {
    return gridSystem.GetGridObject(gridPosition).GetUnitList();
  }

  public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
  {
    gridSystem.GetGridObject(gridPosition).RemoveUnit(unit);
  }

  public void UnitMovedGridPosition(Unit unit, GridPosition from, GridPosition to)
  {
    RemoveUnitAtGridPosition(from, unit);
    AddUnitAtGridPosition(to, unit);
  }

  public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
  {
    return gridSystem.GetGridObject(gridPosition).HasAnyUnit();
  }

  public Unit GetUnitAtGridPosition(GridPosition gridPosition)
  {
    GridObject gridObject = gridSystem.GetGridObject(gridPosition);
    return gridObject.GetUnit();
  }

  public int GetWidth() => gridSystem.GetWidth();
  public int GetHeight() => gridSystem.GetHeight();
  public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
  public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
  public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
}
