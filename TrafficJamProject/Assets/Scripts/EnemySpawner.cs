using PathCreation;
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
        var lane = roadData.roads[Random.Range(0, roadData.roads.Length)];

        Vector3 closestPoint = lane.path.GetClosestPointOnPath(Camera.main.transform.position);
        Vector3 randomPoint = lane.path.GetPointAtDistance(Vector3.Distance(lane.path.GetPoint(0), closestPoint) + Random.Range(10, 30));

        Enemy enemy = Instantiate(enemyPrefab, randomPoint, Quaternion.identity).GetComponent<Enemy>();
        enemy.lane = lane;
    }

    private void Start()
    {
        for(int i = 0; i < roadData.roads.Length * startSpawnMult; i++)
        {
            SpawnEnemy();
        }

        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }
}
