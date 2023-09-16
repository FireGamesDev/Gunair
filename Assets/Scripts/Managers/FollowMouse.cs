using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ClayWarsGameManager clayGameManager;
    [SerializeField] private Canvas canvas;

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

        /*

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Menu")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            // Check if the mouse is over the canvas
            if (canvas != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 localPos))
            {
                // Update the UI element's position
                transform.localPosition = localPos;
            }
        }
        else
        {
            

            
        }*/
    }
}
