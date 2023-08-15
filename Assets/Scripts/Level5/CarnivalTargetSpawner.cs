using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivalTargetSpawner : MonoBehaviour
{
    public CarnivalTarget targetPrefab;
    public CarnivalTarget obstaclePrefab1;
    public CarnivalTarget obstaclePrefab2;
    public float spawnInterval = 3f;
    public float obstacleSpawnChance = 0.3f;

    [SerializeField] private Transform spawnpointLeft;
    [SerializeField] private Transform spawnpointRight;

    private Coroutine spawnerCoroutine;

    private void Start()
    {
        spawnerCoroutine = StartCoroutine(SpawnTargets());
    }

    private IEnumerator SpawnTargets()
    {
        while (true)
        {
            if (Random.value <= obstacleSpawnChance)
            {
                SpawnObstacle();
            }
            else
            {
                SpawnTarget();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnTarget()
    {
        CarnivalTarget target;
        if (Random.value <= 0.5f) //left
        {
            target = Instantiate(targetPrefab, spawnpointRight.transform.position, Quaternion.identity);
            target.SetDirection(true);
        }
        else //right
        {
            target = Instantiate(targetPrefab, spawnpointLeft.transform.position, Quaternion.identity);
            target.SetDirection(false);
        }
    }

    private void SpawnObstacle()
    {
        CarnivalTarget obstacle;
        if (Random.value <= 0.5f) //left
        {
            CarnivalTarget prefab = (Random.value <= 0.5f) ? obstaclePrefab1 : obstaclePrefab2;
            obstacle = Instantiate(prefab, spawnpointRight.transform.position, Quaternion.identity);
            obstacle.SetDirection(true);
        }
        else //right
        {
            CarnivalTarget prefab = (Random.value <= 0.5f) ? obstaclePrefab1 : obstaclePrefab2;
            obstacle = Instantiate(prefab, spawnpointRight.transform.position, Quaternion.identity);
            obstacle.SetDirection(false);
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
