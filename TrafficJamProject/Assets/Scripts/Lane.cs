using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public List<RoadPoint> points = new();

    [SerializeField] GameObject roadIndicatorPrefab;
    [SerializeField] GameObject roadLinePrefab;

    [SerializeField] float generationSpeed;

    [SerializeField] float minHorizontal, maxHorizontal;
    [SerializeField] float minVertical, maxVertical;
    [SerializeField] float roadScale;

    public RoadData road;

    private void Awake()
    {
        road = transform.parent.GetComponent<RoadData>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AddNewLanePoint), 0, generationSpeed);
        InvokeRepeating(nameof(CullFarPoints), 5, 5);
    }

    public Vector3 GetClosestPoint(Vector3 startPos, out int index)
    {
        if (points.Count == 0)
        {
            index = 0;
            return Vector3.zero;
        }

        Vector3 closestPoint = points[0].transform.position;
        float closestDistance = Vector3.Distance(points[0].transform.position, startPos);

        int newIndex = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            float distance = Vector3.Distance(points[i].transform.position, startPos);

            if (distance < closestDistance)
            {
                newIndex = i;

                closestPoint = points[i].transform.position;
                closestDistance = distance;
            }
        }
        index = newIndex;
        return closestPoint;
    }

    public Vector3 CalculateHorizontalIntersection(Vector3 pointA, int pointBIndex, Vector3 originPoint)
    {
        if(pointBIndex == 0 && points.Count <= 0)
            return Vector3.zero;

        if (pointA.y != points[pointBIndex].transform.position.y)
        {
            return Vector3.zero;
        }

        float intersectionX = Mathf.Clamp(originPoint.x, Mathf.Min(pointA.x, points[pointBIndex].transform.position.x), Mathf.Max(pointA.x, points[pointBIndex].transform.position.x));
        Vector3 intersectionPoint = new Vector3(intersectionX, originPoint.y, 0f);

        return intersectionPoint;
    }

    void AddNewLanePoint()
    {
        if(points.Count <= 0)
        {
            points.Add(Instantiate(roadIndicatorPrefab, transform.position, Quaternion.identity, transform).GetComponent<RoadPoint>());
            return;
        }

        Vector3 startPoint = points[points.Count - 1].transform.position;
        Vector3 increment = new(Random.Range(minHorizontal, maxHorizontal), Random.Range(minVertical, maxVertical));
        Vector3 newEndPoint = startPoint + increment;

        if (Vector2.Distance((Vector2)Camera.main.transform.position, newEndPoint) > 50f)
            return;

        points.Add(Instantiate(roadIndicatorPrefab, newEndPoint, Quaternion.identity, transform).GetComponent<RoadPoint>());

        /*Vector3 newEndPointDir = (newEndPoint - startPoint).normalized;
        Vector3 midPos = startPoint + (newEndPoint - startPoint) / 2;
        float angle = (Mathf.Atan2(newEndPointDir.y, newEndPointDir.x) * Mathf.Rad2Deg) - 90;
        GameObject line = Instantiate(roadLinePrefab, midPos, Quaternion.identity, transform);
        line.transform.eulerAngles = new(0, 0, angle);
        float dist = Vector3.Distance(startPoint, newEndPoint);
        line.transform.localScale = new Vector3(.1f, dist, 1);*/
    }

    void CullFarPoints()
    {
        for(int i = points.Count - 1; i >= 0; i--)
        {
            Transform point = points[i].transform;
            if(Camera.main.transform.position.y - point.position.y >= 20)
            {
                Destroy(point.gameObject);
                points.RemoveAt(i);
            }
        }
    }
}
