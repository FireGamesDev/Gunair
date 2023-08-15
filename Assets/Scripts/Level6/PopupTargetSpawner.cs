using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goodPrefab;
    [SerializeField] private GameObject badPrefab;
    [SerializeField] private float initialSpawnTime = 1.0f;
    [SerializeField] private float minSpawnTime = 0.5f;
    [SerializeField] private float spawnTimeDecreaseRate = 0.05f;
    [SerializeField] private List<Transform> spawnpoints = new List<Transform>();

    private float currentSpawnTime;

    private void OnEnable()
    {
        currentSpawnTime = initialSpawnTime;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnTime);

            // Randomly select a spawn point
            int rand = Random.Range(0, spawnpoints.Count);
            while (spawnpoints[rand].childCount > 0)
            {
                rand = Random.Range(0, spawnpoints.Count);
                yield return new WaitForSeconds(0.02f);
            }

            Transform spawnPoint = spawnpoints[rand];

            // Determine which prefab to spawn based on chance
            GameObject prefab = Random.value < 0.2f ? badPrefab : goodPrefab;

            // Spawn the prefab at the selected spawn point
            GameObject spawnedObject = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            spawnedObject.transform.parent = spawnPoint;

            float lifetime = currentSpawnTime * 14f;

            spawnedObject.GetComponent<PopupTargetScale>().StartAnim(lifetime);

            // Create a placeholder object as the parent
            GameObject placeholder = new GameObject("Placeholder");
            placeholder.transform.parent = spawnPoint;

            // Destroy the placeholder and spawned object after a certain time
            Destroy(placeholder, lifetime + 1f);
            Destroy(spawnedObject, lifetime);
            
            // Decrease the spawn time for the next spawn
            currentSpawnTime = Mathf.Max(minSpawnTime, currentSpawnTime - spawnTimeDecreaseRate);
        }
    }
}
