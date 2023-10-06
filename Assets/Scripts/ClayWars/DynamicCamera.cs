using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class DynamicCamera : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject middle;

    public static DynamicCamera Instance;
    public Transform currentTarget { get; set; }

    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float transitionDuration = 2.0f;

    private bool transitioningToMiddle = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            if (!transitioningToMiddle)
            {
                TransitionToMiddle();
                transitioningToMiddle = true;
            }
        }
        else
        {
            cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, Quaternion.LookRotation(currentTarget.position - cam.transform.position), rotationSpeed * Time.deltaTime);

            transitioningToMiddle = false;
        }
    }

    private void TransitionToMiddle()
    {
        MoveCameraToTarget(middle.transform, transitionDuration);
    }

    private void MoveCameraToTarget(Transform target, float duration)
    {
        if (cam != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - cam.transform.position);
            cam.transform.DORotateQuaternion(targetRotation, duration);
        }
    }
}
