using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckHunterSpawner : MonoBehaviour
{
    public GameObject targetPrefab1;
    public GameObject targetPrefab2;
    public GameObject targetPrefab3;
    public float spawnInterval = 3f;

    private Coroutine spawnerCoroutine;

    private void Start()
    {
        spawnerCoroutine = StartCoroutine(SpawnTargets());
    }

    private IEnumerator SpawnTargets()
    {
        while (true)
        {
            SpawnTarget();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnTarget()
    {
        if (Random.value <= 0.33f)
        {
            Instantiate(targetPrefab1, transform.position, Quaternion.identity);
        } else if (Random.value <= 0.66f)
        {
            Instantiate(targetPrefab2, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(targetPrefab3, transform.position, Quaternion.identity);
        }
    }

    private void OnDestroy()
    {
        if (spawnerCoroutine != null)
        {
            StopCoroutine(spawnerCoroutine);
        }
    }
}
