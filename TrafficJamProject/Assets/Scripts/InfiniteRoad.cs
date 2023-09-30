using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteRoad : MonoBehaviour
{
    Player player;
    [SerializeField] GameObject RoadTexture;

    Vector3 startPosition;
    Vector3 startDirection;
    [SerializeField] float roadLength;
    [SerializeField] float roadWidth;

    [SerializeField] int maxRoads;

    [SerializeField] LinkedList<GameObject> activeRoads;
    [SerializeField] int startOfRoadIndex = 0;
    [SerializeField] int endOfRoadIndex = 0;

    [SerializeField] float spawnDistance;

    [SerializeField] int numLanes;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        activeRoads = new LinkedList<GameObject>();
        startPosition = transform.position;
        startDirection = transform.up;
        InstantiateStartingRoad();
    }

    void FixedUpdate()
    {
        CheckForNewRoads();
    }

    void CheckForNewRoads()
    {
        float playerRoadProjection = Vector3.Dot(player.transform.position - startPosition, startDirection);
        if (playerRoadProjection > (roadLength * endOfRoadIndex - spawnDistance))
        {
            // We're close enough to the end to add a new road
            AddRoadToEnd();
        }
        if (playerRoadProjection < (roadLength * startOfRoadIndex + spawnDistance))
        {
            // We're close enough to the end to add a new road
            AddRoadToStart();
        }
    }

    void InstantiateStartingRoad()
    {
        GameObject firstRoad = InstantiateRoad(startPosition);
        activeRoads.AddLast(firstRoad);
    }

    void AddRoadToEnd()
    {
        int nextIndex = endOfRoadIndex + 1;
        Vector3 nextPosition = startPosition + startDirection * roadLength * nextIndex;
        GameObject nextRoad = InstantiateRoad(nextPosition);
        activeRoads.AddLast(nextRoad);
        endOfRoadIndex++;
        if (activeRoads.Count > maxRoads)
        {
            RemoveFromStart();
        }
    }

    void AddRoadToStart()
    {
        int nextIndex = startOfRoadIndex - 1;
        Vector3 nextPosition = startPosition + startDirection * roadLength * nextIndex;
        GameObject nextRoad = InstantiateRoad(nextPosition);
        activeRoads.AddFirst(nextRoad);
        startOfRoadIndex--;
        if (activeRoads.Count > maxRoads)
        {
            RemoveFromEnd();
        }
    }

    void RemoveFromStart()
    {
        GameObject roadToRemove = activeRoads.First.Value;
        activeRoads.RemoveFirst();
        Destroy(roadToRemove);
        startOfRoadIndex++;
    }

    void RemoveFromEnd()
    {
        GameObject roadToRemove = activeRoads.Last.Value;
        activeRoads.RemoveLast();
        Destroy(roadToRemove);
        endOfRoadIndex--;
    }

    GameObject InstantiateRoad(Vector3 position)
    {
        GameObject newRoad = Instantiate<GameObject>(RoadTexture);
        newRoad.transform.localScale = new Vector3(roadWidth, roadLength, 1);
        newRoad.transform.position = position;
        newRoad.transform.up = startDirection;
        return newRoad;
    }
}
