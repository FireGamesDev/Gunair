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

    private void Update()
    {
        if (isEnded)
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void UpdateScore(int scoresToAdd)
    {
        scoreCounter.UpdatePlayerScore(ClayWarsDiscSpawner.Instance.currentPlayerIndexToGiveScoreTo, scoresToAdd);
    }

    public void EndGame()
    {
        if (!isEnded)
        {
            isEnded = true;

            int winnerScore = ClayWarsScoreCounter.Instance.GetHighestScore();
            string winnerName = ClayWarsScoreCounter.Instance.GetPlayerWithHighestScore();
            winnerScoreText.text = winnerScore.ToString();

            if (playerCount > 1)
            {
                endGameText.text = "The winner is: " + winnerName;
            }

            if (winnerName.Contains(PlayerPrefs.GetString("Nickname", "")))
            {
                PlayfabLeaderboard.SubmitScore(winnerScore);
            }

            endScreen.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void Menu()
    {
        MenuManager.isClaywarsActive = true;
        MenuManager.isMinigamesActive = false;

        SceneManager.LoadScene("Menu");
    }
}