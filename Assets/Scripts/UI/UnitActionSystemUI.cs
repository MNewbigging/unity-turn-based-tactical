using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
  [SerializeField] private Transform actionButtonPrefab;
  [SerializeField] private Transform actionButtonContainerTransform;
  [SerializeField] private TextMeshProUGUI actionPointsText;

  private List<ActionButtonUI> actionButtonUIList;

  private void Awake()
  {
    actionButtonUIList = new List<ActionButtonUI>();
  }

  private void Start()
  {
    UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
    UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
    UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
    Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

    CreateUnitActionButtons();
    UpdateSelectedVisual();
    UpdateActionPoints();
  }

  private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
  {
    CreateUnitActionButtons();
    UpdateSelectedVisual();
    UpdateActionPoints();
  }

  private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
  {
    UpdateSelectedVisual();
  }

  private void UnitActionSystem_OnActionStarted(object sender, EventArgs empty)
  {
    UpdateActionPoints();
  }

  private void CreateUnitActionButtons()
  {
    // Destroy existing buttons first
    foreach (Transform buttonTransform in actionButtonContainerTransform)
    {
      Destroy(buttonTransform.gameObject);
    }
    actionButtonUIList.Clear();

    // Then create buttons for the selected unit's actions
    Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

    foreach (BaseAction baseAction in selectedUnit.GetBaseActions())
    {
      // Create the button
      Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
      // Get the button script
      ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
      actionButtonUI.SetBaseAction(baseAction);

      actionButtonUIList.Add(actionButtonUI);
    }
  }

  private void UpdateSelectedVisual()
  {
    foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
    {
      actionButtonUI.UpdateSelectedVisual();
    }
  }

  private void UpdateActionPoints()
  {
    Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
    actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
  }

  private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
  {
    UpdateActionPoints();
  }
}
