using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject clayWarsGo;

    public static bool isClaywarsActive = false;
    public static bool isMinigamesActive = false;

    private void Update()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void Start()
    {
        if (isClaywarsActive)
        {
            clayWarsGo.SetActive(true);
        }

        if (isMinigamesActive)
        {
            Levels();
        }
    }

    public void ResetMenu()
    {
        isClaywarsActive = false;
        isMinigamesActive = false;
    }

    public void Levels()
    {
        levelManager.SetLevels();
        levelManager.gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ClayWars(int playerCount)
    {
        ClayWarsGameManager.playerCount = playerCount;

        SceneManager.LoadScene("ClayWars");
    }
}
