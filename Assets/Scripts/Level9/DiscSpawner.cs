using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscSpawner : MonoBehaviour
{
    [SerializeField] private GameObject disc;
    [SerializeField] private List<Transform> _leftSpawnpoints = new List<Transform>();
    [SerializeField] private List<Transform> _rightSpawnpoints = new List<Transform>();

    [SerializeField] private float spawnTime = 2f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float curveAmount = 2f;

    private void Start()
    {
        StartCoroutine(SpawnerRoutine());
    }

    private IEnumerator SpawnerRoutine()
    {
        yield return new WaitForSeconds(Random.Range(1.2f, spawnTime));

        if (Random.value < 0.4f)
        {
            yield return new WaitForSeconds(0.5f);
        }

        SpawnDiscAndLaunch();

        StartCoroutine(SpawnerRoutine());
    }

    private void SpawnDiscAndLaunch()
    {
        if(Random.value < 0.5f)
        {
            var discGo = Instantiate(disc, _leftSpawnpoints[Random.Range(0, _leftSpawnpoints.Count)].position, Quaternion.identity);
            ThrowObject(discGo.GetComponent<Rigidbody>(), false);
        }
        else
        {
            var discGo = Instantiate(disc, _rightSpawnpoints[Random.Range(0, _rightSpawnpoints.Count)].position, Quaternion.identity);
            ThrowObject(discGo.GetComponent<Rigidbody>(), true);
        }
    }

    private void ThrowObject(Rigidbody rb, bool isRight)
    {
        // Calculate the curve based on the throw direction (left or right)
        float curve = Random.Range(0, curveAmount);

        // Calculate the force vector
        Vector3 force = new Vector3(1f, 0f, 0f) * throwForce;

        if (isRight) force.x *= -1;

        force.y = curve;

        // Apply the force to the Rigidbody
        rb.AddForce(force, ForceMode.Impulse);
    }
}
