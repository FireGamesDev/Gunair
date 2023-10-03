using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDiscIfStopped : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    private void Update()
    {
        // Define the raycast origin and direction (e.g., from the camera's position forward)
        Ray ray = new Ray(transform.position, transform.forward);

        // Declare a RaycastHit variable to store information about the hit
        RaycastHit hit;

        // Perform the raycast and check if it hits an object on the target layer
        if (Physics.Raycast(ray, out hit, 1f, targetLayer))
        {
            // Check if the hit object is on the "Default" layer (layer index 0)
            if (hit.collider.gameObject.layer == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
