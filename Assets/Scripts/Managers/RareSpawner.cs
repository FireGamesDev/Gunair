using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawnRarely;

    [SerializeField] private float chance = 1f;

    [Header("Fish")]
    [SerializeField] private bool isFish = false;
    [SerializeField] private GameObject target;

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), 0f, 10f);
    }

    private void Spawn()
    {
        if (Random.value <= chance)
        {
            var go = Instantiate(objectToSpawnRarely, transform);
            if (isFish)
            {
                go.GetComponent<DuckHuntTarget>().target = target.transform.position;
            }
            
        }
    }
}
