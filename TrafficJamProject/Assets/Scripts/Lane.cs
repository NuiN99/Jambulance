using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public List<Vector2> points = new();


    private void Start()
    {
        InvokeRepeating(nameof(AddNewLanePoint), 0, 0.5f);
    }

    public Vector3 GetClosestPoint(Vector3 startPos, out int index)
    {
        if (points.Count == 0)
        {
            index = 0;
            return Vector3.zero;
        }

        Vector3 closestPoint = points[0];
        float closestDistance = Vector3.Distance(points[0], startPos);

        int newIndex = 0;
        for (int i = 1; i < points.Count; i++)
        {
            float distance = Vector3.Distance(points[i], startPos);

            if (distance < closestDistance)
            {
                newIndex = i;

                closestPoint = points[i];
                closestDistance = distance;
            }
        }
        index = newIndex;
        return closestPoint;
    }

    public Vector3 CalculateHorizontalIntersection(Vector3 pointA, int pointBIndex, Vector3 originPoint)
    {
        if (pointA.y != points[pointBIndex].y)
        {
            return Vector3.zero;
        }

        float intersectionX = Mathf.Clamp(originPoint.x, Mathf.Min(pointA.x, points[pointBIndex].x), Mathf.Max(pointA.x, points[pointBIndex].x));
        Vector3 intersectionPoint = new Vector3(intersectionX, originPoint.y, 0f);

        return intersectionPoint;
    }

    void AddNewLanePoint()
    {
        if(points.Count <= 0)
        {
            points.Add(transform.position);
            return;
        }

        Vector3 endPoint = points[points.Count - 1];
        Vector3 increment = new(Random.Range(-1f, 1f), Random.Range(1f, 5f));
        Vector3 newPos = endPoint + increment;
        points.Add(newPos);
        GameObject test = new($"{points.Count - 1}");
        test.transform.position = newPos;
        test.transform.SetParent(transform);
    }
}
