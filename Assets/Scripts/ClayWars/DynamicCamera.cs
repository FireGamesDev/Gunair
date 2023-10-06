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
    [SerializeField] private float targetTransitionDuration = 1.5f;

    private bool transitioningToMiddle = false;
    private bool isTransitioning = true;
    private bool transitioningToTargetFinished = false;

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (currentTarget != null)
        {
            if (!isTransitioning && transitioningToTargetFinished)
            {
                TrackObject();
            }
        }
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            if (!transitioningToMiddle)
            {
                TransitionToMiddle();
                transitioningToMiddle = true;
                isTransitioning = false;
                transitioningToTargetFinished = false;
            }
        }
        else
        {
            if (!isTransitioning)
            {
                isTransitioning = true;
                transitioningToTargetFinished = false;
                StartCoroutine(TransitionToTarget());
            }
            else 

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

    private IEnumerator TransitionToTarget()
    {
        float elapsedTime = 0f;

        while (elapsedTime < targetTransitionDuration)
        {
            cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, Quaternion.LookRotation(currentTarget.position - cam.transform.position), rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isTransitioning = false;
        transitioningToTargetFinished = true;
    }

    private void TrackObject()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.position - cam.transform.position);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
