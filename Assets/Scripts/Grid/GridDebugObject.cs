using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
  [SerializeField] private TextMeshPro textMeshPro;
  private GridObject gridObject;

  public void SetGridObject(GridObject gridObject)
  {
    // This debug object is for this grid object
    this.gridObject = gridObject;
  }

  private void Update()
  {
    textMeshPro.text = this.gridObject.ToString();
  }
}
