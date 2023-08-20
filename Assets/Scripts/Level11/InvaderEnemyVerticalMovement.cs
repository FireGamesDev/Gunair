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

        // Move horizontally
        if (!descending)
        {

            if (Mathf.Abs(transform.position.x - startingPosition.x) <= horizontalMoveDistance)
            {
                transform.position = newPosition;
            }
            else
            {
                descending = true;
            }
        }
        else
        {
            // Descend and change direction
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;

            if (Mathf.Abs(transform.position.y - startingPosition.y) >= descentDistance)
            {
                // Update startingPosition after each descent
                startingPosition = transform.position;
                descending = false;
                moveDirection *= -1;

                // Reset the position to ensure it moves to the left
                newPosition = transform.position + Vector3.right * moveSpeed * Time.deltaTime * moveDirection;
                transform.position = newPosition;
            }
        }
    }
}
