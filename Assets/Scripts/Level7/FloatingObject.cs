using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour, ITarget
{
    [Header("Movement")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float hitForce = 10f;
    [SerializeField] private float curveAmount = 1f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private int lives = 3;

    [Header("Target")]
    [SerializeField] private int score = 10;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject scorePopup;

    [Header("SFX")]
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip shatterSFX;

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();

        ThrowObject();
    }

    private void ThrowObject()
    {
        // Calculate the curve based on the throw direction (left or right)
        float curve = Random.Range(-curveAmount, curveAmount);

        // Calculate the force vector
        Vector3 force = new Vector3(curve, 1f, 0f) * throwForce;

        // Apply the force to the Rigidbody
        rb.AddForce(force, ForceMode.Impulse);
    }

    private void Update()
    {
        Rotation();

        // out of the map
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    private void Rotation()
    {
        // Get the velocity of the Rigidbody
        Vector3 velocity = rb.velocity;

        // Calculate the rotation angle based on velocity
        float targetRotationAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg / 6;

        // Smoothly interpolate the rotation
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetRotationAngle);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Hit(RaycastHit contactPoint)
    {
        HitEffect(contactPoint);

        AddUpForce(contactPoint);

        lives--;
        if (lives < 1)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void AddUpForce(RaycastHit contactPoint)
    {
        // Calculate the direction from the bullet to the contact point
        Vector3 direction = contactPoint.point - transform.position;
        direction.Normalize();

        // Calculate the curve based on the direction
        float curveX = direction.x;
        float curveY = 0f;

        if (contactPoint.point.y < transform.position.y + (transform.localScale.y * 0.5f))
        {
            // Set a non-zero curveY value if y is lower than half of the bullet's height
            curveY = 1f;
        }

        Vector3 force = new Vector3(curveX, curveY, 0f) * hitForce;

        // Apply the force to the Rigidbody
        rb.AddForce(force, ForceMode.Impulse);
    }

    private void HitEffect(RaycastHit contactPoint)
    {
        GameObject hitGo = Instantiate(hitPrefab, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
        Destroy(hitGo, 1f);

        int scoreToAdd = score;

        if (lives == 1)
        {
            scoreToAdd *= 3;
            Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(shatterSFX, Random.Range(0.8f, 1.2f));
        }
        else
        {
            
            Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(hitSFX, Random.Range(0.8f, 1.2f));
        }

        scoreManager.AddScore(scoreToAdd);

        //score popup
        var popup = Instantiate(scorePopup, contactPoint.point, Quaternion.identity);
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = scoreToAdd.ToString();
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
        Destroy(popup, 1);
    }
}
