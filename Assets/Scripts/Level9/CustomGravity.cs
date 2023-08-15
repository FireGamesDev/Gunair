using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float gravityScale = 0.1f;
    [SerializeField] private float antiGravityChance = 0f;

    private void FixedUpdate()
    {
        if (Random.value < antiGravityChance)
        {
            Vector3 gravity = -gravityScale * Physics.gravity;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
        else
        {
            Vector3 gravity = gravityScale * Physics.gravity;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
        
    }
}
