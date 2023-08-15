using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplittingBalls : MonoBehaviour, ITarget
{
    [SerializeField] private GameObject nextBallPrefab;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject scorePopup;
    [SerializeField] private bool shouldSplit = true;

    [Header("Base")]
    [SerializeField] private int score;
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip hitSFX;

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    private void Split()
    {
        if (!shouldSplit)
        {
            return;
        }

        // Instantiate two nextBallPrefab objects
        GameObject ball1 = Instantiate(nextBallPrefab, transform.position, Quaternion.identity);
        GameObject ball2 = Instantiate(nextBallPrefab, transform.position, Quaternion.identity);

        // Calculate a direction vector for the launch
        Vector3 launchDirection = Random.onUnitSphere;

        // Apply a force to each ball to launch them away from each other
        float launchForce = 10f;
        ball1.GetComponent<Rigidbody>().AddForce(launchDirection * launchForce, ForceMode.Impulse);
        ball2.GetComponent<Rigidbody>().AddForce(-launchDirection * launchForce, ForceMode.Impulse);
    }

    public void Hit(RaycastHit _hit)
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(hitSFX);

        scoreManager.AddScore(score);

        //score popup
        var popup = Instantiate(scorePopup, _hit.point, Quaternion.identity);
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = score.ToString();
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
        Destroy(popup, 2);

        Split();

        Destroy(gameObject);
    }
}
