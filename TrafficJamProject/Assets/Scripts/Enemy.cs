using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Cai;

public class Enemy : MonoBehaviour
{
    Car car;
    public PathCreator lane;
    [SerializeField] float targetOffset = 2.5f;

    private void Awake()
    {
        car = GetComponent<Car>();
    }

    private void FixedUpdate()
    {

        float dist = lane.path.GetClosestDistanceAlongPath(transform.position);
        Vector3 targetPoint = lane.path.GetPointAtDistance(dist + targetOffset);

        Debug.DrawLine(transform.position, targetPoint);
        if (car.colliding) return;

        car.MoveInDirection(transform.up);
        car.RotateToDirection(targetPoint, car.turnSpeed);
    }
}
