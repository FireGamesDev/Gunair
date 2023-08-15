using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour, ITarget
{
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private GameObject scorePopup;
    [SerializeField] private ScoreManager scoreManager;

    [Header("SFX")]
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip hitSFX;

    public void Hit(RaycastHit hit)
    {
        GameObject hitGo = Instantiate(circlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(hitGo, 4f);

        Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(hitSFX, Random.Range(0.8f, 1.2f));

        // Calculate the distance between the collider's owner object and the entered object
        float distance = Vector3.Distance(transform.position, hit.point);


        // Assign a value based on the distance thresholds
        int assignedValue;
        if (distance <= 0.32f)
        {
            assignedValue = 30;
        }
        else if (distance <= 0.8f)
        {
            assignedValue = 10;
        }
        else
        {
            assignedValue = 5;
        }

        //score popup

        var popup = Instantiate(scorePopup, hit.point, Quaternion.identity);
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = assignedValue.ToString();
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
        Destroy(popup, 2);

        scoreManager.AddScore(assignedValue);
    }
}
