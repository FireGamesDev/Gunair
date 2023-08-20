using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderEnemyHorizontalMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public bool moveLeft = true; // Set to false if you want to move right

    private void Update()
    {
        Vector3 movementDirection = moveLeft ? Vector3.left : Vector3.right;
        transform.position += movementDirection * moveSpeed * Time.deltaTime;
    }
}
