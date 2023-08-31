using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreRow : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private float rowHeight = 10f;

    public int score { get; private set; } = 0;
    public string playerName { get; private set; }

    public void SetRow(string playerName, int score, int placing)
    {
        nameText.text = playerName;

        this.playerName = playerName;

        scoreText.text = score.ToString();

        this.score = score;

        Vector2 newAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        newAnchoredPosition.y = -(placing - 1) * rowHeight;
        GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
    }

    public void AddScore(int value)
    {
        score += value;

        UpdateScore(score);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();

        this.score = score;
    }
}
