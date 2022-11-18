using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
  [SerializeField] private GameObject actionCameraGameObject;


  private void Start()
  {
    BaseAction.OnAnyActionStart += BaseAction_OnAnyActionStart;
    BaseAction.OnAnyActionEnd += BaseAction_OnAnyActionEnd;

    HideActionCamera();
  }

  private void BaseAction_OnAnyActionStart(object sender, EventArgs e)
  {
    switch (sender)
    {
      case ShootAction shootAction:
        Unit shooterUnit = shootAction.GetUnit();
        Unit targetUnit = shootAction.GetTargetUnit();

        Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
        Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

        float shoulderOffsetAmount = 0.5f;
        Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

        Vector3 actionCameraPosition =
          shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);

        actionCameraGameObject.transform.position = actionCameraPosition;
        actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);

        ShowActionCamera();
        break;
    }
  }

  private void BaseAction_OnAnyActionEnd(object sender, EventArgs e)
  {
    switch (sender)
    {
      case ShootAction shootAction:
        HideActionCamera();
        break;
    }
  }

  private void ShowActionCamera()
  {
    actionCameraGameObject.SetActive(true);
  }

  private void HideActionCamera()
  {
    actionCameraGameObject.SetActive(false);
  }
}
