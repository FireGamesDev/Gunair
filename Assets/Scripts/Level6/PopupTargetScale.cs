using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTargetScale : MonoBehaviour
{
    private Coroutine animCoroutine;
    private Vector3 startScale;
    private float animationTime;

    [SerializeField] private float targetLifeTime = 0f;

    private void Start()
    {
        if (targetLifeTime != 0f)
        {
            StartAnim(targetLifeTime);
        }
    }

    public void StartAnim(float lifetime)
    {
        // Store the start scale of the object
        startScale = transform.localScale;

        // Calculate the time for each animation phase
        animationTime = lifetime / 2f;

        // Stop any existing animation coroutine
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
        }

        // Start the animation coroutine
        animCoroutine = StartCoroutine(AnimationRoutine());
    }

    private IEnumerator AnimationRoutine()
    {
        float elapsedTime = 0f;

        if (targetLifeTime == 0f)
        {
            // Scale up phase 
            while (elapsedTime < animationTime)
            {
                float t = elapsedTime / animationTime;
                transform.localScale = Vector3.Lerp(Vector3.zero, startScale, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        

        // Scale down phase
        elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            float t = elapsedTime / animationTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the scale to zero
        transform.localScale = Vector3.zero;

        if (targetLifeTime != 0f)
        {
            Destroy(gameObject);
        }
    }
}
