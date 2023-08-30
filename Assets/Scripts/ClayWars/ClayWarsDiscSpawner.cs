using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayWarsDiscSpawner : MonoBehaviour
{
    [SerializeField] private GameObject disc;
    [SerializeField] private GameObject fromLeftIconUI;
    [SerializeField] private GameObject fromRightIconUI;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private List<Transform> _leftSpawnpoints = new List<Transform>();
    [SerializeField] private List<Transform> _rightSpawnpoints = new List<Transform>();

    [SerializeField] private float spawnTime = 2f;
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float curveAmount = 2f;

    private void Start()
    {
        StartCoroutine(SpawnerRoutine());
    }

    private IEnumerator SpawnerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1.2f, spawnTime));

            if (Random.value < 0.4f)
            {
                yield return new WaitForSeconds(0.5f);
            }

            bool toLeft = Random.value < 0.5f;

            fromLeftIconUI.SetActive(toLeft);

            fromRightIconUI.SetActive(!toLeft);

            yield return new WaitForSeconds(1.5f);

            SpawnDiscAndLaunch(toLeft);
        }
    }

    private void SpawnDiscAndLaunch(bool toLeft)
    {
        if (toLeft)
        {
            var discGo = Instantiate(disc, _leftSpawnpoints[Random.Range(0, _leftSpawnpoints.Count)].position, Quaternion.identity);
            ThrowObject(discGo.GetComponent<Rigidbody>(), false);

            cam.m_LookAt = discGo.transform;

            Destroy(discGo, lifeTime);
        }
        else
        {
            var discGo = Instantiate(disc, _rightSpawnpoints[Random.Range(0, _rightSpawnpoints.Count)].position, Quaternion.identity);
            ThrowObject(discGo.GetComponent<Rigidbody>(), true);

            cam.m_LookAt = discGo.transform;

            Destroy(discGo, lifeTime);
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
