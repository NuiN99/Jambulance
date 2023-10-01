using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] RoadData roadData;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] float startSpawnCount = 25;
    [SerializeField] float spawnInterval;

    [SerializeField] float minSpeedIncrement = 0f;
    [SerializeField] float maxSpeedIncrement = 0f;

    Enemy SpawnEnemy()
    {
        var lane = roadData.lanes[Random.Range(0, roadData.lanes.Count)];

        float randomPosUp = Random.Range(10f, 15f);
        float randomPosDown = Random.Range(-15f, -10f);
        float randY = Random.Range(0, 2) == 0? randomPosDown : randomPosUp;

        Vector3 closestPoint = lane.GetClosestPoint(Camera.main.transform.position, out int index);
        Vector3 randomYPos = closestPoint + new Vector3(0, randY);

        Vector3 spawnPoint = lane.CalculateHorizontalIntersection(closestPoint, index, randomYPos);

        Physics2D.queriesStartInColliders = true;
        RaycastHit2D spawnHit = Physics2D.CircleCast(spawnPoint, .75f, Vector3.forward);
        if (spawnHit)
        {
            print("Enemy will spawn in collider, cancelling spawn");
            return null;
        }
        Physics2D.queriesStartInColliders = false;


        Enemy enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity).GetComponent<Enemy>();
        enemy.currentLane = lane;
        enemy.roadData = roadData;

        float dirAngle = (Mathf.Atan2(roadData.direction.y, roadData.direction.x) * Mathf.Rad2Deg) - 90;
        enemy.transform.eulerAngles = new Vector3(0, 0, dirAngle);

        if(enemy.TryGetComponent(out Car enemyCar))
        {
            enemyCar.startSpeed = enemyCar.stats.moveSpeed;
            enemyCar.startSpeed += Random.Range(minSpeedIncrement, maxSpeedIncrement);
            enemyCar.targetSpeed = enemyCar.startSpeed;
        }
        return enemy;
    }

    private void Start()
    {

        SpawnStartEnemies();
        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }

    void SpawnStartEnemies()
    {
        for (int i = 0; i < startSpawnCount; i++)
        {
            Enemy enemy = SpawnEnemy();
            if (enemy == null)
                continue;
            if (enemy.roadData.direction != Vector2.up)
            {
                Destroy(enemy.gameObject);
            }

            Vector2 camPos = Camera.main.transform.position;
            Vector2 randomPosInView = new Vector2(Random.Range(camPos.x - 4, camPos.x + 4), Random.Range(camPos.y - 10, camPos.y + 10));
            enemy.transform.position = randomPosInView;

            if (Vector3.Distance(enemy.transform.position, camPos) < 1.5f)
            {
                Destroy(enemy.gameObject);
                return;
            }

            float dist = 999;
            foreach (Lane lane in FindObjectsOfType<Lane>())
            {
                if (lane.road.direction != Vector2.up)
                    continue;

                float newDist = Vector3.Distance(lane.GetClosestPoint(enemy.transform.position, out _), enemy.transform.position);
                if (newDist < dist)
                {
                    dist = newDist;
                    enemy.currentLane = lane;
                }
            }

            float dirAngle = (Mathf.Atan2(roadData.direction.y, roadData.direction.x) * Mathf.Rad2Deg) - 90;
            enemy.transform.eulerAngles = new Vector3(0, 0, dirAngle);
        }
    }
}
