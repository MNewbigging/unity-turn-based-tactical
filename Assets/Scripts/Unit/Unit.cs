using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
  private const int ACTION_POINTS_MAX = 2;

  public static event EventHandler OnAnyActionPointsChanged;
  public static event EventHandler OnAnyUnitSpawned;
  public static event EventHandler OnAnyUnitDead;

  [SerializeField] private bool isEnemy;

  private HealthSystem healthSystem;
  private GridPosition gridPosition;
  private BaseAction[] baseActions;
  private int actionPoints = ACTION_POINTS_MAX;

  private void Awake()
  {
    baseActions = GetComponents<BaseAction>();
    healthSystem = GetComponent<HealthSystem>();
  }

  private void Start()
  {
    gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

    TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

    healthSystem.OnDead += HealthSystem_OnDead;

    OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
  }

  private void Update()
  {
    // Keep world pos in sync with grid pos
    GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    if (newGridPosition != gridPosition)
    {
      // Unit has changed grid position
      GridPosition oldGridPos = gridPosition;
      gridPosition = newGridPosition;
      LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPos, newGridPosition);

    }
  }

  public T GetAction<T>() where T : BaseAction
  {
    foreach (BaseAction baseAction in baseActions)
    {
      if (baseAction is T)
      {
        return (T)baseAction;
      }
    }

    return null;
  }

  public BaseAction[] GetBaseActions()
  {
    return baseActions;
  }

  public GridPosition GetGridPosition()
  {
    return gridPosition;
  }

  public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
  {
    return actionPoints >= baseAction.GetActionPointsCost();
  }

  private void SpendActionPoints(int amount)
  {
    this.actionPoints -= amount;
    OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
  }

  public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
  {
    if (CanSpendActionPointsToTakeAction(baseAction))
    {
      SpendActionPoints(baseAction.GetActionPointsCost());
      return true;
    }
    else
    {
      return false;
    }
  }

  public int GetActionPoints()
  {
    return actionPoints;
  }

  private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
  {
    if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
    (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
    {
      actionPoints = ACTION_POINTS_MAX;

      OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }
  }

  public bool IsEnemy()
  {
    return isEnemy;
  }

  public void Damage(int damageAmount)
  {
    healthSystem.TakeDamage(damageAmount);
  }

  public Vector3 GetWorldPosition()
  {
    return transform.position;
  }

  private void HealthSystem_OnDead(object sender, EventArgs e)
  {
    LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
    Destroy(gameObject);

    OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
  }

  public float GetHealthNormalised()
  {
    return healthSystem.GetHealthNormalised();
  }
}
