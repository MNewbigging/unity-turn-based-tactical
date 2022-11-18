using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
  [SerializeField] private TextMeshPro textMeshPro;
  private object gridObject;

  public virtual void SetGridObject(object gridObject)
  {
    // This debug object is for this grid object
    this.gridObject = gridObject;
  }

  protected virtual void Update()
  {
    textMeshPro.text = this.gridObject.ToString();
  }
}
