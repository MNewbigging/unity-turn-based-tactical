using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
  public event EventHandler OnShoot;

  private enum State
  {
    Aiming,
    Shooting,
    Cooloff
  }

  private State state;
  private int maxShootDistance = 7;
  private float stateTimer;
  private Unit targetUnit;
  private bool canShootBullet;

  public override string GetActionName()
  {
    return "Shoot";
  }

  private void Update()
  {
    if (!isActive)
    {
      return;
    }

    switch (state)
    {
      case State.Aiming:
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, rotateSpeed * Time.deltaTime);
        break;
      case State.Shooting:
        if (canShootBullet)
        {
          Shoot();
          canShootBullet = false;
        }
        break;
    }

    stateTimer -= Time.deltaTime;
    if (stateTimer <= 0f)
    {
      NextState();
    }

  }

  private void NextState()
  {
    switch (state)
    {
      case State.Aiming:
        state = State.Shooting;
        float shootingStateTime = 0.1f;
        stateTimer = shootingStateTime;
        break;
      case State.Shooting:
        state = State.Cooloff;
        float cooloffStatetime = 0.5f;
        stateTimer = cooloffStatetime;
        break;
      case State.Cooloff:
        ActionComplete();
        break;
    }
  }

  private void Shoot()
  {
    targetUnit.Damage();
    OnShoot?.Invoke(this, EventArgs.Empty);
  }

  public override List<GridPosition> GetValidActionGridPositionList()
  {
    List<GridPosition> validGridPositionList = new List<GridPosition>();

    GridPosition unitGridPosition = unit.GetGridPosition();

    for (int x = -maxShootDistance; x <= maxShootDistance; x++)
    {
      for (int z = -maxShootDistance; z <= maxShootDistance; z++)
      {
        GridPosition offsetGridPosition = new GridPosition(x, z);
        GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

        if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
        {
          continue;
        }

        int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
        if (testDistance > maxShootDistance)
        {
          continue;
        }

        if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
        {
          continue;
        }

        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
        if (targetUnit.IsEnemy() == unit.IsEnemy())
        {
          // Both units on same 'team'
          continue;
        }

        validGridPositionList.Add(testGridPosition);
      }
    }

    return validGridPositionList;
  }

  public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
  {
    ActionStart(onActionComplete);

    targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

    state = State.Aiming;
    float aimingStateTime = 1f;
    stateTimer = aimingStateTime;

    canShootBullet = true;
  }
}