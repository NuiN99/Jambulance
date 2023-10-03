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
    public bool merging = false;

    [SerializeField] LayerMask obstacleMask;

    Obstacle detectedObstacle;

    [SerializeField] AudioClip[] honkSounds;
    [SerializeField] float honkDelay;
    float timeSinceLastHonk = 0;

    [SerializeField] AudioClip[] screechSounds;
    [SerializeField] float screechDelay;
    float timeSinceLastScreech = 0;

    [SerializeField] LayerMask carMask;

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
        TryBrakeSounds();
    }

    void TryBrakeSounds()
    {
        float distFromCam = Vector2.Distance(Camera.main.transform.position, transform.position);

        distFromCam = Mathf.Clamp(distFromCam, 1f, 100f);

        if (car.braking && Time.time - timeSinceLastHonk > honkDelay)
        {
            timeSinceLastHonk = Time.time;
            AudioController.Instance.PlaySpatialSound(honkSounds[Random.Range(0, honkSounds.Length)], transform.position, Mathf.Min((1f / distFromCam) / 4, 1f));
        }
        if (car.braking && Time.time - timeSinceLastScreech > screechDelay)
        {
            timeSinceLastScreech = Time.time;
            AudioController.Instance.PlaySpatialSound(screechSounds[Random.Range(0, screechSounds.Length)], transform.position, Mathf.Min((1f / distFromCam) / 4, 1f));
        }
    }


    void MoveTowardsRoadUpwards()
    {
        RaycastHit2D[] hitFwdAll = Physics2D.BoxCastAll(transform.position, new Vector3(0.5f, 1f), transform.eulerAngles.z, transform.up, fwdCheckDist, carMask);
        RaycastHit2D[] hitLeftAll = Physics2D.BoxCastAll(transform.position, transform.localScale, transform.eulerAngles.z, -transform.right, horizontalCheckDist, carMask);
        RaycastHit2D[] hitRightAll = Physics2D.BoxCastAll(transform.position, transform.localScale, transform.eulerAngles.z, transform.right, horizontalCheckDist, carMask);
        RaycastHit2D[] hitBackAll = Physics2D.BoxCastAll(transform.position, transform.localScale, transform.eulerAngles.z, -transform.up, fwdCheckDist, carMask);

        RaycastHit2D hitFwd = new();
        RaycastHit2D hitLeft = new();
        RaycastHit2D hitRight = new();
        RaycastHit2D hitBack = new();

        RaycastHit2D hitFwdObstacle = Physics2D.Raycast(transform.position, Vector2.up, fwdCheckDist * 5, obstacleMask);

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
        ExtDebug.DrawBoxCastBox(fwdOrigin, new Vector3(0.5f, fwdCheckDist) / 2, transform.rotation, Vector3.zero, 0, GetCheckColor(hitFwd));
        ExtDebug.DrawBoxCastBox(backOrigin, new Vector3(transform.localScale.x, fwdCheckDist / 1.5f) / 2, transform.rotation, Vector3.zero, 0, GetCheckColor(hitBack));


        if (hitFwdObstacle && hitFwdObstacle.collider.TryGetComponent(out Obstacle obstacle))
        {
            Debug.DrawLine(transform.position, hitFwdObstacle.point, Color.yellow);
            ReactToObstacle(obstacle);
        }
        
        TurnTowardsLane(hitLeft, hitRight);
        MoveForwardIfFree(hitFwd, hitBack);
    }

    void TurnTowardsLane(bool hitLeft, bool hitRight)
    {
        Vector2 closestPoint = currentLane.GetClosestPoint(transform.position, out int index);
        Vector2 intersection = currentLane.CalculateHorizontalIntersection(closestPoint, index, transform.position);
        Vector2 targetPoint = intersection + (currentLane.road.direction * 3f);

        if (Vector3.Distance(intersection, transform.position) <= 0.3f)
            merging = false;

        inLane = Vector3.Distance(intersection, transform.position) <= 0.5f;

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

    void ReactToObstacle(Obstacle obstacle)
    {
        if (detectedObstacle == obstacle) return;
        detectedObstacle = obstacle;
        roadData.ChangeLaneRandom(currentLane);
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

        if(currentLane != startLane)
        {
            merging = true;
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
