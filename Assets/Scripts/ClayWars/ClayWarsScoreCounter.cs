using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Make sure you have DOTween imported
using System.Linq;

public class ClayWarsScoreCounter : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject textPopup;
    [SerializeField] private Transform parent;

    [SerializeField] private AnimationCurve placingChangeCurve;

    private List<ScoreRow> scoreRows = new List<ScoreRow>();

    public static ClayWarsScoreCounter Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitPlayers();
    }

    private void InitPlayers()
    {
        for (int i = 1; i <= ClayWarsGameManager.playerCount; i++)
        {
            ScoreRow row = Instantiate(prefab, parent).GetComponent<ScoreRow>();
            row.SetRow("Player" + i.ToString(), 0, i);
            scoreRows.Add(row);
        }
    }

    public void UpdatePlayerScoreAndPlacing(int playerIndex, int scoresToAdd)
    {
        if (playerIndex >= 0 && playerIndex < scoreRows.Count)
        {
            scoreRows[playerIndex].AddScore(scoresToAdd);

            if(scoreRows.Count > 1)
            {
                int newPlacing = GetPlacingForScore(scoreRows[playerIndex].score);
                scoreRows[playerIndex].UpdatePlacing(newPlacing);

                UpdateLeaderboard();
            }
        }
    }

    private void UpdateLeaderboard()
    {
        //anim
        for (int i = 0; i < scoreRows.Count; i++)
        {
            int newPlacing = GetPlacingForScore(scoreRows[i].score);
            AnimatePlacingChange(i, scoreRows[i].currentPlacing, newPlacing);
        }

        //execute
        for (int i = 0; i < scoreRows.Count; i++)
        {
            int newPlacing = GetPlacingForScore(scoreRows[i].score);
            scoreRows[i].UpdatePlacing(newPlacing);
        }
    }


    private int GetPlacingForScore(int score)
    {
        scoreRows.Sort((a, b) => b.score.CompareTo(a.score));

        int index = scoreRows.FindIndex(row => row.score == score);

        if (index == -1)
        {
            return 1;
        }

        return index + 1;
    }
    private void AnimatePlacingChange(int playerIndex, int previousPlacing, int newPlacing)
    {
        if (newPlacing < previousPlacing)
        {
            float newYPosition = scoreRows[newPlacing - 1].GetComponent<RectTransform>().anchoredPosition.y;
            Vector2 targetAnchoredPosition = new Vector2(scoreRows[playerIndex].GetComponent<RectTransform>().anchoredPosition.x, newYPosition);

            float duration = 0.5f;
            scoreRows[playerIndex].GetComponent<RectTransform>().DOAnchorPos(targetAnchoredPosition, duration).SetEase(placingChangeCurve);
        }
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
}
