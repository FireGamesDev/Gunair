using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayWarsGameManager : MonoBehaviour
{
    public static int playerCount = 1;
    public static ClayWarsGameManager Instance;

    public bool isEnded = false;

    private int currentPlayerIndex = 0;

    [SerializeField] private ClayWarsScoreCounter scoreCounter;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int scoresToAdd)
    {
        scoreCounter.UpdatePlayerScoreAndPlacing(currentPlayerIndex, scoresToAdd);
    }
}
