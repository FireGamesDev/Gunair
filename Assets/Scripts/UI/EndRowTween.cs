using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRowTween : MonoBehaviour
{
    [SerializeField] private float delay;

    private void OnEnable()
    {
        StartCoroutine(Main());
    }

    private IEnumerator Main()
    {
        transform.localScale = Vector3.zero;
        Vector3 targetScale = new Vector3(1f, 1f, 1f);

        yield return new WaitForSeconds(delay);

        transform.DOScale(targetScale, 1f)
            .SetLoops(1)
            .SetEase(Ease.InOutQuad); // Easing function for smooth motion
    }
}
