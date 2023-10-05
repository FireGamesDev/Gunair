using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyClaySideMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    private Vector3 direction;

    private bool isActivated = false;

    private IEnumerator Start()
    {
        rb = GetComponent<Rigidbody>();

        yield return new WaitForSeconds(2f);

        isActivated = true;
    }

    public void SetDirection(bool isRight)
    {
        if (isRight)
        {
            direction = Vector3.left;
        }
        else direction = Vector3.right;
    }

    private void FixedUpdate()
    {
        if (!isActivated) return;

        Vector3 currentVelocity = rb.velocity;

        Vector3 movement = direction * moveSpeed;
        Vector3 newVelocity = new Vector3(movement.x, currentVelocity.y, currentVelocity.z);

        rb.velocity = newVelocity;
    }
}
