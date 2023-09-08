using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelObject levelPrefab;
    [SerializeField] private Transform levelRow1;
    [SerializeField] private Transform levelRow2;
    [SerializeField] private Transform levelRow3;

    [SerializeField] private int levelCount = 5;

    private int previousLevelStars = 0;

    public void SetLevels()
    {
        DestroyLevels();

        for (int i = 1; i <= levelCount; i++)
        {
            int stars = PlayerPrefs.GetInt($"Level{i}_Stars", 0);

            LevelObject level;

            if (i > 10)
            {
                level = Instantiate(levelPrefab, levelRow3);
            }
            else if (i > 5)
            {
                level = Instantiate(levelPrefab, levelRow2);
            }
            else
            {
                level = Instantiate(levelPrefab, levelRow1);
            }

            bool isLocked = false;

            if (i != 1 && stars == 0)
            {
                isLocked = true;
            }

            // Level unlocked
            if (i != 1 && previousLevelStars > 0)
            {
                isLocked = false;
            }

            previousLevelStars = stars;

            float accuracy = LoadAccuracy(i);

            level.SetLevel(i, stars, accuracy, isLocked);

            LoadBestTime(i ,level);
        }
    }

    private void DestroyLevels()
    {
        foreach (Transform item in levelRow1)
        {
            Destroy(item.gameObject);
        }
        
        foreach (Transform item in levelRow2)
        {
            Destroy(item.gameObject);
        }
        
        foreach (Transform item in levelRow3)
        {
            Destroy(item.gameObject);
        }
    }

    private void LoadBestTime(int levelIndex, LevelObject level)
    {
        float loadedSecond;
        LoadLevelTimes(levelIndex, out loadedSecond);

        level.SetBestTime(loadedSecond);
    }

    private float LoadAccuracy(int levelIndex)
    {
        string accuracyKey = $"Level{levelIndex}_Accuracy";
        return PlayerPrefs.GetFloat(accuracyKey, 0);
    }

    public static void SaveLevelStars(int levelIndex, int stars, float second, float accuracy)
    {
        string key = $"Level{levelIndex}_Stars";
        int previousStars = PlayerPrefs.GetInt(key, 0);

        if (stars > previousStars)
        {
            PlayerPrefs.SetInt(key, stars);
        }
        
        string accuracyKey = $"Level{levelIndex}_Accuracy";
        float previousAccuracy = PlayerPrefs.GetInt(accuracyKey, 0);

        if (accuracy > previousAccuracy)
        {
            PlayerPrefs.SetFloat(accuracyKey, accuracy);
        }

        float previousSecond = PlayerPrefs.GetFloat($"{key}_Second", -1f);
        if (second != -1f && (previousSecond == -1f || second < previousSecond))
        {
            PlayerPrefs.SetFloat($"{key}_Second", second);
        }

        PlayerPrefs.Save();
    }

    public static void LoadLevelTimes(int levelIndex, out float second)
    {
        string key = $"Level{levelIndex}_Stars";
        second = PlayerPrefs.GetFloat($"{key}_Second", -1f);
    }
}
