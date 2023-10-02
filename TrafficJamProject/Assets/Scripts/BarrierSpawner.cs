using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpawner : MonoBehaviour
{
    [SerializeField] GameObject barrier;
    [SerializeField] float barrierChance;

    Player player;

    Vector3 startPosition;

    [SerializeField] float barrierLength;

    [SerializeField] int maxBarriers;

    [SerializeField] LinkedList<GameObject> activeBarriers;
    int startOfBarrierIndex = 0;
    int endOfBarrierIndex = 0;

    [SerializeField] float spawnDistance;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        activeBarriers = new LinkedList<GameObject>();
        startPosition = transform.position;
        InstantiateStartingBarrier();
    }

    void FixedUpdate()
    {
        CheckForNewBarriers();
    }

    void CheckForNewBarriers()
    {
        float playerBarrierProjection = Vector3.Dot(player.transform.position - startPosition, transform.up);
        if (playerBarrierProjection > (barrierLength * endOfBarrierIndex - spawnDistance))
        {
            // We're close enough to the end to add a new barrier
            TryAddBarrierToEnd();
        }
        if (playerBarrierProjection < (barrierLength * startOfBarrierIndex + spawnDistance))
        {
            // We're close enough to the end to add a new barrier
            TryAddBarrierToStart();
        }
    }

    void InstantiateStartingBarrier()
    {
        GameObject firstBarrier = InstantiateBarrier(startPosition);
        activeBarriers.AddLast(firstBarrier);
    }

    void TryAddBarrierToEnd()
    {
        int nextIndex = endOfBarrierIndex + 1;
        Vector3 nextPosition = startPosition + transform.up * barrierLength * nextIndex;
        endOfBarrierIndex++;
        if (Random.Range(0f, 1f) > barrierChance) return; // Don't spawn sometimes
        GameObject nextBarrier = InstantiateBarrier(nextPosition);
        activeBarriers.AddLast(nextBarrier);
        if (activeBarriers.Count > maxBarriers)
        {
            RemoveFromStart();
        }
    }

    void TryAddBarrierToStart()
    {
        int nextIndex = startOfBarrierIndex - 1;
        Vector3 nextPosition = startPosition + transform.up * barrierLength * nextIndex;
        startOfBarrierIndex--;
        if (Random.Range(0f, 1f) > barrierChance) return; // Don't spawn sometimes
        GameObject nextBarrier = InstantiateBarrier(nextPosition);
        activeBarriers.AddFirst(nextBarrier);
        if (activeBarriers.Count > maxBarriers)
        {
            RemoveFromEnd();
        }
    }

    void RemoveFromStart()
    {
        GameObject barrierToRemove = activeBarriers.First.Value;
        activeBarriers.RemoveFirst();
        Destroy(barrierToRemove);
        startOfBarrierIndex++;
    }

    void RemoveFromEnd()
    {
        GameObject barrierToRemove = activeBarriers.Last.Value;
        activeBarriers.RemoveLast();
        Destroy(barrierToRemove);
        endOfBarrierIndex--;
    }

    GameObject InstantiateBarrier(Vector3 position)
    {
        GameObject newBarrier = Instantiate<GameObject>(barrier);
        newBarrier.transform.position = position;
        newBarrier.transform.up = transform.up;
        return newBarrier;
    }
}
