using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceDisk : MonoBehaviour
{
    public Rigidbody diskRigidbody;
    public float bounceForce = 4f;

    private void Update()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.3f))
        {
            Bounce();
        }
    }

    private void Bounce()
    {
        diskRigidbody.AddForce(new Vector3(1, 1, 0) * bounceForce, ForceMode.Impulse);
    }
}