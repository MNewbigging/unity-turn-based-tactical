using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
  public static event EventHandler OnAnyActionStart;
  public static event EventHandler OnAnyActionEnd;

  protected Unit unit;
  protected bool isActive;
  protected Action onActionComplete;

  protected virtual void Awake()
  {
    unit = GetComponent<Unit>();
  }

  public abstract string GetActionName();

  public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

  public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
  {
    List<GridPosition> validPositionList = GetValidActionGridPositionList();
    return validPositionList.Contains(gridPosition);
  }

  public abstract List<GridPosition> GetValidActionGridPositionList();

  public virtual int GetActionPointsCost()
  {
    return 1;
  }

  protected void ActionStart(Action onActionComplete)
  {
    isActive = true;
    this.onActionComplete = onActionComplete;

    OnAnyActionStart?.Invoke(this, EventArgs.Empty);
  }

  protected void ActionComplete()
  {
    isActive = false;
    onActionComplete();

    OnAnyActionEnd?.Invoke(this, EventArgs.Empty);
  }

  public Unit GetUnit()
  {
    return unit;
  }
}
