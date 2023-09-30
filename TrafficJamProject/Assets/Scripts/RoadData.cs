using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadData : MonoBehaviour
{
    public PathCreator[] roads;

    private void Awake()
    {
        roads = GetComponentsInChildren<PathCreator>();
    }
}
