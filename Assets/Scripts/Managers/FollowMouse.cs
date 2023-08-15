using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private Vector3 pos;

    private void Update()
    {
        if (gameManager.isEnded) 
        {
            gameObject.SetActive(false);
        }

        Cursor.visible = false;
        pos = Input.mousePosition;
        transform.position = pos;
    }
}
