using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClayWarsGameManager : MonoBehaviour
{
    [SerializeField] private ClayWarsScoreCounter scoreCounter;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMPro.TMP_Text winnerScoreText;
    [SerializeField] private TMPro.TMP_Text endGameText;

    public static int playerCount = 1;
    public static ClayWarsGameManager Instance;

    public bool isEnded { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int scoresToAdd)
    {
        scoreCounter.UpdatePlayerScoreAndPlacing(ClayWarsRoundManager.Instance.currentPlayerIndexInRound, scoresToAdd);
    }

    public void EndGame()
    {
        if (!isEnded)
        {
            isEnded = true;

            int winnerScore = ClayWarsScoreCounter.Instance.GetHighestScore();
            string winnerName = ClayWarsScoreCounter.Instance.GetPlayerWithHighestScore();
            winnerScoreText.text = winnerScore.ToString();
            endGameText.text = "The winner is: " + winnerName;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
