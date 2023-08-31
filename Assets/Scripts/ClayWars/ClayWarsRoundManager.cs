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
    [SerializeField] private GunManager shotgun;

    [SerializeField] private int rounds = 4;
    [SerializeField] private GameObject roundCirclePrefab;
    [SerializeField] private Transform parent;

    private int currentRoundNumber = -1;
    public int currentPlayerIndexInRound { get; private set; } = -1;

    public static ClayWarsRoundManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitRoundCircles();

        if (ClayWarsGameManager.playerCount < 2)
        {
            currentPlayerIndexInRound = 0;
        }
        
        if (ClayWarsGameManager.playerCount > 1)
        {
            currentRoundNumber = 0;
        }

        StartCoroutine(FirstRound());
    }

    private IEnumerator FirstRound()
    {
        yield return new WaitForSeconds(1);

        NextPlayer();

        NextRound();
    }

    private void InitRoundCircles()
    {
        for (int i = 0; i < rounds; i++)
        {
            roundIndicatorCircles.Add(Instantiate(roundCirclePrefab, parent));
        }
    }

    public void NextPlayer()
    {
        if (ClayWarsGameManager.playerCount > 1)
        {
            currentPlayerIndexInRound++;

            shotgun.ReloadShotgunOnNewRoundInstantly();

            if (currentPlayerIndexInRound >= ClayWarsGameManager.playerCount)
            {
                currentPlayerIndexInRound = 0;
                currentRoundNumber += 1;
            }
        }  
    }

    public void NextRound()
    {
        if (ClayWarsGameManager.playerCount > 1)
        {
            if (currentRoundNumber < rounds)
            {
                currentPlayerDisplay.text = ClayWarsScoreCounter.Instance.GetPlayerNameByIndex(currentPlayerIndexInRound);

                currentPlayerDisplay.DOFade(0, 0).OnComplete(() =>
                {
                    currentPlayerDisplay.DOFade(1, 1).OnComplete(() =>
                    {
                        currentPlayerDisplay.DOFade(0, 1).SetDelay(1);
                    });
                });
            }
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

        roundIndicatorCircles[currentRoundNumber].GetComponent<Image>().color = Color.white;

        // Fade in and out animation for roundDisplay
        roundDisplay.DOFade(0, 0).OnComplete(() =>
        {
            if(currentRoundNumber == rounds - 1)
            {
                roundDisplay.text = "LAST ROUND";
            }
            else roundDisplay.text = "ROUND " + (currentRoundNumber + 1).ToString();
            roundDisplay.DOFade(1, 1).OnComplete(() =>
            {
                roundDisplay.DOFade(0, 1).SetDelay(1);
            });
        });


        // Scale animation for header
        header.transform.localScale = new Vector3(1, 0, 1);
        header.transform.DOScaleY(1, 1).OnComplete(() =>
        {
            header.transform.DOScaleY(0, 1).SetDelay(1);
        });
    }
}
