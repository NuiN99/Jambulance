using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadData : MonoBehaviour
{
    public List<Lane> lanes = new();

    public Vector2 direction;

    private void Awake()
    {
        lanes = GetComponentsInChildren<Lane>().ToList();
    }

    public Lane ChangeLaneRandom(Lane currentLane)
    {
        int dir = Random.Range(0, 2) == 0 ? -1 : 1;
        return ChangeLane(currentLane, dir);
    }

    public Lane ChangeLane(Lane currentLane, int dir)
    {
        if (lanes.Contains(currentLane))
        {
            if (lanes.Count <= 1) return currentLane;

            int index = lanes.IndexOf(currentLane);

            int lastIndex = lanes.Count - 1;
            if (index == lastIndex && dir == 1 ||
                index == 0 && dir == -1)
            {
                return currentLane;
            }

            index += dir;
            return lanes[index];
        }
        return currentLane;
    }
}
