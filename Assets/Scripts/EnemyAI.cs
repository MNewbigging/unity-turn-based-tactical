using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour
{
  private enum State
  {
    WaitingForEnemyTurn,
    TakingTurn,
    Busy
  }

  private State state;
  private float timer;

  private void Awake()
  {
    state = State.WaitingForEnemyTurn;
  }

  private void Start()
  {
    TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
  }

  private void Update()
  {
    if (TurnSystem.Instance.IsPlayerTurn())
    {
      return;
    }

    switch (state)
    {
      case State.WaitingForEnemyTurn:
        break;
      case State.TakingTurn:
        timer -= Time.deltaTime;
        if (timer < 0)
        {
          if (TryTakeEnemyAIAction(SetStateTakingTurn))
          {
            state = State.Busy;
          }
          else
          {
            TurnSystem.Instance.NextTurn();
          }
        }
        break;
      case State.Busy:
        break;
    }
  }

  private void SetStateTakingTurn()
  {
    timer = 0.5f;
    state = State.TakingTurn;
  }

  private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
  {
    if (!TurnSystem.Instance.IsPlayerTurn())
    {
      state = State.TakingTurn;
      timer = 2f;
    }
  }

  private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
  {
    foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
    {
      if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
      {
        return true;
      }
    }

    return false;
  }

  private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
  {
    SpinAction spinAction = enemyUnit.GetSpinAction();

    // Get grid position clicked
    GridPosition actionGridPosition = enemyUnit.GetGridPosition();

    // Continue only if clicked grid cell is valid for current action
    if (!spinAction.IsValidActionGridPosition(actionGridPosition))
    {
      return false;
    }

    // Continue only if unit can afford to take that action
    if (!enemyUnit.TrySpendActionPointsToTakeAction(spinAction))
    {
      return false;
    }

    // Take the action
    spinAction.TakeAction(actionGridPosition, onEnemyAIActionComplete);

    return true;
  }
}
