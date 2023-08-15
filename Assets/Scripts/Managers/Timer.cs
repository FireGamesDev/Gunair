using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float timeRemaining = 30f;
    public float time { get; set; } = 0f;
    [SerializeField] private TMPro.TMP_Text timerText;

    private void Start()
    {
        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        while (timeRemaining > 0f)
        {
            timerText.text = timeRemaining.ToString("F0");
            yield return null; // Wait for the next frame
            timeRemaining -= Time.deltaTime;
            time += Time.deltaTime;
        }

        gameManager.End();
    }
}
