using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Cai
{
    public class Enemy : MonoBehaviour
    {
        Car car;
        public PathCreator lane;
        [SerializeField] float targetOffset = 2.5f;

        [SerializeField] float fwdCheckDist, horizontalCheckDist;

        private void Awake()
        {
            car = GetComponent<Car>();
        }

        private void FixedUpdate()
        {
            MoveTowardsRoadUpwards();
        }

        void MoveTowardsRoadUpwards()
        {
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, transform.right, horizontalCheckDist);
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -transform.right, horizontalCheckDist);
            RaycastHit2D hitFwd = Physics2D.Raycast(transform.position, transform.up, fwdCheckDist);

            DrawRaycasts(hitLeft, hitRight, hitFwd);

            TurnTowardsPath(hitLeft, hitRight);

            MoveForwardIfFree(hitFwd);
        }

        void ChangeLane()
        {

        }

        void DrawRaycasts(RaycastHit2D left, RaycastHit2D right, RaycastHit2D fwd)
        {
            
            if (left)
                Debug.DrawLine(transform.position, left.point, Color.red);
            else
                Debug.DrawLine(transform.position, transform.position + (-transform.right * horizontalCheckDist), Color.yellow);

            if (right)
                Debug.DrawLine(transform.position, right.point, Color.red);
            else
                Debug.DrawLine(transform.position, transform.position + (transform.right * horizontalCheckDist), Color.yellow);

            if (fwd)
                Debug.DrawLine(transform.position, fwd.point, Color.red);
            else
                Debug.DrawLine(transform.position, transform.position + (transform.up * fwdCheckDist), Color.yellow);
        }

        void TurnTowardsPath(bool hitLeft, bool hitRight)
        {
            float dist = lane.path.GetClosestDistanceAlongPath(transform.position);
            Vector3 targetPoint = lane.path.GetPointAtDistance(dist + targetOffset);

            Debug.DrawLine(transform.position, targetPoint);

            if (!hitLeft && !hitRight)
            {
                Vector3 directTarget = lane.path.GetPointAtDistance(dist);

                if (Vector3.Distance(transform.position, directTarget) > 0.025f)
                    car.RotateToDirection(targetPoint, car.turnSpeed);
            }
            else if (hitRight && !hitLeft)
            {
                car.RotateToDirection(transform.position - transform.right, car.turnSpeed / 2);
            }
            else if (hitLeft && !hitRight)
            {
                car.RotateToDirection(transform.position + transform.right, car.turnSpeed / 2);
            }
        }

        void MoveForwardIfFree(RaycastHit2D hitFwd)
        {
            float moveSpeed = car.moveSpeed;
            if (hitFwd)
            {
                float speedMult = Vector3.Distance(transform.position, hitFwd.point) / fwdCheckDist;
                moveSpeed *= speedMult;

                if (speedMult <= 0.5f)
                    moveSpeed = 0f;

                car.MoveInDirection(transform.up, moveSpeed);
            }
            else
            {
                car.MoveInDirection(transform.up, car.moveSpeed);
            }
        }
    }

}
