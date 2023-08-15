using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour, ITarget
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private CharacterTarget target;
    [SerializeField] private int score;
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject scorePopup;
    [SerializeField] private AudioClip hitSFX;

    public bool damagable { get; set; } = false;

    public void Hit(RaycastHit hit)
    {
        if (damagable)
        {
            Instantiate(explosionEffect, hit.point, Quaternion.identity);

            Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(hitSFX, Random.Range(0.8f, 1.2f));

            scoreManager.AddScore(score);



            //score popup
            Vector3 pos = hit.point;
            pos.z = -1;

            var popup = Instantiate(scorePopup, pos, Quaternion.identity);
            popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = score.ToString();
            popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
            Destroy(popup, 2);

            target.NextBodyPart();

            Destroy(gameObject);
        }
    }
}
