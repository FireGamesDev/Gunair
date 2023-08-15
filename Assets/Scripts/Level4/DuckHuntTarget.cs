using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckHuntTarget : MonoBehaviour, ITarget
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 8f;
    [SerializeField] private int score = 10;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject scorePopup;
    [SerializeField] private Animator anim;

    [Header("SFX")]
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip hitSFX;

    private Vector3 target;

    private bool isDead = false;
    private bool isStartFalling = false;

    private ScoreManager scoreManager;
    private SpriteRenderer sprite;

    private Coroutine timeupRoutine;

    private void Start()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isDead)
        {
            if (Vector3.Distance(transform.position, target) > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

                // Determine the direction of movement
                bool isMovingLeft = target.x < transform.position.x;

                sprite.flipX = isMovingLeft;
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

            Animation();

        }
        else
        {
            if (isStartFalling)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position - transform.up, 10 * Time.deltaTime);
            }
        }
    }

    private void Animation()
    {
        if (Mathf.Abs(transform.position.x - target.x) < 1)
        {
            anim.SetInteger("Fly", 2);
        } else if (Mathf.Abs(transform.position.y - target.y) < 1)
        {
            anim.SetInteger("Fly", 0);
        } else
        {
            anim.SetInteger("Fly", 1);
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

        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(0.4f);
        isStartFalling = true;

        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    private IEnumerator Timeup()
    {
        speed *= 2;
        target = transform.position + new Vector3(0, 50, 0);

        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }
}
