using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MapBgSetterUI : MonoBehaviour
{
    [SerializeField] private Sprite desertBg;
    [SerializeField] private Sprite forestBg;

    private Image image;

    private void OnEnable()
    {
        image = GetComponent<Image>();
        UpdateImg();

        InvokeRepeating(nameof(UpdateImg), 0f, 2f);
    }

    private void UpdateImg()
    {
        if (PlayerPrefs.GetString("currentMap", "Desert") == "Desert")
        {
            ChangeImage(desertBg);
        }
        else
        {
            ChangeImage(forestBg);
        }
    }

    private void ChangeImage(Sprite newSprite)
    {
        image.sprite = newSprite;
    }

    public void UpdateImgTransition()
    {
        UpdateImg();
    }
}
