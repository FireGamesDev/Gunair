using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
