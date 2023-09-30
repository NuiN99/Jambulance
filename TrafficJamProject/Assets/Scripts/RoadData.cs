using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cai
{
    public class RoadData : MonoBehaviour
    {
        public List<PathCreator> roads = new();

        private void Awake()
        {
            roads = GetComponentsInChildren<PathCreator>().ToList();
        }

        public PathCreator ChangeLaneRandom(PathCreator currentLane)
        {
            int dir = Random.Range(0, 2) == 0 ? -1 : 1;
            return ChangeLane(currentLane, dir);
        }

        public PathCreator ChangeLane(PathCreator currentLane, int dir)
        {
            if (roads.Contains(currentLane))
            {
                if (roads.Count <= 1) return currentLane;

                int index = roads.IndexOf(currentLane);

                int lastIndex = roads.Count - 1;
                if (index == lastIndex && dir == 1 ||
                    index == 0 && dir == -1)
                {
                    return currentLane;
                }

                index += dir;
                return roads[index];
            }
            return currentLane;
        }
    }

}
