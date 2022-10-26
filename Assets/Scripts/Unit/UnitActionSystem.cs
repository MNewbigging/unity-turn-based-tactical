using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
  public static UnitActionSystem Instance { get; private set; }
  public event EventHandler OnSelectedUnitChanged;
  public event EventHandler OnSelectedActionChanged;
  public event EventHandler<bool> OnBusyChanged;
  public event EventHandler OnActionStarted;

  [SerializeField] private Unit selectedUnit;
  [SerializeField] private LayerMask unitLayerMask;

  private BaseAction selectedAction;
  private bool isBusy;

  public Unit GetSelectedUnit()
  {
    return selectedUnit;
  }

  private void Awake()
  {
    if (Instance != null)
    {
      Debug.LogError("Found more than one UnitActionSystem: " + transform + " - " + Instance);
      Destroy(gameObject);
      return;
    }

    Instance = this;
  }

  private void Start()
  {
    SelectUnit(selectedUnit);
  }

  private void Update()
  {
    if (isBusy)
    {
      return;
    }

    if (!TurnSystem.Instance.IsPlayerTurn())
    {
      return;
    }

    // Stop if mouse is over a UI element
    if (EventSystem.current.IsPointerOverGameObject())
    {
      return;
    }

    // If a unit was selected, no further action can be taken
    if (TrySelectUnit())
    {
      return;
    }

    // Otherwise, handle the selected action on input
    HandleSelectedAction();
  }

  private bool TrySelectUnit()
  {
    if (Input.GetMouseButtonDown(0))
    {
      // Raycast into scene against units
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      bool didHit = Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask);

      // If ray hit and a Unit component was found
      if (didHit && raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
      {
        // Cannot select enemies
        if (unit.IsEnemy())
        {
          return false;
        }

        // Select the unit - if not already selected
        if (unit != selectedUnit)
        {
          SelectUnit(unit);
          return true;
        }
      }
    }


    return false;
  }


  private void HandleSelectedAction()
  {
    if (Input.GetMouseButtonDown(0))
    {
      // Get grid position clicked
      GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

      // Continue only if clicked grid cell is valid for current action
      if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
      {
        return;
      }

      // Continue only if unit can afford to take that action
      if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
      {
        return;
      }

      // Take the action
      SetBusy();
      selectedAction.TakeAction(mouseGridPosition, ClearBusy);
      OnActionStarted?.Invoke(this, EventArgs.Empty);
    }
  }

  private void SetBusy()
  {
    isBusy = true;
    OnBusyChanged(this, isBusy);
  }

  private void ClearBusy()
  {
    isBusy = false;
    OnBusyChanged(this, isBusy);
  }

  private void SelectUnit(Unit unit)
  {
    selectedUnit = unit;
    SetSelectedAction(unit.GetMoveAction());

    OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
  }

  public void SetSelectedAction(BaseAction baseAction)
  {
    selectedAction = baseAction;

    OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
  }

  public BaseAction GetSelectedAction()
  {
    return selectedAction;
  }
}
