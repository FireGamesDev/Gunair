using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceInvaderSpawner : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform spawnpoint;
    [SerializeField] private List<GameObject> targets = new List<GameObject>();
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private GameObject team;
    [SerializeField] private float spawnTime;
    [SerializeField] private bool isVertical;
    [SerializeField] private bool isLeft;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (!gameManager.isEnded)
        {
            yield return new WaitForSeconds(spawnTime);

            bool spawnTeam = Random.value < 0.4f && team != null;

            if (spawnTeam)
            {
                GameObject enemy = Instantiate(team, spawnpoint.position, Quaternion.identity);
                enemy.GetComponent<InvaderEnemyVerticalMovement>().enabled = true;

                enemies.Add(enemy);
            }
            else
            {
                GameObject enemy = Instantiate(targets[Random.Range(0, targets.Count)], spawnpoint.position, Quaternion.identity);

                enemies.Add(enemy);

                if (!isVertical)
                {
                    enemy.GetComponent<InvaderEnemyHorizontalMovement>().moveLeft = isLeft;
                    enemy.GetComponent<InvaderEnemyHorizontalMovement>().enabled = true;
                }
                else
                {
                    enemy.GetComponent<InvaderEnemyVerticalMovement>().enabled = true;
                }
            }
        }

        DeleteEnemyObjects();
    }

    private void DeleteEnemyObjects()
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

}
