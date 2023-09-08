using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingKnobsTween : MonoBehaviour
{
    [SerializeField] private float delay;

    private void OnEnable()
    {
        StartCoroutine(Main());
    }

    private IEnumerator Main()
    {
        Vector3 targetScale = new Vector3(3f, 3f, 1f);

        yield return new WaitForSeconds(delay);

        // Set up the yoyo animation
        transform.DOScale(targetScale, 1f)
            .SetLoops(-1, LoopType.Yoyo) // Infinite loop with yoyo motion
            .SetEase(Ease.InOutQuad); // Easing function for smooth motion
    }
}
