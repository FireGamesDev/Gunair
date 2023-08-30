using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLimiter : MonoBehaviour
{
    public Vector3 minRotationAngles;
    public Vector3 maxRotationAngles;
    public GameObject followRotation;

    private void Update()
    {
        Vector3 currentRotation = new Vector3(0, followRotation.transform.rotation.y, 0);
        Vector3 clampedRotation = new Vector3(
            Mathf.Clamp(currentRotation.x, minRotationAngles.x, maxRotationAngles.x),
            Mathf.Clamp(currentRotation.y, minRotationAngles.y, maxRotationAngles.y),
            Mathf.Clamp(currentRotation.z, minRotationAngles.z, maxRotationAngles.z)
        );

        gameObject.transform.localRotation = Quaternion.Euler(currentRotation);
    }
}

