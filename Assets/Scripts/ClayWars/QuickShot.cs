using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuickShot : MonoBehaviour
{
    public static QuickShot Instance;

    [SerializeField] private TMP_Text quickShotText;
    [SerializeField] private GameObject quickShotGo;
    [SerializeField] private Image backgroundImage;

    [Header("Scoring")]
    [SerializeField] private float maxScoreTime = 4.0f;
    [SerializeField] private int maxScore = 10;

    private float startTime;
    private float elapsedTime;
    private bool isTiming;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isTiming)
        {
            elapsedTime = Time.time - startTime;
            UpdateTimerDisplay(elapsedTime);
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isTiming = true;
        quickShotGo.SetActive(true);
    }

    public void StopTimer()
    {
        isTiming = false;

        // Play fade animations and then disable quickShotGo
        backgroundImage.DOFade(0f, 2f);
        quickShotText.DOFade(0f, 2f).OnComplete(() =>
        {
            quickShotGo.SetActive(false);

            backgroundImage.DOFade(1f, 0f);
            quickShotText.DOFade(1f, 0f);
        });
    }

    private void UpdateTimerDisplay(float elapsedTime)
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        quickShotText.text = $"{seconds:00}\"{((elapsedTime - seconds) * 100):00}";
    }

    public int CalculateScore()
    {
        float normalizedTime = Mathf.Clamp01(elapsedTime / maxScoreTime);
        int score = Mathf.RoundToInt(Mathf.Lerp(maxScore, 0, normalizedTime));
        return score;
    }
}
