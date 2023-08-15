using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    private void Update()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
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
}
