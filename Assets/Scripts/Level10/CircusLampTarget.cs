using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircusLampTarget : MonoBehaviour
{
    [SerializeField] private GameObject goodTarget;
    [SerializeField] private GameObject badTarget;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private List<SpriteRenderer> lamps = new List<SpriteRenderer>();
    [SerializeField] private float minCycleTime = 2.5f;
    [SerializeField] private float maxCycleTime = 4f;

    private bool isCycleActive = false;
    private GameObject currentTarget;

    private void Start()
    {
        ResetLampsColor();

        StartCycle();
    }

    private void StartCycle()
    {
        if (isCycleActive)
            return;

        isCycleActive = true;
        StartCoroutine(GenerateCycle());
    }

    private IEnumerator GenerateCycle()
    {
        float cycleTime = Random.Range(minCycleTime, maxCycleTime); // Randomize the cycle time

        while (isCycleActive)
        {
            // Activate lamps one by one with time intervals
            float interval = cycleTime / lamps.Count;
            for (int i = 0; i < lamps.Count; i++)
            {
                yield return new WaitForSeconds(interval);
                lamps[i].color = GetLampColor(i);

                lamps[i].GetComponent<Animator>().enabled = true;
            }

            Spawn();

            while (currentTarget != null)
            {
                yield return null;
            }

            cycleTime = Random.Range(minCycleTime, maxCycleTime); // Randomize the cycle time

            ResetLampsColor();
        }
    }

    private void Spawn()
    {
        if (currentTarget != null)
            Destroy(currentTarget);

        if (Random.value < 0.6f)
        {
            currentTarget = Instantiate(goodTarget, spawnPos.position, Quaternion.identity);
        }
        else
        {
            currentTarget = Instantiate(badTarget, spawnPos.position, Quaternion.identity);
        }

        Destroy(currentTarget, Random.Range(1f, 1.7f));
    }

    private void SetLampColors(Color color)
    {
        foreach (SpriteRenderer lamp in lamps)
        {
            lamp.color = color;
        }
    }

    private Color GetLampColor(int index)
    {
        if (index < 6)
        {
            return Color.green;
        }
        else if (index < 9)
        {
            return Color.yellow;
        }
        else
        {
            return Color.red;
        }
    }

    private void ResetLampsColor()
    {
        SetLampColors(Color.gray);

        foreach (var lamp in lamps)
        {
            lamp.GetComponent<Animator>().enabled = false;
        }
    }
}
