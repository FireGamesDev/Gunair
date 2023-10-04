using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDiscIfStopped : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    private void Update()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.3f, targetLayer))
        {
            Destroy(gameObject);
        }
    }
}
