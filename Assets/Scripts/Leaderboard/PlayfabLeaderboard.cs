using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System.Linq;

public class PlayfabLeaderboard : MonoBehaviour
{
    [SerializeField] private GameObject localLeaderboard;
    [SerializeField] private GameObject globalLeaderboard;

    [SerializeField] private Transform rowsParent;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject empty;

    private string LeaderboardStatisticName;
    private bool isLoading = false;

    private string leaderboardChoice;

    private void OnEnable()
    {
        LeaderboardStatisticName = "ClayWarsLeaderboard";
    }

    public void SetLeaderboard()
    {
        leaderboardChoice = PlayerPrefs.GetString("LeaderboardChoice", "Global");
        if (leaderboardChoice == "Local")
        {
            ShowLocalLeaderboard();
        }
        else
        {
            ShowGlobalLeaderboard();
        }
    }

    private void ShowLocalLeaderboard()
    {
        PlayerPrefs.SetString("LeaderboardChoice", "Local");

        localLeaderboard.SetActive(true);
        globalLeaderboard.SetActive(false);

        SetLocalLeaderboard(LoadLocalLeaderboard());
    }


    private void ShowGlobalLeaderboard()
    {
        PlayerPrefs.SetString("LeaderboardChoice", "Global");

        globalLeaderboard.SetActive(true);
        localLeaderboard.SetActive(false);

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

    public static void SubmitScore(string name, int score, bool isGlobal)
    {
        if (isGlobal)
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
        else
        {
            AddScoreToLocalLeaderboard(name, score);
        }
    }

    private static void AddScoreToLocalLeaderboard(string name, int score)
    {
        List<LocalLeaderboardEntry> localLeaderboard = new List<LocalLeaderboardEntry>();

        // Load existing local leaderboard data from PlayerPrefs
        string localLeaderboardData = PlayerPrefs.GetString("LocalLeaderboard", "");
        if (!string.IsNullOrEmpty(localLeaderboardData))
        {
            LocalLeaderboardWrapper wrapper = JsonUtility.FromJson<LocalLeaderboardWrapper>(localLeaderboardData);
            localLeaderboard = wrapper.entries;
        }

        // Check if an entry with the same name exists
        LocalLeaderboardEntry existingEntry = localLeaderboard.Find(entry => entry.playerName == name);

        if (existingEntry != null)
        {
            // If an existing entry is found, compare the scores
            if (score > existingEntry.score)
            {
                // Update the existing entry with the new score
                existingEntry.score = score;
            }
        }
        else
        {
            // No existing entry found, add a new entry
            LocalLeaderboardEntry newEntry = new LocalLeaderboardEntry { playerName = name, score = score };
            localLeaderboard.Add(newEntry);
        }

        // Sort the local leaderboard by score (you can choose ascending or descending order)
        localLeaderboard = localLeaderboard.OrderByDescending(entry => entry.score).ToList();

        // Limit the number of entries if needed (e.g., top 10 scores)
        if (localLeaderboard.Count > 10)
        {
            localLeaderboard = localLeaderboard.Take(10).ToList();
        }

        // Save the updated local leaderboard data in PlayerPrefs
        LocalLeaderboardWrapper updatedWrapper = new LocalLeaderboardWrapper(localLeaderboard);
        string updatedLocalLeaderboardData = JsonUtility.ToJson(updatedWrapper);
        PlayerPrefs.SetString("LocalLeaderboard", updatedLocalLeaderboardData);
        PlayerPrefs.Save();
    }

    private List<LocalLeaderboardEntry> LoadLocalLeaderboard()
    {
        string localLeaderboardData = PlayerPrefs.GetString("LocalLeaderboard", "");
        if (!string.IsNullOrEmpty(localLeaderboardData))
        {
            LocalLeaderboardWrapper wrapper = JsonUtility.FromJson<LocalLeaderboardWrapper>(localLeaderboardData);
            return wrapper.entries;
        }
        return new List<LocalLeaderboardEntry>();
    }

    private void SetLocalLeaderboard(List<LocalLeaderboardEntry> localLeaderboard)
    {
        SetLoading(true);

        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        empty.SetActive(true);

        int rank = 1;
        foreach (var item in localLeaderboard)
        {
            if (item.score == 0) continue;
            GameObject row = Instantiate(rowPrefab, rowsParent);
            //Sprite rankSprite = null;
            if (rank <= 3)
            {
                //rankSprite = rankingSprites[rank - 1];
            }
            bool isLocal = item.playerName == PlayerPrefs.GetString("Nickname");
            int itemRank = rank;
            row.GetComponent<LeaderboardRow>().SetRow(itemRank, item.playerName, item.score, isLocal);
            rank++;

            empty.SetActive(false);
        }
        SetLoading(false);
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

