using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ClayWarsGameManager clayGameManager;

    private Vector3 pos;

    private void Update()
    {
        if (gameManager != null && gameManager.isEnded) 
        {
            gameObject.SetActive(false);
        }
        
        if (clayGameManager != null && clayGameManager.isEnded) 
        {
            gameObject.SetActive(false);
        }

        Cursor.visible = false;

        pos = Input.mousePosition;
        transform.position = pos;
    }
}
