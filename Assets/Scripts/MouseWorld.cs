using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake() 
    {
        instance = this;
    }

    // If you want to see the mouse position, enable the child MouseVisial object 
    // private void Update()
    // {
    //     // Move to position hit
    //     transform.position = MouseWorld.GetPosition();
    // }

    public static Vector3 GetPosition() 
    {
         // Get ray to cast using screen space coords along camera projection
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Cast the ray against all colliders and store hit data in new var
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);

        // Will be 0, 0, 0 if nothing was hit
        return raycastHit.point;    
    }
}
