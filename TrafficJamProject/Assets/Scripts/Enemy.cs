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

    private void Awake()
    {
        car = GetComponent<Car>();
    }

    private void Start()
    {
        car.moveSpeed = Random.Range(car.moveSpeed - 30f, car.moveSpeed + 30f);

        StartCoroutine(AttemptActionAfterDelay());
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
        //float dist = currentLane.path.GetClosestDistanceAlongPath(transform.position);
        //Vector3 targetPoint = currentLane.path.GetPointAtDistance(dist + targetOffset);

        Vector3 closestPoint = currentLane.GetClosestPoint(transform.position, out int index);
        Vector3 intersection = currentLane.CalculateHorizontalIntersection(closestPoint, index, transform.position);
        Vector3 targetPoint = intersection + new Vector3(0, 3f);

        Debug.DrawLine(transform.position, targetPoint);

        if (!hitLeft && !hitRight)
        {
            if (Vector3.Distance(transform.position, targetPoint) > 0.025f)
                car.RotateToDirection(targetPoint, car.turnSpeed);
        }
        else if (hitRight && !hitLeft)
        {
            //AudioController.Instance.PlaySpatialSound(honkSound, 0.05f);

            car.RotateToDirection(transform.position - transform.right, car.turnSpeed / reactiveTurnDivider);
        }
        else if (hitLeft && !hitRight)
        {
            car.RotateToDirection(transform.position + transform.right, car.turnSpeed / reactiveTurnDivider);
        }
    }

    void MoveForwardIfFree(RaycastHit2D hitFwd)
    {
        float moveSpeed = car.moveSpeed;
        if (hitFwd)
        {
            float speedMult = (Vector3.Distance(transform.position, hitFwd.point) / fwdCheckDist) / 1.5f;
            moveSpeed *= speedMult;

            if (speedMult <= 0.25f)
                moveSpeed = 0f;

            car.MoveInDirection(transform.up, moveSpeed);
        }
        else
        {
            car.MoveInDirection(transform.up, car.moveSpeed);
        }
    }

    // either change lane, accelerate?
    IEnumerator AttemptActionAfterDelay()
    {
        yield return new WaitForSeconds(Random.Range(minActionTime, maxActionTime));
        currentLane = roadData.ChangeLaneRandom(currentLane);
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
}
