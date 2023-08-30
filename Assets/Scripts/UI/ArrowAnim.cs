using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowAnim : MonoBehaviour
{
    public Vector2 movement;

    public float yoyoDuration = 0.2f;
    public float delayBeforeDisable = 2.0f;

    private Vector2 startPos = Vector2.zero;
    private Tweener yoyoTweener;

    private void OnEnable()
    {
        if (startPos == Vector2.zero)
        {
            startPos = GetComponent<RectTransform>().anchoredPosition;
        }

        yoyoTweener = GetComponent<RectTransform>().DOAnchorPos(movement, yoyoDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        DOVirtual.DelayedCall(delayBeforeDisable, () =>
        {
            yoyoTweener.Kill(); // Stop the yoyo movement
            GetComponent<RectTransform>().anchoredPosition = startPos; // Reset position
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        if (yoyoTweener != null && yoyoTweener.IsActive())
        {
            yoyoTweener.Kill(); // Stop the yoyo movement if the GameObject is disabled prematurely
        }

        GetComponent<RectTransform>().anchoredPosition = startPos;
    }
}
