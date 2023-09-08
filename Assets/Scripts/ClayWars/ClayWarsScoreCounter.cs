using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ClayWarsScoreCounter : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject textPopup;
    [SerializeField] private Transform parent;
    [SerializeField] private TMPro.TMP_Text accuracyText;

    private List<ScoreRow> scoreRows = new List<ScoreRow>();

    private Dictionary<int, int> shotsFiredByPlayers = new Dictionary<int, int>();
    private Dictionary<int, int> shotsHitByPlayers = new Dictionary<int, int>();

    public static ClayWarsScoreCounter Instance;

    private float currentAccuracy = 0f;
    public int shotsFired { get; set; } = 0;
    public int shotsHit { get; set; } = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitPlayers();

        UpdateAccuracyText(0f);
    }

    private void InitPlayers()
    {
        for (int i = 1; i <= ClayWarsGameManager.playerCount; i++)
        {
            ScoreRow row = Instantiate(prefab, parent).GetComponent<ScoreRow>();
            if (i == 1)
            {
                row.SetRow(PlayerPrefs.GetString("Nickname", ""), 0, i);
            }
            else
            {
                row.SetRow(PlayerPrefs.GetString("Nickname", "") + i.ToString(), 0, i);
            }
            
            scoreRows.Add(row);
        }
    }

    public void UpdatePlayerScore(int playerIndex, int scoresToAdd)
    {
        if (playerIndex >= 0 && playerIndex < scoreRows.Count)
        {
            scoreRows[playerIndex].AddScore(scoresToAdd);
        }

        shotsHit++;
        int currentPlayerIndex = ClayWarsRoundManager.Instance.currentPlayerIndexInRound;

        if (shotsHitByPlayers.ContainsKey(currentPlayerIndex))
        {
            shotsHitByPlayers[currentPlayerIndex] = shotsHit;
        }
        else
        {
            shotsHitByPlayers.Add(currentPlayerIndex, shotsHit);
        }

        UpdateAccuracy(currentPlayerIndex);
    }

    public void TextFeedback(int score, Vector3 pos)
    {
        string textFeedback = "";
        Color feedbackColor = Color.white;

        if (score > 14)
        {
            textFeedback = "Awesome!";
            feedbackColor = Color.red;
        }
        else if (score > 10)
        {
            textFeedback = "Great!";
            feedbackColor = Color.green;
        }
        else if (score > 0)
        {
            textFeedback = "Good";
            feedbackColor = Color.yellow;
        }

        pos.y -= 0.3f;

        if (textFeedback != "")
        {
            var popup = Instantiate(textPopup, pos, Quaternion.identity);
            popup.transform.LookAt(Camera.main.transform);

            Vector3 scale = popup.transform.localScale;
            scale.x *= -1;
            popup.transform.localScale = scale;

            popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = textFeedback;
            popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = feedbackColor;
            Destroy(popup, 1);
        }
    }

    public string GetPlayerWithHighestScore()
    {
        ScoreRow highestScorer = scoreRows.OrderByDescending(row => row.score).FirstOrDefault();

        if (highestScorer != null)
        {
            return highestScorer.playerName;
        }

        return "";
    }
    
    public string GetPlayerNameByIndex(int index)
    {
        return scoreRows[index].playerName;
    }

    public int GetHighestScore()
    {
        ScoreRow highestScorer = scoreRows.OrderByDescending(row => row.score).FirstOrDefault();

        if (highestScorer != null)
        {
            return highestScorer.score;
        }

        return 0;
    }

    private void UpdateAccuracyText(float value)
    {
        currentAccuracy = value;
        accuracyText.text = "Accuracy: " + currentAccuracy.ToString("F1");
    }
    
    public void ShotFired()
    {
        shotsFired += 1;

        int currentPlayerIndex = ClayWarsRoundManager.Instance.currentPlayerIndexInRound;

        if (shotsFiredByPlayers.ContainsKey(currentPlayerIndex))
        {
            shotsFiredByPlayers[currentPlayerIndex] = shotsFired;
        }
        else
        {
            shotsFiredByPlayers.Add(currentPlayerIndex, shotsFired);
        }

        UpdateAccuracy(ClayWarsRoundManager.Instance.currentPlayerIndexInRound);
    }

    private void UpdateAccuracy(int playerIndex)
    {
        float accuracy = 0f;
        if (shotsFiredByPlayers.ContainsKey(playerIndex) && shotsHitByPlayers.ContainsKey(playerIndex))
        {
            accuracy = (float)shotsHitByPlayers[playerIndex] / shotsFiredByPlayers[playerIndex] * 100f;
        }
        UpdateAccuracyText(accuracy);
    }

    public void NextPlayer(int playerIndex)
    {
        if (shotsFiredByPlayers.ContainsKey(playerIndex))
        {
            shotsFired = shotsFiredByPlayers[playerIndex];
        }
        else
        {
            shotsFired = 0;
        }
        
        if (shotsHitByPlayers.ContainsKey(playerIndex))
        {
            shotsHit = shotsHitByPlayers[playerIndex];
        }
        else
        {
            shotsHit = 0;
        }

        UpdateAccuracy(playerIndex);
    }
}
