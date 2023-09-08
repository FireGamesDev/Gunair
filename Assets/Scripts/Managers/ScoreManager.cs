using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Scripts.Managers;
using System.Text.RegularExpressions;

public class ScoreManager : MonoBehaviour
{
    public int targetScore = 200;
    [SerializeField] private List<int> scoreLimits = new List<int>();
    [SerializeField] private bool instantWin = false;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Timer timer;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private TMP_Text higscoreText;
    [SerializeField] private TMP_Text newHighScore;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private GameObject highscoreGo;
    [SerializeField] private GameObject scorePopup;

    [SerializeField] private List<GameObject> stars = new List<GameObject>();

    [Header("SFX")]
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip starSFX;

    public int score { get; private set; } = 0;
    private int higscore = 0;

    private float currentAccuracy = 0f;
    public int shotsFired { get; set; } = 0;
    private int shotsHit = 0;


    private void Start()
    {
        higscore = PlayerPrefs.GetInt("Highscore", 0);
        higscoreText.text = higscore.ToString();

        SetTargetScore();
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();

        var popup = Instantiate(scorePopup, scoreText.transform);
        popup.GetComponent<TMP_Text>().text = value.ToString();
        popup.GetComponent<TMP_Text>().color = Color.white;

        Destroy(popup, 1);

        CheckIfEnded();

        shotsHit++;

        UpdateAccuracy();
    }
    
    public void RemoveScore(int value)
    {
        score -= value;
        scoreText.text = score.ToString();

        var popup = Instantiate(scorePopup, scoreText.transform);
        popup.GetComponent<TMP_Text>().text = value.ToString();
        popup.GetComponent<TMP_Text>().color = Color.red;

        Destroy(popup, 1);

        CheckIfEnded();
    }

    private void CheckIfEnded()
    {
        if (score >= scoreLimits[2] && instantWin)
        {
            gameManager.End();
        }

        if (score > higscore)
        {
            //highscoreGo.SetActive(true);
            higscore = score;

            higscoreText.text = higscore.ToString();

            PlayerPrefs.SetInt("Highscore", higscore);
        }
    }

    private void SetTargetScore()
    {
        targetText.text = targetScore.ToString();
    }

    public void SetStars()
    {
        StartCoroutine(SetStarsRoutine());
    }

    private IEnumerator SetStarsRoutine()
    {
        if (score >= scoreLimits[2])
        {
            if (instantWin)
            {
                SaveStars(scoreLimits, timer.time);
            }
            else
            {
                SaveStars(scoreLimits, -1);
            }
        }
        else
        {
            SaveStars(scoreLimits, -1);
        }

        for (int i = 0; i < scoreLimits.Count; i++)
        {
            stars[i].SetActive(score >= scoreLimits[i]);
            if (score >= scoreLimits[i])
            {
                Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(starSFX, 1 + 3 * i / 10);
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void SaveStars(List<int> scoreLimits, float time)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string levelNumberString = Regex.Replace(sceneName, "[^0-9]", "");
        int levelNumber = int.Parse(levelNumberString);

        int starRating = 0;

        if (score >= scoreLimits[0])
        {
            starRating = 1;

            if (score >= scoreLimits[1])
            {
                starRating = 2;
                if (score >= scoreLimits[2])
                {
                    starRating = 3;
                }
            }
        }

        float accuracy = currentAccuracy;

        LevelManager.SaveLevelStars(levelNumber, starRating, time, accuracy);
    }

    public void ShotFired()
    {
        shotsFired += 1;
        UpdateAccuracy();
    }

    private void UpdateAccuracy()
    {
        currentAccuracy = Mathf.Min((float)shotsHit / shotsFired * 100f, 100f);

        accuracyText.text = "Accuracy: " + currentAccuracy.ToString("F1");
    }

    public void SetFontColors()
    {
        scoreText.color = Color.white;
        targetText.color = Color.white;
        higscoreText.color = Color.white;
        newHighScore.color = Color.white;
    }
}
