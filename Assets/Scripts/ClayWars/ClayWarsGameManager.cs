using DG.Tweening;
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

    [Header("Winner")]
    [SerializeField] private TMPro.TMP_InputField winnerNameInput;
    [SerializeField] private TMPro.TMP_Text submittedText;

    private string winnerName;

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
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
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

            endScreen.SetActive(true);

            ClayWarsScoreCounter.Instance.parent.gameObject.SetActive(false);

            winnerName = ClayWarsScoreCounter.Instance.GetPlayerWithHighestScore();
            winnerNameInput.text = winnerName;

            StartCoroutine(ScoreDisplayRoutine());
        }
    }

    private IEnumerator ScoreDisplayRoutine()
    {
        for (int i = 0; i < playerCount; i++)
        {
            int scoresToAdd = (int)scoreCounter.GetAccuracyOfPlayer(i);
            scoreCounter.EndExtraScore(i, scoresToAdd);
            yield return new WaitForSeconds(3);
        }

        yield return new WaitForSeconds(1);

        DisplayWinner();
    }

    private void DisplayWinner()
    {
        int winnerScore = ClayWarsScoreCounter.Instance.GetHighestScore();
        winnerScoreText.text = winnerScore.ToString();

        if (playerCount > 1)
        {
            endGameText.text = "The winner is: " + winnerName;
        }
    }

    public void SubmitWinner()
    {
        PlayfabLeaderboard.SubmitScore(winnerNameInput.text, ClayWarsScoreCounter.Instance.GetHighestScore(), false);

        submittedText.gameObject.SetActive(true);
        submittedText.alpha = 1f;

        submittedText.DOFade(0, 1f).OnComplete(() =>
        {
            submittedText.gameObject.SetActive(false);
        });
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
