using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Car car;

    public Lane currentLane;

    //[SerializeField] float targetOffset = 2.5f;

    [SerializeField] float fwdCheckDist = 2; 
    [SerializeField] float horizontalCheckDist = 0.25f;

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
        if (roadData == null)
        {
            RoadData[] roadDatas = FindObjectsOfType<RoadData>();
            foreach(RoadData road in roadDatas)
            {
                if(road.direction == Vector2.up)
                {
                    roadData = road;
                    break;
                }
            }
            currentLane = roadData.lanes[2];
        }

        StartCoroutine(AttemptActionAfterDelay());

        InvokeRepeating(nameof(DestroyIfFarAway), 3f, 3f);
    }

    private void FixedUpdate()
    {
        MoveTowardsRoadUpwards();
    }

    void MoveTowardsRoadUpwards()
    {
        RaycastHit2D[] hitFwdAll = Physics2D.BoxCastAll(transform.position, transform.localScale, transform.eulerAngles.z, transform.up, fwdCheckDist);
        RaycastHit2D[] hitLeftAll = Physics2D.BoxCastAll(transform.position, transform.localScale, transform.eulerAngles.z, -transform.right, horizontalCheckDist);
        RaycastHit2D[] hitRightAll = Physics2D.BoxCastAll(transform.position, transform.localScale, transform.eulerAngles.z, transform.right, horizontalCheckDist);
        RaycastHit2D[] hitBackAll = Physics2D.BoxCastAll(transform.position, transform.localScale, transform.eulerAngles.z, -transform.up, fwdCheckDist);

        RaycastHit2D hitFwd = new();
        RaycastHit2D hitLeft = new();
        RaycastHit2D hitRight = new();
        RaycastHit2D hitBack = new();

        foreach (var hit in hitFwdAll)
        {
            if (hit.collider.gameObject != gameObject)
                hitFwd = hit;
        }
        foreach (var hit in hitLeftAll)
        {
            if (hit.collider.gameObject != gameObject)
                hitLeft = hit;
        }
        foreach (var hit in hitRightAll)
        {
            if (hit.collider.gameObject != gameObject)
                hitRight = hit;
        }
        foreach (var hit in hitBackAll)
        {
            if (hit.collider.gameObject != gameObject)
                hitBack = hit;
        }

        Vector3 leftOrigin = (transform.position + -transform.right * horizontalCheckDist);
        Vector3 rightOrigin = (transform.position + transform.right * horizontalCheckDist);
        Vector3 fwdOrigin = transform.position + transform.up * (fwdCheckDist - transform.localScale.y / 2);
        Vector3 backOrigin = transform.position - transform.up * (fwdCheckDist / 1.5f - transform.localScale.y / 2);

        ExtDebug.DrawBoxCastBox(leftOrigin, transform.localScale / 2, transform.rotation, Vector3.zero, 0, GetCheckColor(hitLeft));
        ExtDebug.DrawBoxCastBox(rightOrigin, transform.localScale / 2, transform.rotation, Vector3.zero, 0, GetCheckColor(hitRight));
        ExtDebug.DrawBoxCastBox(fwdOrigin, new Vector3(transform.localScale.x, fwdCheckDist) / 2, transform.rotation, Vector3.zero, 0, GetCheckColor(hitFwd));
        ExtDebug.DrawBoxCastBox(backOrigin, new Vector3(transform.localScale.x, fwdCheckDist / 1.5f) / 2, transform.rotation, Vector3.zero, 0, GetCheckColor(hitBack));

        TurnTowardsLane(hitLeft, hitRight);
        MoveForwardIfFree(hitFwd, hitBack);
    }

    void TurnTowardsLane(bool hitLeft, bool hitRight)
    {
        Vector2 closestPoint = currentLane.GetClosestPoint(transform.position, out int index);
        Vector2 intersection = currentLane.CalculateHorizontalIntersection(closestPoint, index, transform.position);
        Vector2 targetPoint = intersection + (currentLane.road.direction * 3f);

        inLane = Vector3.Distance(intersection, transform.position) <= 0.5f;
        //Color laneColor = inLane ? Color.blue : Color.yellow;
        //GetComponent<SpriteRenderer>().color = laneColor;

        Debug.DrawLine(transform.position, targetPoint);

        if (!hitLeft && !hitRight)
        {
            car.targetSpeed = car.startSpeed;
            if (Vector3.Distance(transform.position, targetPoint) > 0.025f)
                car.RotateToDirection(targetPoint, car.stats.turnSpeed);
        }
        else if (hitRight && !hitLeft)
        {
            if (!inLane)
            {
                car.RotateToDirection(transform.position + transform.right, car.stats.turnSpeed / reactiveTurnDivider);
                //car.targetSpeed = car.startSpeed / 4;
            }
                

            car.RotateToDirection(transform.position - transform.right, car.stats.turnSpeed / reactiveTurnDivider);
        }
        else if (hitLeft && !hitRight)
        {
            if (!inLane)
            {
                car.RotateToDirection(transform.position - transform.right, car.stats.turnSpeed / reactiveTurnDivider);
                //car.targetSpeed = car.startSpeed / 4;
            }

            car.RotateToDirection(transform.position + transform.right, car.stats.turnSpeed / reactiveTurnDivider);
        }
    }

    void MoveForwardIfFree(RaycastHit2D hitFwd, RaycastHit2D hitBack)
    {
        if (hitFwd)
        {
            car.Brake();
            float distToHit = (Vector3.Distance(transform.position, hitFwd.point));
            //car.targetSpeed = (car.startSpeed * speedMult) + 5f;
            //if (distToHit <= 0.3f)
                //car.targetSpeed = -0.75f;
            


            //if (hitBack) car.targetSpeed = 0;

            //car.curAccel -= Time.deltaTime
        }
            
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
            RaycastHit2D hitLeft = Physics2D.BoxCast(transform.position, transform.localScale, transform.eulerAngles.z, -transform.right, horizontalCheckDist * 5f);
            if (hitLeft)
            {
                currentLane = startLane;
            }
        }
        else if(randomDir == 1)
        {
            RaycastHit2D hitRight = Physics2D.BoxCast(transform.position, transform.localScale, transform.eulerAngles.z, transform.right, horizontalCheckDist * 5f);
            if (hitRight)
            {
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
