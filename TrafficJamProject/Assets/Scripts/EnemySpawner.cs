using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] RoadData roadData;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] float startSpawnMult = 30f;
    [SerializeField] float spawnInterval;

    void SpawnEnemy()
    {
        var lane = roadData.lanes[Random.Range(0, roadData.lanes.Count)];

        float randomPosUp = Random.Range(10f, 15f);
        float randomPosDown = Random.Range(-15f, -10f);
        float randY = Random.Range(0, 2) == 0? randomPosDown : randomPosUp;

        Vector3 closestPoint = lane.GetClosestPoint(Camera.main.transform.position, out int index);
        Vector3 randomYPos = closestPoint + new Vector3(0, randY);

        Vector3 spawnPoint = lane.CalculateHorizontalIntersection(closestPoint, index, randomYPos);

        Enemy enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity).GetComponent<Enemy>();
        enemy.currentLane = lane;
        enemy.roadData = roadData;
    }

    private void Start()
    {
        for (int i = 0; i < roadData.lanes.Count * startSpawnMult; i++)
        {
            SpawnEnemy();
        }

        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }
}
