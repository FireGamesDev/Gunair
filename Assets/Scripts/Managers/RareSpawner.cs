using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RareSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawnRarely;

    [SerializeField] private float chance = 1f;

    [Header("Fish")]
    [SerializeField] private bool isFish = false;
    [SerializeField] private GameObject target;

    [SerializeField] private PhotonView _pv;

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), 0f, 10f);
    }

    private void Spawn()
    {
        if (Random.value <= chance)
        {
            if (PhotonNetwork.InRoom)
            {
                _pv.RPC(nameof(SpawnRPC), RpcTarget.All);
            }
            else
            {
                SpawnRPC();
            }
        }
    }

    [PunRPC]
    private void SpawnRPC()
    {
        var go = Instantiate(objectToSpawnRarely, transform);
        if (isFish)
        {
            go.GetComponent<DuckHuntTarget>().target = target.transform.position;
        }
    }
}
