using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionDropdown : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Dropdown dropdown;
    [SerializeField] private string defaultText = "Default Text";

    public static bool isDefault = false;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("Region", -1) == -1)
        {
            isDefault = true;
        }
    }

    private void OnGUI()
    {
        if (isDefault)
        {
            dropdown.captionText.text = defaultText;
        }
    }
}
