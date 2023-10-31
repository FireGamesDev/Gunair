using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class LeaderboardChanger : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text currentLeaderboardDisplay;

    [SerializeField] private List<string> leaderboardNames = new List<string>();

    [SerializeField] private PlayfabLeaderboard leaderboard;

    private int currentIndex = 0;

    private void Start()
    {
        LoadLeaderboard(PlayerPrefs.GetString("LeaderboardChoice", "Global"));
    }

    private void LoadLeaderboard(string leaderboardChoice)
    {
        PlayerPrefs.SetString("LeaderboardChoice", leaderboardChoice);

        currentIndex = leaderboardNames.FindIndex(s => s == leaderboardChoice);
        currentLeaderboardDisplay.text = leaderboardChoice;

        leaderboard.SetLeaderboard();
    }

    public void Next()
    {
        currentIndex++;

        if (currentIndex >= leaderboardNames.Count)
        {
            currentIndex = 0;
        }

        LoadLeaderboard(leaderboardNames[currentIndex]);
    }

    public void Previous()
    {
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = leaderboardNames.Count - 1;
        }

        LoadLeaderboard(leaderboardNames[currentIndex]);
    }
}
