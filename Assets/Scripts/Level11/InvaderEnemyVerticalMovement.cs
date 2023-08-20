using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderEnemyVerticalMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float horizontalMoveDistance = 2f;
    public float descentDistance = 1f;

    private Vector3 startingPosition;
    private int moveDirection = 1; // 1 for right, -1 for left
    private bool descending = false;

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        Vector3 newPosition = transform.position + Vector3.right * moveSpeed * Time.deltaTime * moveDirection;

        // Move horizontally within the defined distance
        if (!descending && Mathf.Abs(newPosition.x - startingPosition.x) <= horizontalMoveDistance)
        {
            transform.position = newPosition;
        }
        else
        {
            // Descend and change direction
            descending = true;
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;

            if (Mathf.Abs(transform.position.y - startingPosition.y) >= descentDistance)
            {
                descending = false;
                moveDirection *= -1;
            }
        }
    }
}
