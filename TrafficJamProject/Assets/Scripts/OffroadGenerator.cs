using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;

public class OffroadGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> segmentsList = new();

    Player player;

    Vector3 startPosition;

    [SerializeField] float segmentLength;

    [SerializeField] int maxSegments;

    [SerializeField] float interval = 0.2f;

    [SerializeField] LinkedList<GameObject> activeSegments;
    int startOfSegmentIndex = 0;
    int endOfSegmentIndex = 0;

    [SerializeField] float spawnDistance;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        activeSegments = new LinkedList<GameObject>();
        startPosition = transform.position;
        InstantiateStartingSegment();

        InvokeRepeating(nameof(CheckForNewSegments), interval, interval);
    }

    void CheckForNewSegments()
    {
        float playerSegmentProjection = Vector3.Dot(player.transform.position - startPosition, transform.up);
        if (playerSegmentProjection > (segmentLength * endOfSegmentIndex - spawnDistance))
        {
            // We're close enough to the end to add a new segment
            AddSegmentToEnd();
        }
        if (playerSegmentProjection < (segmentLength * startOfSegmentIndex + spawnDistance))
        {
            // We're close enough to the end to add a new segment
            AddSegmentToStart();
        }
    }

    void InstantiateStartingSegment()
    {
        GameObject firstSegment = InstantiateSegment(startPosition, 0);
        activeSegments.AddLast(firstSegment);
    }

    void AddSegmentToEnd()
    {
        int nextIndex = endOfSegmentIndex + 1;
        Vector3 nextPosition = startPosition + transform.up * segmentLength * nextIndex;
        GameObject nextSegment = InstantiateSegment(nextPosition, nextIndex);
        activeSegments.AddLast(nextSegment);
        endOfSegmentIndex++;
        if (activeSegments.Count > maxSegments)
        {
            RemoveFromStart();
        }
    }

    void AddSegmentToStart()
    {
        int nextIndex = startOfSegmentIndex - 1;
        if (nextIndex <= 0) return;

        Vector3 nextPosition = startPosition + transform.up * segmentLength * nextIndex;
        GameObject nextSegment = InstantiateSegment(nextPosition, nextIndex);
        activeSegments.AddFirst(nextSegment);
        startOfSegmentIndex--;
        if (activeSegments.Count > maxSegments)
        {
            RemoveFromEnd();
        }
    }

    void RemoveFromStart()
    {
        GameObject segmentToRemove = activeSegments.First.Value;
        activeSegments.RemoveFirst();
        Destroy(segmentToRemove);
        startOfSegmentIndex++;
    }

    void RemoveFromEnd()
    {
        GameObject segmentToRemove = activeSegments.Last.Value;
        activeSegments.RemoveLast();
        Destroy(segmentToRemove);
        endOfSegmentIndex--;
    }

    GameObject InstantiateSegment(Vector3 position, int index)
    {
        GameObject newSegment = Instantiate<GameObject>(segmentsList[seededRandomInt(index) % segmentsList.Count]);
        newSegment.transform.position = position;
        newSegment.transform.up = transform.up;

        int randScale = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;
        int randScaleY = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;
        newSegment.transform.localScale = new Vector3(newSegment.transform.localScale.x * randScale, newSegment.transform.localScale.y * randScaleY, newSegment.transform.localScale.z);

        return newSegment;
    }

    static int seededRandomInt(int seed)
    {
        System.Random rand = new System.Random(seed);
        return rand.Next(seed);
    }
}
