using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System.Linq;

public class PlayfabLeaderboard : MonoBehaviour
{
    [SerializeField] private Transform rowsParent;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject empty;

    private string LeaderboardStatisticName;
    private bool isLoading = false;

    private void OnEnable()
    {
        LeaderboardStatisticName = "ClayWarsLeaderboard";
        GetLeaderboardAroundThePlayer();
    }

    public void RefreshLeaderboard()
    {
        GetLeaderboardAroundThePlayer();
    }

    private void GetLeaderboardAroundThePlayer()
    {
        SetLoading(true);
        GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = LeaderboardStatisticName
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetLeaderboardAroundPlayerSuccess, OnError);
    }

    public void GetTop20()
    {
        if (isLoading) return;
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }
        SetLoading(true);
        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            StatisticName = LeaderboardStatisticName,
            StartPosition = 0,
            MaxResultsCount = 20
        };
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        empty.SetActive(true);

        List<PlayerLeaderboardEntry> sortedEntries = result.Leaderboard.OrderBy(entry => entry.StatValue).ToList();
        int i = 0;
        foreach (var item in sortedEntries)
        {
            if (item.StatValue == 0) continue;

            GameObject row = Instantiate(rowPrefab, rowsParent);
            //Sprite rankSprite = null;
            if (i < 3)
            {
                //rankSprite = rankingSprites[i];
            }
            i++;
            bool isLocal = item.DisplayName == PlayerPrefs.GetString("Nickname");
            row.GetComponent<LeaderboardRow>().SetRow(i, item.DisplayName, item.StatValue, isLocal);

            empty.SetActive(false);
        }
        SetLoading(false);
    }

    private void OnGetLeaderboardAroundPlayerSuccess(GetLeaderboardAroundPlayerResult result)
    {
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        empty.SetActive(true);

        List<PlayerLeaderboardEntry> sortedEntries = result.Leaderboard; //.OrderBy(entry => entry.StatValue).ToList();
        int localPlayerRank = sortedEntries.FindIndex(entry => entry.DisplayName == PlayerPrefs.GetString("Nickname"));
        int rank = 1;
        foreach (var item in sortedEntries)
        {
            if (item.StatValue == 0) continue;
            GameObject row = Instantiate(rowPrefab, rowsParent);
            //Sprite rankSprite = null;
            if (rank <= 3)
            {
                //rankSprite = rankingSprites[rank - 1];
            }
            bool isLocal = item.DisplayName == PlayerPrefs.GetString("Nickname");
            int itemRank = isLocal ? localPlayerRank + 1 : rank;
            row.GetComponent<LeaderboardRow>().SetRow(itemRank, item.DisplayName, item.StatValue, isLocal);
            rank++;

            empty.SetActive(false);
        }
        SetLoading(false);
    }

    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get leaderboard: " + error.ErrorMessage);
        SetLoading(false);
    }

    public static void SubmitScore(int score)
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "ClayWarsLeaderboard",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnErrorStatic);
    }

    private static void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score submitted successfully!");
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Failed to submit score: " + error.ErrorMessage);
        SetLoading(false);
    }

    private static void OnErrorStatic(PlayFabError error)
    {
        Debug.LogError("Failed to submit score: " + error.ErrorMessage);
    }

    private void SetLoading(bool value)
    {
        isLoading = value;
        loadingScreen.SetActive(value);
    }
}

