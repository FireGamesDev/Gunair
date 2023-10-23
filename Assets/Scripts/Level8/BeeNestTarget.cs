using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Managers;

public class BeeNestTarget : MonoBehaviour, ITarget
{
    //[SerializeField] private float speed = 5f;
    [SerializeField] private float approachSpeed = 2f; // Adjust the speed of approaching the target
    [SerializeField] private float circleRadius = 1.5f; // Adjust the radius of the circular motion
    [SerializeField] private float circleSpeed = 2f; // Adjust the speed of the circular motion
    [SerializeField] private float lifeTime = 8f;
    [SerializeField] private int score = 10;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject scorePopup;

    [Header("SFX")]
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip hitSFX;

    private Vector3 target;

    private bool isDead = false;

    private ScoreManager scoreManager;
    private SpriteRenderer sprite;

    private Coroutine timeupRoutine;

    private void Start()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        sprite = GetComponent<SpriteRenderer>();

        SetTarget();
    }

    private void Update()
    {
        if (!isDead)
        {
            if (Vector2.Distance(transform.position, target) > 0.1f)
            {
                // Calculate the direction towards the target
                Vector2 direction = ((Vector2)target - (Vector2)transform.position).normalized;

                // Calculate the angle of rotation for the circular motion
                float angle = Time.time * circleSpeed;

                // Calculate the circular position using sine and cosine functions
                Vector2 circlePosition = new Vector2(target.x, target.y) + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circleRadius;

                // Calculate the desired position by combining the circular position and the direction towards the target
                Vector2 desiredPosition = circlePosition + direction * circleRadius;

                // Move towards the desired position using both circular and approach speed
                Vector2 newPosition = Vector2.MoveTowards(transform.position, desiredPosition, Time.deltaTime * approachSpeed);

                // Update the position
                transform.position = newPosition;

                // Determine the direction of movement
                bool isMovingLeft = target.x < transform.position.x;
                //sprite.flipX = isMovingLeft;
            }
            else
            {
                SetTarget();
            }

            if (lifeTime > 0)
            {
                lifeTime -= Time.deltaTime;
            }
            else
            {
                if (timeupRoutine == null)
                {
                    timeupRoutine = StartCoroutine(Timeup());
                }
            }
        }
    }

    private void SetTarget()
    {
        if (lifeTime > 0)
        {
            target = target + new Vector3(Random.Range(-12, 12), Random.Range(-12, 12), 0);
            if (target.x > 8)
                target.x = 8;
            if (target.x < -8)
                target.x = -8;
            if (target.y > 4)
                target.y = 4;
            if (target.y < -2)
                target.y = -2;
        }
    }

    public void Hit(RaycastHit contactPoint)
    {
        if (isDead) return;

        isDead = true;

        //Instantiate(explosionEffect, transform.position, Quaternion.identity);

        GameObject hitGo = Instantiate(hitPrefab, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
        Destroy(hitGo, 1f);

        Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(hitSFX, Random.Range(0.8f, 1.2f));

        //score popup
        var popup = Instantiate(scorePopup, contactPoint.point, Quaternion.identity);
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = score.ToString();
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
        Destroy(popup, 2);

        scoreManager.AddScore(score);

        Destroy(gameObject);
    }

    private IEnumerator Timeup()
    {
        float duration = 3f; // Total duration in seconds
        float elapsed = 0f; // Elapsed time

        Color startColor = sprite.color; // Initial color of the sprite
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Target color with zero alpha

        while (elapsed < duration)
        {
            float t = elapsed / duration; // Calculate the normalized time

            // Interpolate the color between startColor and endColor based on the normalized time
            Color currentColor = Color.Lerp(startColor, endColor, t);

            sprite.color = currentColor; // Update the sprite color

            elapsed += Time.deltaTime; // Increase the elapsed time by the frame time
            yield return null; // Wait for the next frame
        }

        // Set the final color to ensure it reaches the target color exactly
        sprite.color = endColor;

        Destroy(gameObject);
    }
}

