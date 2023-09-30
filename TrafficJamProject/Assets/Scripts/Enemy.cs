using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Car car;

    public Lane currentLane;

    [SerializeField] float targetOffset = 2.5f;

    [SerializeField] float fwdCheckDist, horizontalCheckDist;

    public RoadData roadData;

    [SerializeField] float minActionTime = 2.5f;
    [SerializeField] float maxActionTime = 10f;

    [SerializeField] float reactiveTurnDivider = 4f;

    [SerializeField] AudioClip honkSound;

    bool inLane;

    private void Awake()
    {
        car = GetComponent<Car>();
    }

    private void Start()
    {
        StartCoroutine(AttemptActionAfterDelay());

        InvokeRepeating(nameof(DestroyIfFarAway), 3f, 3f);
    }

    private void FixedUpdate()
    {
        MoveTowardsRoadUpwards();
    }

    void MoveTowardsRoadUpwards()
    {
        RaycastHit2D hitFwd = Physics2D.BoxCast(transform.position, transform.localScale, transform.eulerAngles.z, transform.up, fwdCheckDist);
        RaycastHit2D hitLeft = Physics2D.BoxCast(transform.position, transform.localScale, transform.eulerAngles.z, -transform.right, horizontalCheckDist);
        RaycastHit2D hitRight = Physics2D.BoxCast(transform.position, transform.localScale, transform.eulerAngles.z, transform.right, horizontalCheckDist);

        Vector3 leftOrigin = (transform.position + -transform.right * horizontalCheckDist);
        Vector3 rightOrigin = (transform.position + transform.right * horizontalCheckDist);
        Vector3 fwdOrigin = transform.position + transform.up * (fwdCheckDist - transform.localScale.y / 2);

        ExtDebug.DrawBoxCastBox(leftOrigin, transform.localScale / 2, transform.rotation, Vector3.zero, 0, GetCheckColor(hitLeft));
        ExtDebug.DrawBoxCastBox(rightOrigin, transform.localScale / 2, transform.rotation, Vector3.zero, 0, GetCheckColor(hitRight));
        ExtDebug.DrawBoxCastBox(fwdOrigin, new Vector3(transform.localScale.x, fwdCheckDist) / 2, transform.rotation, Vector3.zero, 0, GetCheckColor(hitFwd));

        TurnTowardsLane(hitLeft, hitRight);
        MoveForwardIfFree(hitFwd);
    }

    void TurnTowardsLane(bool hitLeft, bool hitRight)
    {
        Vector2 closestPoint = currentLane.GetClosestPoint(transform.position, out int index);
        Vector2 intersection = currentLane.CalculateHorizontalIntersection(closestPoint, index, transform.position);
        Vector2 targetPoint = intersection + (currentLane.road.direction * 3f);

        inLane = Vector3.Distance(intersection, transform.position) <= 0.5f;
        Color laneColor = inLane ? Color.green : Color.red;
        GetComponent<SpriteRenderer>().color = laneColor;

        Debug.DrawLine(transform.position, targetPoint);

        if (!hitLeft && !hitRight)
        {
            car.targetSpeed = car.moveSpeed;
            if (Vector3.Distance(transform.position, targetPoint) > 0.025f)
                car.RotateToDirection(targetPoint, car.turnSpeed);
        }
        else if (hitRight && !hitLeft)
        {
            if (!inLane)
            {
                car.RotateToDirection(transform.position + transform.right, car.turnSpeed / reactiveTurnDivider);
                car.targetSpeed = car.moveSpeed / 4;
            }
                

            car.RotateToDirection(transform.position - transform.right, car.turnSpeed / reactiveTurnDivider);
        }
        else if (hitLeft && !hitRight)
        {
            if (!inLane)
            {
                car.RotateToDirection(transform.position - transform.right, car.turnSpeed / reactiveTurnDivider);
                car.targetSpeed = car.moveSpeed / 4;
            }

            car.RotateToDirection(transform.position + transform.right, car.turnSpeed / reactiveTurnDivider);
        }
    }

    void MoveForwardIfFree(RaycastHit2D hitFwd)
    {
        if (hitFwd)
            car.Brake();
        else
            car.UnBrake();

        car.MoveInDirection(transform.up, car.targetSpeed);
    }

    // either change lane, accelerate?
    IEnumerator AttemptActionAfterDelay()
    {
        yield return new WaitForSeconds(Random.Range(minActionTime, maxActionTime));

        Lane startLane = currentLane;
        int randomDir = Random.Range(0, 2) == 0 ? -1 : 1;
        currentLane = roadData.ChangeLane(currentLane, randomDir);

        if(randomDir == -1)
        {
            RaycastHit2D hitLeft = Physics2D.BoxCast(transform.position, transform.localScale, transform.eulerAngles.z, -transform.right, horizontalCheckDist * 2f);
            if (hitLeft)
            {
                print("hitleft, cancelled merge");
                currentLane = startLane;
            }
        }
        else if(randomDir == 1)
        {
            RaycastHit2D hitRight = Physics2D.BoxCast(transform.position, transform.localScale, transform.eulerAngles.z, transform.right, horizontalCheckDist * 2f);
            if (hitRight)
            {
                print("hitright, cancelled merge");
                currentLane = startLane;
            }
        }   

        StartCoroutine(AttemptActionAfterDelay());
    }

    void DrawCheck(RaycastHit2D hit, Vector3 targetPos)
    {
        Debug.DrawLine(transform.position, targetPos, GetCheckColor(hit));
    }

    Color GetCheckColor(bool hit)
    {
        if (hit)
            return Color.red;
        else
            return Color.yellow;
    }

    void DestroyIfFarAway()
    {
        if(Vector2.Distance((Vector2)Camera.main.transform.position, transform.position) >= 10f)
        {
            Destroy(gameObject);
        }
    }
}
