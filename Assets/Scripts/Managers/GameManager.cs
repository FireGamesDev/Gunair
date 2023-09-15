using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private ScoreManager scoreManager;

    public bool isEnded { get; private set; } = false;

    private void Update()
    {
        if (isEnded)
        {
            //Cursor.visible = true;
        }  
    }

    public void End()
    {
        if (isEnded) return;

        isEnded = true;

        if (scoreManager.targetScore > scoreManager.score)
        {
            Lose();
        }
        else
        {
            Win();
        }
    }

    private void Lose()
    {
        endScreen.SetActive(true);

        scoreManager.SetStars();
        scoreManager.SetFontColors();
    }
    
    private void Win()
    {
        if (nextButton != null) nextButton.SetActive(true);

        endScreen.SetActive(true);

        scoreManager.SetStars();
        scoreManager.SetFontColors();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Next()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Menu()
    {
        MenuManager.isMinigamesActive = true;
        MenuManager.isClaywarsActive = false;

        SceneManager.LoadScene("Menu");
    }
}
