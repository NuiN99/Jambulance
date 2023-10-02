using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] RoadData roadData;
    [SerializeField] List<GameObject> enemyPrefabs;

    [SerializeField] float spawnInterval;

    [SerializeField] float minSpeedIncrement = 0f;
    [SerializeField] float maxSpeedIncrement = 0f;

    CarsController carsController;

    [SerializeField] float spawnIntervalMin;
    [SerializeField] float spawnIntervalMax;

    [SerializeField] float rampTime;
    float startTime;
    float lastSpawnTime = 0f;

    private void Awake()
    {
        carsController = FindObjectOfType<CarsController>();
        startTime = Time.time;
    }

    Enemy SpawnEnemy()
    {
        var lane = roadData.lanes[Random.Range(0, roadData.lanes.Count)];

        float randomPosUp = Random.Range(12.5f, 17.5f);
        float randomPosDown = Random.Range(-15f, -10f);
        float randY = Random.Range(0, 2) == 0? randomPosDown : randomPosUp;

        if(roadData.direction == Vector2.down && randY == randomPosDown)
            return null;

        Vector3 closestPoint = lane.GetClosestPoint((Vector2)Camera.main.transform.position, out int index);
        Vector3 randomYPos = closestPoint + new Vector3(0, randY);

        Vector3 spawnPoint = lane.CalculateHorizontalIntersection(closestPoint, index, randomYPos);

        Physics2D.queriesStartInColliders = true;
        RaycastHit2D spawnHit = Physics2D.CircleCast(spawnPoint, .75f, Vector3.forward);
        if (spawnHit)
        {
            return null;
        }
        Physics2D.queriesStartInColliders = false;

        GameObject chosenPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Enemy enemy = Instantiate(chosenPrefab, spawnPoint, Quaternion.identity).GetComponent<Enemy>();
        enemy.currentLane = lane;
        enemy.roadData = roadData;

        float dirAngle = (Mathf.Atan2(roadData.direction.y, roadData.direction.x) * Mathf.Rad2Deg) - 90;
        enemy.transform.eulerAngles = new Vector3(0, 0, dirAngle);

        if(enemy.TryGetComponent(out Car enemyCar))
        {
            enemyCar.startSpeed = enemyCar.stats.moveSpeed;
            enemyCar.startSpeed += Random.Range(minSpeedIncrement, maxSpeedIncrement);
            enemyCar.targetSpeed = enemyCar.startSpeed;
            enemyCar.curAccel = enemyCar.stats.maxAcceleration;

            if(roadData.direction == Vector2.down)
            {
                carsController.SwitchStats(enemyCar, carsController.copStats);
                carsController.ResetSpriteSelection(enemyCar);
            }
                
        }
        return enemy;
    }

    private void Update()
    {
        if (GameController.Instance.started && (Time.time - lastSpawnTime) > CurrSpawnInterval())
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }
    //Returns the spawn interval given current ramp up time
    float CurrSpawnInterval()
    {
        return spawnIntervalMin * (1 - RampMultiplier()) + spawnIntervalMax * RampMultiplier();
    }

    //Returns the ramp multiplier [0, 1] based on how long we are along the ramp time
    float RampMultiplier()
    {
        float currTime = GameController.Instance.startTime - GameController.Instance.timeRemaining;
        return Mathf.Min(currTime / rampTime, 1f);
    }
}
