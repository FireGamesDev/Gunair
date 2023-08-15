using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelObject : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text levelNumberText;
    [SerializeField] private Button levelButton;
    [SerializeField] private GameObject lockGo;
    [SerializeField] private List<GameObject> starList = new List<GameObject>();
    [SerializeField] private TMPro.TMP_Text secondText;

    public void SetLevel(int levelNumber, int stars, bool isLocked)
    {
        levelButton.interactable = !isLocked;

        if (isLocked)
        {
            lockGo.SetActive(true);
        }
        else
        {
            levelNumberText.text = levelNumber.ToString();

            for (int i = 0; i < stars; i++)
            {
                starList[i].SetActive(true);
            }

            levelButton.onClick.AddListener(() => { SceneManager.LoadScene("Level" + levelNumber.ToString()); });
        }
    }

    public void SetBestTime(float second)
    {
        if (second != -1f)
        {
            int wholeSeconds = Mathf.FloorToInt(second);
            int milliseconds = Mathf.RoundToInt((second - wholeSeconds) * 100f);

            string displayText = $"{wholeSeconds:D2}:{milliseconds:D2}";
            secondText.text = displayText;
        }
    }
}
