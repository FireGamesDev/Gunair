using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CarnivalTarget : MonoBehaviour, ITarget
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int score = 10;
    [SerializeField] private bool isObstacle = false;
    [SerializeField] private bool notMovingTarget = false;
    [SerializeField] private bool isClay = false;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject scorePopup;

    [Header("SFX")]
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip ricochetSFX;

    private float xDirection = 0f;

    private ScoreManager scoreManager;

    private void Start()
    {
        if (!isClay)
        {
            scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        }
        

        speed *= Random.Range(0.4f, 3f);
    }

    private void Update()
    {
        if (isClay) return;

        // out of the map
        if (Mathf.Abs(transform.position.x) > 10 || Mathf.Abs(transform.position.y) > 10)
        {
            Destroy(gameObject);
        }

        if (notMovingTarget) return;

        transform.Translate(Vector3.right * xDirection * speed * Time.deltaTime);
    }

    public void SetDirection(bool isLeft)
    {
        if (isLeft) xDirection = -1f;
        else xDirection = 1f;
    }

    public void Hit(RaycastHit contactPoint)
    {
        if (!isObstacle)
        {
            print("hit");
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

            GameObject hitGo = Instantiate(hitPrefab, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
            Destroy(hitGo, 1f);

            Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(hitSFX, Random.Range(0.8f, 1.2f));

            if(scoreManager != null) scoreManager.AddScore(score);

            if (isClay)
            {
                int quickshotScore = QuickShot.Instance.CalculateScore();

                score += quickshotScore;

                QuickShot.Instance.StopTimer();
                if (PhotonNetwork.InRoom)
                {
                    MultiplayerGameManager.Instance.UpdateScore(score);
                }
                else
                {
                    ClayWarsGameManager.Instance.UpdateScore(score);
                }

                ClayWarsScoreCounter.Instance.TextFeedback(quickshotScore, contactPoint.point);
            }

            Destroy(gameObject);

            //score popup
            var popup = Instantiate(scorePopup, contactPoint.point, Quaternion.identity);
            popup.transform.LookAt(Camera.main.transform);
            Vector3 scale = popup.transform.localScale;
            scale.x *= -1;
            popup.transform.localScale = scale;
            popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = score.ToString();
            popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
            Destroy(popup, 1);

            print(PhotonNetwork.InRoom + " in room");

            if (PhotonNetwork.InRoom)
            {
                if (MultiplayerGameManager.GetLocalPlayerIndex() == ClayWarsRoundManager.Instance.currentPlayerIndexInRound)
                {
                    //GetComponent<PhotonView>().RPC(nameof(DestroyDiscRPC), RpcTarget.All);
                    PhotonNetwork.Destroy(gameObject);
                }
                PhotonNetwork.Destroy(gameObject);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
        else
        {
            GameObject hitGo = Instantiate(hitPrefab, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
            Destroy(hitGo, 1f);

            //score popup
            var popup = Instantiate(scorePopup, contactPoint.point, Quaternion.identity);
            popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = score.ToString();
            popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.red;
            Destroy(popup, 2);

            Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(ricochetSFX, Random.Range(0.8f, 1.2f));

            if (scoreManager != null) scoreManager.RemoveScore(score);
        }
    }

    [PunRPC]
    public void DestroyDiscRPC()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (isClay)
        {
            ClayWarsDiscSpawner.Instance.currentDiscCount--;
        }
    }
}