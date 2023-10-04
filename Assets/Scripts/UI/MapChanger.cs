using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MapChanger : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text currentMapDisplay;

    [SerializeField] private List<string> sceneNames = new List<string>();

    private int currentSceneIndex = 0;

    private void Start()
    {
        LoadMap(PlayerPrefs.GetString("currentMap", "Desert"));
    }

    private void LoadMap(string mapName)
    {
        PlayerPrefs.SetString("currentMap", mapName);
        currentSceneIndex = sceneNames.FindIndex(s => s == mapName);
        currentMapDisplay.text = "Current Map: " + mapName;
    }

    public void Next()
    {
        currentSceneIndex++;

        if (currentSceneIndex >= sceneNames.Count)
        {
            currentSceneIndex = 0;
        }

        LoadMap(sceneNames[currentSceneIndex]);
    }

    public void Previous()
    {
        currentSceneIndex--;

        if (currentSceneIndex < 0)
        {
            currentSceneIndex = sceneNames.Count - 1;
        }

        LoadMap(sceneNames[currentSceneIndex]);
    }
}
