using UnityEngine;
using Photon.Pun;

public class GunRotation : MonoBehaviour
{
    private Camera cam;
    public Transform reticle;
    public float rotationSpeed = 5f;

    private Vector3 prevHitPos;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (MultiplayerGameManager.GetLocalPlayerIndex() != ClayWarsRoundManager.Instance.currentPlayerIndexInRound)
            {
                return;
            }
        }

        Vector3 targetPosition = prevHitPos;

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 10000f))
        {
            targetPosition = hit.point;
        }

        prevHitPos = targetPosition;

        // Calculate the direction from the gun to the target position
        Vector3 direction = targetPosition - transform.position;

        RotateGunTowardsDirection(direction);
    }

    private void RotateGunTowardsDirection(Vector3 direction)
    {
        // Calculate the rotation angle around the gun's y-axis
        float angle = Mathf.Atan2(-direction.z, direction.x) * Mathf.Rad2Deg;

        // Apply additional rotation to the calculated angle
        angle += 180f;

        // Calculate the rotation angle around the gun's x-axis
        float verticalAngle = Mathf.Atan2(direction.y, Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z)) * Mathf.Rad2Deg;

        // Create the target rotation based on the modified angles
        Quaternion targetRotation = Quaternion.Euler(0f, angle, -verticalAngle);

        // Smoothly rotate the gun towards the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}