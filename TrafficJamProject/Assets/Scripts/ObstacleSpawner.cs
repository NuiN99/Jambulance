using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] RoadData roadData;
    [SerializeField] GameObject[] obstaclePrefabs;

    [SerializeField] float minInterval;
    [SerializeField] float maxInterval;

    private void Start()
    {
        StartCoroutine(SpawnObstacles());
    }

    IEnumerator SpawnObstacles()
    {
        yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Lane lane = roadData.lanes[Random.Range(0, roadData.lanes.Count)];
        RoadPoint point = lane.points[lane.points.Count - 1];

        if (!point.occupied)
        {
            point.occupied = true;
            Instantiate(prefab, point.transform.position, Quaternion.identity);
        }

        StartCoroutine(SpawnObstacles());
    }
}
