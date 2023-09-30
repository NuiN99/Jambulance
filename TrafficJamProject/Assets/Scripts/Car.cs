using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cai
{
    public class Car : MonoBehaviour
    {
        Rigidbody2D rb;

        [SerializeField] public float moveSpeed;
        [SerializeField] public float turnSpeed;

        [SerializeField] float drag;
        [SerializeField] float angularDrag;
        [SerializeField] float mass;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.mass = mass;
        }

       
        public void MoveInDirection(Vector3 dir)
        {
            rb.AddForce(moveSpeed * dir);
        }
        public void MoveInDirection(Vector3 dir, float speed)
        {
            rb.AddForce(speed * dir);
        }

        public void RotateInDirection(float dir)
        {
            float targetAngle = transform.eulerAngles.z + (turnSpeed * rb.velocity.magnitude * -dir);
            rb.MoveRotation(targetAngle);
        }
        public void RotateInDirection(float dir, float speed)
        {
            float targetAngle = transform.eulerAngles.z + (speed * rb.velocity.magnitude * -dir);
            rb.MoveRotation(targetAngle);
        }

        public void RotateToDirection(Vector3 target, float speed)
        {
            Vector3 targetDir = (target - transform.position).normalized;
            float dirAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            dirAngle -= 90;

            // Calculate the angle between the current rotation and the target angle
            float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.z, dirAngle);

            // Determine the direction to rotate based on the angle difference
            int dir = angleDifference < 0 ? 1 : -1;

            // Calculate the target angle
            float targetAngle = transform.eulerAngles.z + (speed * rb.velocity.magnitude * -dir);

            // Move towards the target angle
            rb.MoveRotation(targetAngle);
        }
    }

}
