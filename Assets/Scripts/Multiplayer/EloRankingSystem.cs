using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using System.Collections;
using PlayFab.ClientModels;
using System;
using UnityEngine.UI;

public class EloRankingSystem : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text eloText;
    public static int eloRating = 0;

    private static int KFactor = 0;
    private int maxKFactor = 256; 
    private int minKFactor = 32;

    private const int defaultELO = 500;

    private bool isLoading = false;
    [SerializeField] private bool isGameScene = false;

    private void Start()
    {
        KFactor = minKFactor;

        InvokeRepeating(nameof(OnPlayfabLogin), 0.5f, 1f);
    }

    public void OnPlayfabLogin()
    {
        if (isLoading) return;

        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            isLoading = true;

            AssignBalancedKFactorToWorkWithLessThanMaxKFactorPlayers();

            GetEloRating();
        }
    }

    private void AssignBalancedKFactorToWorkWithLessThanMaxKFactorPlayers()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            StatisticName = "ELOLeaderboard"
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        int factor;
        factor = maxKFactor - result.Leaderboard.Count;

        if (factor < minKFactor)
        {
            KFactor = minKFactor;
        }
        else
        {
            KFactor = factor;
        }
    }

    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get leaderboard: " + error.ErrorMessage);
    }

    // Function to calculate the expected score between two players
    private static float CalculateExpectedScore(int ratingA, int ratingB)
    {
        float difference = ratingB - ratingA;
        return 1f / (1f + Mathf.Pow(10f, difference / (400f + KFactor)));
    }

    // Function to calculate the new Elo rating change, 1 win, 0.5 Draw, 0 Loss
    public static int CalculateEloChange(int ratingA, int ratingB, float score)
    {
        // Calculate the expected score based on the difference in ratings
        float expectedScore = CalculateExpectedScore(ratingA, ratingB);

        // Calculate the Elo rating change using the provided formula and K-factor
        int eloChange = Mathf.RoundToInt(KFactor * (score - expectedScore));

        return eloChange;
    }

    #region Playfab Load and Save

    // Call this function to save the Elo rating to PlayFab
    public static void SaveEloRating(int elo)
    {
        // Convert the Elo rating to a string to store it in Player Data
        string eloRatingString = elo.ToString();

        // Call the UpdatePlayerData API to save the Elo rating
        UpdateUserDataRequest request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                // Store the Elo rating with a specific key (e.g., "EloRating")
                {"EloRating", eloRatingString}
            }
        };

        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            // Use the PlayFabClientAPI to make the API call
            PlayFabClientAPI.UpdateUserData(request, OnUpdateUserDataSuccess, OnUpdateUserDataError);
        }
    }

    // Callback for a successful UpdatePlayerData API call
    private static void OnUpdateUserDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Elo rating saved successfully!");
    }

    // Callback for an error in the UpdatePlayerData API call
    private  static void OnUpdateUserDataError(PlayFabError error)
    {
        Debug.LogError("Error saving Elo rating: " + error.ErrorMessage);
    }

    // Call this function to get the Elo rating from PlayFab
    public void GetEloRating()
    {
        // Call the GetUserData API to retrieve the player's data
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetUserDataSuccess, OnGetUserDataError);
    }

    // Callback for a successful GetUserData API call
    private void OnGetUserDataSuccess(GetUserDataResult result)
    {
        // Check if the Elo rating data is available in the result
        if (result.Data != null && result.Data.TryGetValue("EloRating", out UserDataRecord eloRatingData))
        {
            // Convert the Elo rating data to an integer
            if (int.TryParse(eloRatingData.Value, out int elo))
            {
                // Update the local Elo rating variable
                eloRating = elo;

                if (eloText != null)
                {
                    eloText.text = "ELO:" + eloRating.ToString();
                }

                UpdateBadge(elo);

                // Use the Elo rating as needed in your game
                //Debug.Log("Elo rating: " + eloRating);
            }
            else
            {
                Debug.LogWarning("Failed to parse Elo rating data.");
            }
        }
        else
        {
            Debug.LogWarning("Elo rating data not found.");

            SubmitELO(defaultELO);
        }
    }

    #region BadgeSystem

    [Header("BadgeSystem")]
    public Image image1;
    public Image image2;
    public Image image3;
    public Sprite badge1;
    public Sprite badge2;
    public TMPro.TMP_Text rankingName;
    public List<int> levels = new List<int> { 400, 600, 800, 1000, 1200, 1500, 1700 }; // Customize the level ranges as needed
    public List<string> rankNames = new List<string>();
    public GameObject eliteBadgeGo; // Reference to the GameObject for the elite badge

    private Dictionary<int, Sprite> badgeSprites;

    public static string rankName;

    private void UpdateBadge(int eloScore)
    {
        // Create the dictionary to map ELO score ranges to badge sprites
        badgeSprites = new Dictionary<int, Sprite>
    {
        { 0, null }, // No badge for ELO scores below the first level
        { levels[0], badge1 },
        { levels[1], badge1 },
        { levels[2], badge1 },
        { levels[3], badge2 },
        { levels[4], badge2 },
        { levels[5], badge2 },
        { levels[6], badge2 }, // Badge 2 for ELO scores above 1700
        { int.MaxValue, badge2 } // Badge 2 for ELO scores above 1700
    };

        SetBadgeImages(eloScore);
    }

    private void SetBadgeImages(int eloScore)
    {
        if (isGameScene) return;

        // Find the appropriate badge for the ELO score using the dictionary
        Sprite badge = GetBadgeForELO(eloScore);

        // Set the badge image for the three images
        image1.sprite = badge;
        image2.sprite = badge;
        image3.sprite = badge;

        if (eloScore < levels[0])
        {
            image1.gameObject.SetActive(false);
            image2.gameObject.SetActive(false);
            image3.gameObject.SetActive(false);

            rankingName.text = rankNames[0];
        }
        else if (eloScore < levels[1])
        {
            image1.gameObject.SetActive(true);
            image2.gameObject.SetActive(false);
            image3.gameObject.SetActive(false);

            rankingName.text = rankNames[1];
        }
        else if (eloScore < levels[2])
        {
            image1.gameObject.SetActive(true);
            image2.gameObject.SetActive(true);
            image3.gameObject.SetActive(false);

            rankingName.text = rankNames[2];
        }
        else if (eloScore < levels[3])
        {
            image1.gameObject.SetActive(true);
            image2.gameObject.SetActive(true);
            image3.gameObject.SetActive(true);

            rankingName.text = rankNames[3];
        }
        else if (eloScore < levels[4])
        {
            image1.gameObject.SetActive(true);
            image2.gameObject.SetActive(false);
            image3.gameObject.SetActive(false);

            rankingName.text = rankNames[4];
        }
        else if (eloScore < levels[5])
        {
            image1.gameObject.SetActive(true);
            image2.gameObject.SetActive(true);
            image3.gameObject.SetActive(false);

            rankingName.text = rankNames[5];
        }
        else if (eloScore < levels[6])
        {
            image1.gameObject.SetActive(true);
            image2.gameObject.SetActive(true);
            image3.gameObject.SetActive(true);

            rankingName.text = rankNames[6];
        }
        else
        {
            image1.gameObject.SetActive(false);
            image2.gameObject.SetActive(false);
            image3.gameObject.SetActive(false);

            rankingName.text = rankNames[7];
        }

        rankName = rankingName.text;

        // Enable or disable the elite badge GameObject based on the ELO score
        eliteBadgeGo.SetActive(eloScore >= levels[6]);
    }

    private Sprite GetBadgeForELO(int eloScore)
    {
        // Find the appropriate badge for the ELO score using the dictionary
        foreach (var level in levels)
        {
            if (eloScore < level)
            {
                return badgeSprites[level];
            }
        }

        return null; // Return null if no matching badge is found (ELO score above the highest level)
    }

    #endregion

    // Callback for an error in the GetUserData API call
    private void OnGetUserDataError(PlayFabError error)
    {
        Debug.LogError("Error getting Elo rating: " + error.ErrorMessage);
    }

    public static void UpdateELOScore(int eloChange, float score)
    {
        if (score == 1f)
        {
            SubmitELO(eloRating + eloChange);
        } else if(score == 0.5f)
        {
            SubmitELO(eloRating + eloChange);
        }
        else
        {
            SubmitELO(eloRating - eloChange);
        }
    }

    private static void SubmitELO(int eloScore)
    {
        string leaderboardStatisticName = "ELOLeaderboard";

        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardStatisticName,
                    Value = eloScore
                }
            }
        };

        print(eloScore);

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnErrorStatic);

        SaveEloRating(eloScore);
    }

    private static void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score submitted successfully!");
    }

    private static void OnErrorStatic(PlayFabError error)
    {
        Debug.LogError("Failed to submit score: " + error.ErrorMessage);
    }

    #endregion
}