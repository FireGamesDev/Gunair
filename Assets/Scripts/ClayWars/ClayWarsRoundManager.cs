using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClayWarsRoundManager : MonoBehaviour
{
    private List<GameObject> roundIndicatorCircles = new List<GameObject>();
    [SerializeField] private GameObject header;
    [SerializeField] private TMPro.TMP_Text roundDisplay;
    [SerializeField] private TMPro.TMP_Text currentPlayerDisplay;

    [SerializeField] private int rounds = 4;
    [SerializeField] private GameObject roundCirclePrefab;
    [SerializeField] private Transform parent;

    private int currentRoundNumber = 0;
    public int currentPlayerIndexInRound { get; private set; } = -1;

    public static ClayWarsRoundManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitRoundCircles();

        NextRound();
    }

    private void InitRoundCircles()
    {
        for (int i = 0; i < rounds; i++)
        {
            roundIndicatorCircles.Add(Instantiate(roundCirclePrefab, parent));
        }
    }

    public void NextRound()
    {
        if (ClayWarsGameManager.playerCount > 1)
        {
            currentPlayerIndexInRound++;

            currentPlayerDisplay.text = ClayWarsScoreCounter.Instance.GetPlayerNameByIndex(currentPlayerIndexInRound);

            if (currentPlayerIndexInRound >= ClayWarsGameManager.playerCount)
            {
                currentPlayerIndexInRound = -1;
                currentRoundNumber += 1;
            }

            currentPlayerDisplay.DOFade(0, 0).OnComplete(() =>
            {
                currentPlayerDisplay.DOFade(1, 1).OnComplete(() =>
                {
                    currentPlayerDisplay.DOFade(0, 1).SetDelay(1);
                });
            });
        }
        else
        {
            currentRoundNumber += 1;
        }

        if (currentRoundNumber >= rounds)
        {
            ClayWarsGameManager.Instance.EndGame();

            return;
        }

        roundIndicatorCircles[currentRoundNumber - 1].GetComponent<Image>().color = Color.white;


        // Fade in and out animation for roundDisplay
        roundDisplay.DOFade(0, 0).OnComplete(() =>
        {
            roundDisplay.text = "ROUND " + currentRoundNumber.ToString();
            roundDisplay.DOFade(1, 1).OnComplete(() =>
            {
                roundDisplay.DOFade(0, 1).SetDelay(1);

                ClayWarsDiscSpawner.Instance.NewRound();
            });
        });


        // Scale animation for header
        header.transform.localScale = Vector3.zero;
        header.transform.DOScaleY(1, 1).OnComplete(() =>
        {
            header.transform.DOScaleY(0, 1).SetDelay(1);
        });
    }
}
