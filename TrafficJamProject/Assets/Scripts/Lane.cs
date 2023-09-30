using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public List<Vector2> points = new();

    [SerializeField] GameObject roadSegment;

    [SerializeField] float generationSpeed;

    [SerializeField] float minHorizontal, maxHorizontal;
    [SerializeField] float minVertical, maxVertical;
    [SerializeField] float roadScale;

    private void Start()
    {
        InvokeRepeating(nameof(AddNewLanePoint), 0, generationSpeed);
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

        Vector3 startPoint = points[points.Count - 1];
        Vector3 increment = new(Random.Range(minHorizontal, maxHorizontal), Random.Range(minVertical, maxVertical));
        Vector3 newEndPoint = startPoint + increment;
        points.Add(newEndPoint);

        Vector3 midPos = startPoint + (newEndPoint - startPoint) / 2;

        
        GameObject segment = Instantiate(roadSegment, midPos, Quaternion.identity, transform);

        Vector3 newEndPointDir = (newEndPoint - startPoint).normalized;
        float angle = (Mathf.Atan2(newEndPointDir.y, newEndPointDir.x) * Mathf.Rad2Deg) - 90;
        
        segment.transform.eulerAngles = new(0, 0, angle);

        float dist = Vector3.Distance(startPoint, newEndPoint);
        segment.transform.localScale = new Vector3(roadScale, dist, 1);
    }
}
