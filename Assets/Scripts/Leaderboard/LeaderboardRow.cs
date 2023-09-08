using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private Image rankImg;
    [SerializeField] private Text rankingText;
    [SerializeField] private Text nicknameText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text tenthSecondText;

    public void SetRow(int rank, string nickname, int score, bool isLocal)
    {
        rankingText.text = rank.ToString();
        nicknameText.text = nickname;

        scoreText.text = score.ToString();

        if (isLocal)
        {
            Color _orange = new Color(1.0f, 0.64f, 0.0f);
            GetComponent<Image>().color = _orange;
        }
    }
}
