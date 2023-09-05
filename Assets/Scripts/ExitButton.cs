using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour, ITarget
{
    [SerializeField] private GameObject sure;

    [SerializeField] private bool isSure = false;

    public void Hit(RaycastHit contactPoint)
    {
        if (isSure)
        {
            MenuManager.isMinigamesActive = true;
            MenuManager.isClaywarsActive = false;

            SceneManager.LoadScene("Menu");
        }
        else
        {
            sure.SetActive(true);
        }
    }
}
