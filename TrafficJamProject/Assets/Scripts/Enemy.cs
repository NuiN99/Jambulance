using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Cai;

public class Enemy : MonoBehaviour
{
    Car car;

    [SerializeField] float maxTurnAngle;

    [SerializeField] PathCreator lane;

    private void FixedUpdate()
    {
        car = GetComponent<Car>();

        car.MoveInDirection(transform.up);

        Vector3 closestPoint = lane.path.GetClosestPointOnPath(transform.position);
        Vector3 targetPoint = lane.path.GetPointAtDistance(Vector3.Distance(lane.path.GetPoint(0), closestPoint) + 2);
        Debug.DrawLine(transform.position, targetPoint);
        car.RotateToDirection(targetPoint, car.turnSpeed);
    }
}
