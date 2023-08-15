using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movementDuration = 2f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float startTime;

    private bool canStart = false;

    private void Start()
    {
        // Store the initial position of the camera
        initialPosition = transform.position;

        StartCoroutine(CameraRoutine());
    }

    private void Update()
    {
        if (!canStart) return;

        // Calculate the time elapsed since the movement started
        float timeElapsed = Time.time - startTime;

        // Calculate the interpolation factor between 0 and 1 based on the elapsed time and duration
        float t = Mathf.Clamp01(timeElapsed / movementDuration);

        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

        // Check if the movement is complete
        if (t >= 1f)
        {
            // Movement complete
            Debug.Log("Camera movement complete");

            // Optional: Perform any additional actions after the movement is complete

            // Disable or remove this script if needed
            enabled = false;
        }
    }

    private void StartCameraMovement(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        // Start the camera movement by setting the start time
        startTime = Time.time;
    }

    private IEnumerator CameraRoutine()
    {
        yield return new WaitForSeconds(10);
        StartCameraMovement(new Vector3(0, 0, -10));
        canStart = true;
    }
}