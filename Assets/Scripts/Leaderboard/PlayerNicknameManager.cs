using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;
using Scripts.Managers;

public class PlayerNicknameManager : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> letterDisplays = new List<TMP_Text>();

    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private GameObject nameChangeMenu;

    [Header("SFX")]
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip changeSFX;

    private string currentPlayerName;

    private int[] letterIndices;
    private string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    private void Start()
    {
        if (PlayerPrefs.GetString("Nickname", "") != "")
        {
            currentPlayerName = PlayerPrefs.GetString("Nickname", "");
            nameDisplay.text = "Signed in as: " + currentPlayerName;
            Login(currentPlayerName);
        }
        else
        {
            RandomizeNickname();

            nameChangeMenu.SetActive(true);
        }
    }

    public void RandomizeNickname()
    {
        letterIndices = new int[3];

        for (int i = 0; i < 3; i++)
        {
            letterIndices[i] = Random.Range(0, alphabet.Length);
            UpdateNicknameDisplay(i, alphabet[letterIndices[i]]);
        }
    }

    public void SetNickname()
    {
        string currentNickname = GetNickname();
        Debug.Log("Set nickname: " + currentNickname);

        PlayerPrefs.SetString("Nickname", currentNickname);

        currentPlayerName = PlayerPrefs.GetString("Nickname", "");
        nameDisplay.text = "Signed in as: " + currentPlayerName;
        Login(currentPlayerName);
    }

    public string GetNickname()
    {
        string nickname = alphabet[letterIndices[0]] + alphabet[letterIndices[1]] + alphabet[letterIndices[2]];
        return nickname;
    }

    public void ChangeLetter1(int direction)
    {
        ChangeLetter(0, direction);

        PlaySFX();
    }

    public void ChangeLetter2(int direction)
    {
        ChangeLetter(1, direction);

        PlaySFX();
    }
    
    public void ChangeLetter3(int direction)
    {
        ChangeLetter(2, direction);

        PlaySFX();
    }

    private void ChangeLetter(int index, int direction)
    {
        letterIndices[index] = (letterIndices[index] + direction) % alphabet.Length;

        if (letterIndices[index] < 0)
        {
            letterIndices[index] += alphabet.Length;
        }

        letterDisplays[index].text = alphabet[letterIndices[index]];
    }

    private void PlaySFX()
    {
        Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(changeSFX);
    }

    private void UpdateNicknameDisplay(int i, string newLetter)
    {
        letterDisplays[i].text = newLetter;
    }

    #region Login
    public void Login(string username)
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
        {
            CustomId = username,
            CreateAccount = true // Create a new account if it doesn't exist
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");

        // Set the display name after successful login
        string displayName = PlayerPrefs.GetString("Nickname", "");
        UpdateDisplayName(displayName);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.ErrorMessage);
    }

    private void UpdateDisplayName(string displayName)
    {
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateDisplayNameSuccess, OnUpdateDisplayNameFailure);
    }

    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display name updated successfully!");
    }

    private void OnUpdateDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update display name: " + error.ErrorMessage);
    }
    #endregion
}
