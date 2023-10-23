using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ClayWarsDiscSpawner : MonoBehaviour
{
    [SerializeField] private GameObject disc;
    [SerializeField] private bool isBouncy = false;
    [SerializeField] private GameObject camHelper;
    [SerializeField] private GameObject fromLeftIconUI;
    [SerializeField] private GameObject fromRightIconUI;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private List<Transform> _leftSpawnpoints = new List<Transform>();
    [SerializeField] private List<Transform> _rightSpawnpoints = new List<Transform>();

    [SerializeField] private float spawnTime = 2f;
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float curveAmount = 2f;

    [SerializeField] private int discNumberForTheRoundMin = 3;
    [SerializeField] private int discNumberForTheRoundMax = 8;

    [Header("Multiplayer")]
    [SerializeField] private PhotonView _pv;

    public int discNumberForTheRound { get; private set; } = 4;


    public static ClayWarsDiscSpawner Instance;

    public int currentPlayerIndexToGiveScoreTo { get; set; } = 0;

    public int currentDiscCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NewRound();

        StartCoroutine(SpawnerRoutine());
    }

    private IEnumerator SpawnerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1.2f, spawnTime));

            while (currentDiscCount > 0)
            {
                yield return new WaitForSeconds(1);
            }

            if (discNumberForTheRound <= 0)
            {
                ClayWarsRoundManager.Instance.NextPlayer();

                ClayWarsRoundManager.Instance.NextRound();

                int playerCount;

                if (PhotonNetwork.InRoom)
                {
                    playerCount = PhotonNetwork.PlayerList.Length;
                }
                else
                {
                    playerCount = ClayWarsGameManager.playerCount;
                }

                if (playerCount > 1)
                {
                    yield return new WaitForSeconds(5f);

                    currentPlayerIndexToGiveScoreTo = ClayWarsRoundManager.Instance.currentPlayerIndexInRound;
                }
                else
                {
                    yield return new WaitForSeconds(1.5f);
                }

                NewRound();
            }

            if (PhotonNetwork.InRoom)
            {
                while (MultiplayerGameManager.Instance.isEnded) //stop it
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                while (ClayWarsGameManager.Instance.isEnded) //stop it
                {
                    yield return new WaitForSeconds(1f);
                }
            }


            if (!PhotonNetwork.InRoom)
            {
                if (Random.value < 0.5f)
                {
                    yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));
                }
            }

            bool toLeft = Random.value < 0.5f;

            fromLeftIconUI.SetActive(toLeft);

            fromRightIconUI.SetActive(!toLeft);

            yield return new WaitForSeconds(3.5f);

            if (Random.value < 0.5f)
            {
                SpawnDiscAndLaunch(toLeft);
            }
            else
            {
                //spawnd double with delay
                SpawnDiscAndLaunch(toLeft);
                yield return new WaitForSeconds(1f);
                SpawnDiscAndLaunch(toLeft);
            }

            yield return new WaitForSeconds(0.3f);

            QuickShot.Instance.StartTimer();
        }
    }

    private void SpawnDiscAndLaunch(bool toLeft)
    {
        discNumberForTheRound--;

        if (discNumberForTheRound <= -1) return;

        currentDiscCount++;

        if (toLeft)
        {
            var spawnpos = _leftSpawnpoints[Random.Range(0, _leftSpawnpoints.Count)];

            if (PhotonNetwork.InRoom)
            {
                if (MultiplayerGameManager.GetLocalPlayerIndex() == ClayWarsRoundManager.Instance.currentPlayerIndexInRound)
                {
                    _pv.RPC(nameof(SpawnDiscRPC), RpcTarget.All, spawnpos.position, toLeft, discNumberForTheRound);
                } 
            }
            else
            {
                SpawnDiscRPC(spawnpos.position, toLeft, discNumberForTheRound);
            }
        }
        else
        {
            var spawnpos = _rightSpawnpoints[Random.Range(0, _rightSpawnpoints.Count)];

            if (PhotonNetwork.InRoom)
            {
                if (MultiplayerGameManager.GetLocalPlayerIndex() == ClayWarsRoundManager.Instance.currentPlayerIndexInRound)
                {
                    _pv.RPC(nameof(SpawnDiscRPC), RpcTarget.All, spawnpos.position, toLeft, discNumberForTheRound);
                }
            }
            else
            {
                SpawnDiscRPC(spawnpos.position, toLeft, discNumberForTheRound);
            }
        }
    }

    [PunRPC]
    private void SpawnDiscRPC(Vector3 spawnpos, bool toLeft, int discNumberForTheRound)
    {
        GameObject discGo;
        if (!isBouncy)
        {
            discGo = Instantiate(disc, spawnpos, Quaternion.identity);
        }
        else
        {
            discGo = Instantiate(disc, spawnpos, Quaternion.Euler(90, 0, 0));
        }
        if (!isBouncy) ThrowObject(discGo.GetComponent<Rigidbody>(), !toLeft);
        else ThrowBouncyObject(discGo.GetComponent<Rigidbody>(), !toLeft);

        StartCoroutine(DelayAndThenTransitionCamera(discGo));

        Destroy(discGo, lifeTime);

        this.discNumberForTheRound = discNumberForTheRound;
    }

    private IEnumerator DelayAndThenTransitionCamera(GameObject discGo)
    {
        yield return new WaitForSeconds(1.1f);

        //var camTargetHelper = Instantiate(camHelper, spawnpos, Quaternion.identity);
        //camTargetHelper.GetComponent<CamTargetHelper>().SetHelper(discGo.transform);

        //cam.m_LookAt = camTargetHelper.transform;

        DynamicCamera.Instance.currentTarget = discGo.transform;
    }

    private void ThrowObject(Rigidbody rb, bool isRight)
    {
        // Calculate the curve based on the throw direction (left or right)
        float curve = Random.Range(0, curveAmount);

        // Calculate the force vector
        Vector3 force = rb.gameObject.transform.right * throwForce;

        if (isRight) force.x *= -1;

        force.y = curve;

        // Apply the force to the Rigidbody
        rb.AddForce(force, ForceMode.Impulse);
    }
    
    private void ThrowBouncyObject(Rigidbody rb, bool isRight)
    {
        // Calculate the force vector
        Vector3 force = rb.gameObject.transform.right * throwForce;

        if (isRight) force.x *= -1;

        rb.gameObject.GetComponent<BouncyClaySideMovement>().SetDirection(isRight);

        // Apply the force to the Rigidbody
        //rb.AddForce(force, ForceMode.Impulse);
    }

    public void NewRound()
    {
        discNumberForTheRound = Random.Range(discNumberForTheRoundMin, discNumberForTheRoundMax);
    }
}
