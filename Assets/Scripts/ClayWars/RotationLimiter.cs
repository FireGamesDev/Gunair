using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotationLimiter : MonoBehaviour
{
    public GameObject followRotation;

    private void Update()
    {
        Vector3 currentRotation = new Vector3(0, followRotation.transform.rotation.y, 0);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }
}
