using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveAction : BaseAction
{
  [SerializeField] private Animator unitAnimator;
  [SerializeField] private int maxMoveDistance = 4;
  private Vector3 targetPosition;
  private int moveSpeed = 4;

  protected override void Awake()
  {
    base.Awake();

    targetPosition = transform.position;
  }


  private void Update()
  {
    if (!isActive)
    {
      return;
    }

    // There's a margin of error for reaching a target position
    float stoppingDistance = 0.05f;
    if (Vector3.Distance(transform.position, this.targetPosition) > stoppingDistance)
    {
      MoveToTarget(this.targetPosition);
    }
    else
    {
      // Not moving; play idle animation
      unitAnimator.SetBool("IsWalking", false);

      ActionComplete();
    }
  }

  private void MoveToTarget(Vector3 targetPosition)
  {
    // Move towards target
    Vector3 moveDirection = (targetPosition - transform.position).normalized;
    transform.position += moveDirection * this.moveSpeed * Time.deltaTime;

    // Rotate to face target smoothly
    float rotateSpeed = 10f;
    transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

    // Play walking animation
    unitAnimator.SetBool("IsWalking", true);
  }

  public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
  {
    ActionStart(onActionComplete);

    this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
  }

  public override List<GridPosition> GetValidActionGridPositionList()
  {
    List<GridPosition> validGridPositionList = new List<GridPosition>();

    GridPosition unitGridPosition = unit.GetGridPosition();

    for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
    {
      for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
      {
        GridPosition offsetGridPosition = new GridPosition(x, z);
        GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

        if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
        {
          continue;
        }

        if (unitGridPosition == testGridPosition)
        {
          continue;
        }

        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
        {
          continue;
        }

        validGridPositionList.Add(testGridPosition);
      }
    }

    return validGridPositionList;
  }

  public override string GetActionName()
  {
    return "Move";
  }


}
