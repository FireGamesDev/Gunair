using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private bool isShotgun;

    [Header("Base")]
    [SerializeField] private float shootForce = 100f;
    [SerializeField] private float cooldown = 0.5f;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject chasingPrefab;
    [SerializeField] private GameObject reticle;
    [SerializeField] private GameObject barrel;
    [SerializeField] private GameObject muzzle;
    [SerializeField] private Animator cursor;

    [SerializeField] private GameManager gameManager;

    [Header("Shotgun")]
    [SerializeField] private int pelletCount = 7;
    [SerializeField] private float spreadRadius = 10f;

    [Header("Magazine")]
    [SerializeField] private int magSize = 6;
    [SerializeField] private bool isInfinite = false;
    [SerializeField] private Transform bulletUIParent;
    [SerializeField] private GameObject bulletIconPrefab;
    [SerializeField] private GameObject infiniteBullet;

    [Header("Reloading")]
    [SerializeField] private float reloadTime = 3f;
    [SerializeField] private GameObject reloadingText;
    [SerializeField] private bool noReload = false;
    [SerializeField] private Animator noReloadAnim;

    [Header("SFX")]
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip reloadSFX;
    [SerializeField] private AudioClip outOfAmmoSFX;
    [SerializeField] private AudioClip failSFX;

    [Header("ShotgunSFX")]
    [SerializeField] private AudioClip shotgunReloadStart;
    [SerializeField] private AudioClip shotgunReloadOne;
    [SerializeField] private AudioClip shotgunReloadEnd;

    private bool canShoot = true;
    private bool reloading = false;

    private int currentBulletCount = 0;

    private void Start()
    {
        SetMag();
    }

    private void Update()
    {
        if(gameManager != null) if (gameManager.isEnded) return;
        if (reloading) return;

        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canShoot && currentBulletCount > 0)
            {
                canShoot = false;
                Shoot();
            }
            else if (currentBulletCount <= 0)
            {
                Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(outOfAmmoSFX);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (isInfinite) return;

            if (!reloading)
            {
                if (!noReload)
                {
                    reloading = true;
                    StartCoroutine(Reload());
                }
                else
                {
                    Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(failSFX);
                    noReloadAnim.SetTrigger("Action");

                    gameManager.End();
                }
            }
        }
    }

    private void Shoot()
    {
        if (!isShotgun)
        {
            ShootABullet(Input.mousePosition);
        }
        else
        {
            for (int i = 0; i < pelletCount; i++)
            {
                Vector3 spreadPosition = GenerateSpreadPosition(Input.mousePosition, spreadRadius);
                ShootABullet(spreadPosition);
            }
        }

        var chasing = Instantiate(chasingPrefab, barrel.transform.position, transform.rotation);
        chasing.GetComponent<Rigidbody>().AddForce(Vector3.right * 0.7f, ForceMode.Impulse);
        Destroy(chasing, 1f);

        Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFX(shootSFX);

        StartCoroutine(Muzzle());

        cursor.SetTrigger("Shoot");

        if (GameObject.Find("ScoreManager") != null)
        {
            GameObject.Find("ScoreManager").GetComponent<ScoreManager>().ShotFired();
        }
        
        if (GameObject.Find("ClayWarsScoreCounter") != null)
        {
            GameObject.Find("ClayWarsScoreCounter").GetComponent<ClayWarsScoreCounter>().ShotFired();
        }
        

        if (!isInfinite)
        {
            currentBulletCount--;
            DestroyAnAmmo();
        }

        StartCoroutine(Cooldown());
    }


    private Vector3 GenerateSpreadPosition(Vector3 centerPosition, float spreadRadius)
    {
        float randomAngle = Random.Range(0f, 180f);
        float randomYOffset = Random.Range(-spreadRadius, spreadRadius);
        Vector3 randomDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
        Vector3 spreadPosition = centerPosition + randomDirection * Random.Range(0f, spreadRadius) + new Vector3(0f, randomYOffset, 0f);
        return spreadPosition;
    }


    private void ShootABullet(Vector3 raycastDirection)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(raycastDirection);

        //actual shooting with raycast
        if (Physics.Raycast(ray, out hit, 100f) && Vector3.Distance(transform.position, hit.point) < 200f)
        {
            if (hitEffect != null)
            {
                GameObject hitGo = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitGo, 2f);
            }
            

            ITarget target = hit.transform.GetComponent<ITarget>();

            if (target != null)
            {
                target.Hit(hit);

                cursor.SetTrigger("Hit");
            }

            /*
            // Creating the bullet only visual
            Vector3 bulletDirection = (hit.point - transform.position).normalized;
            Quaternion bulletRotation = Quaternion.LookRotation(bulletDirection);

            // Apply 90-degree offset on the y-axis
            Quaternion yRotationOffset = Quaternion.Euler(0f, 90f, 0f);
            bulletRotation *= yRotationOffset;

            var pel = Instantiate(bulletPrefab, reticle.transform.position, bulletRotation);
            pel.GetComponent<Rigidbody>().AddForce(bulletDirection * shootForce);
            Destroy(pel, 2);
            */
        }
    }

    private IEnumerator Muzzle()
    {
        muzzle.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        muzzle.SetActive(false);
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    private void AddBulletToTheMag()
    {
        currentBulletCount++;

        Instantiate(bulletIconPrefab, bulletUIParent);
    }

    private void SetMag()
    {
        DestroyAmmoDisplay();

        currentBulletCount = magSize;

        if (!isInfinite)
        {
            for (int i = 0; i < magSize; i++)
            {
                Instantiate(bulletIconPrefab, bulletUIParent);
            }
        }

        if (isInfinite)
        {
            infiniteBullet.SetActive(true);
        }
    }

    private IEnumerator Reload()
    {
        if (!isShotgun)
        {
            Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFXWithVolume(reloadSFX, 0.3f);
        }
        else
        {
            Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFXWithVolume(shotgunReloadStart, 0.3f);
            yield return new WaitForSeconds(0.3f); // wait for the sound effect to play
        }

        reloadingText.SetActive(true);

        while (currentBulletCount < magSize)
        {
            AddBulletToTheMag();

            if (isShotgun)
            {
                Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFXWithVolume(shotgunReloadOne, 0.3f);
            }

            cursor.SetTrigger("Shoot");

            yield return new WaitForSeconds(reloadTime / magSize);
        }

        if (isShotgun)
        {
            Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFXWithVolume(shotgunReloadEnd, 0.3f);
        }

        reloadingText.SetActive(false);
        reloading = false;
    }

    public void ReloadShotgunOnNewRoundInstantly()
    {
        while (currentBulletCount < magSize)
        {
            AddBulletToTheMag();
        }

        if (isShotgun)
        {
            Instantiate(sfxPrefab, transform.position, Quaternion.identity).GetComponent<SFXPlayer>().PlaySFXWithVolume(shotgunReloadEnd, 0.3f);
        }
    }

    private void DestroyAmmoDisplay()
    {
        int childCount = bulletUIParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject child = bulletUIParent.transform.GetChild(i).gameObject;
            Destroy(child);
        }
    }

    private void DestroyAnAmmo()
    {
        Destroy(bulletUIParent.transform.GetChild(0).gameObject);
    }
}
