using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeHive : MonoBehaviour, ITarget
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject scorePopup;

    [SerializeField] private int score = -10;

    private Coroutine routine;

    private void Start()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    public void Hit(RaycastHit contactPoint)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(OnHit());



        //score popup
        var popup = Instantiate(scorePopup, contactPoint.point, Quaternion.identity);
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = score.ToString();
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
        Destroy(popup, 2);

        scoreManager.AddScore(score);
    }

    private IEnumerator OnHit()
    {
        sprite.color = Color.red;



        yield return new WaitForSeconds(0.2f);

        sprite.color = Color.white;
    }
}
