using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Import DoTween namespace

public class CamTargetHelper : MonoBehaviour
{
    private Transform objectToFollow;
    private float lifetime = 20f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public void SetHelper(Transform objectToFollow)
    {
        this.objectToFollow = objectToFollow;
    }

    private void Update()
    {
        if (objectToFollow != null)
        {
            transform.position = objectToFollow.position;
        }
        else
        {
            if (RotationLimiter.Instance != null && RotationLimiter.Instance.middle != null)
            {
                Vector3 targetPosition = RotationLimiter.Instance.middle.transform.position;
                float moveDuration = 2.0f;

                transform.DOMove(targetPosition, moveDuration);
            }
        }
    }
}
