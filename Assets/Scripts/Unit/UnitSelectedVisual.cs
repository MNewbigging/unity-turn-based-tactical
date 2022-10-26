using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
  [SerializeField] private Unit unit;

  private MeshRenderer meshRenderer;

  private void Awake()
  {
    meshRenderer = GetComponent<MeshRenderer>();
  }

  private void Start()
  {
    UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectUnit;

    ToggleVisual();
  }

  private void OnSelectUnit(object sender, EventArgs empty)
  {
    ToggleVisual();
  }

  private void ToggleVisual()
  {
    // Show or hide this object based on if unit selected is this one 
    meshRenderer.enabled = UnitActionSystem.Instance.GetSelectedUnit() == unit;
  }
}
