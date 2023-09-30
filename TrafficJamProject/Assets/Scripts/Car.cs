using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cai
{
    public class Car : MonoBehaviour
    {
        public Rigidbody2D rb;

        [SerializeField] public float moveSpeed;
        [SerializeField] public float turnSpeed;

        [SerializeField] float brakeStrength = 2f;

        [SerializeField] float drag;
        [SerializeField] float angularDrag;
        [SerializeField] float mass;

        [SerializeField] float aggression;

        [SerializeField] float recoveryTime;

        public bool colliding;

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

            float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.z, dirAngle);

            int dir = angleDifference < 0 ? -1 : 1;

            float targetAngle = transform.eulerAngles.z + (speed * rb.velocity.magnitude * dir);

            if (angleDifference >= 1.5f || angleDifference <= -1.5f)
                rb.MoveRotation(targetAngle);
        }

        public void Brake()
        {
            float brakeDrag = rb.drag * brakeStrength;
            rb.drag = brakeDrag;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            colliding = true;

            float collisionForce = collision.relativeVelocity.magnitude * 2.5f;

            if (GetComponent<Player>() && collision.gameObject.TryGetComponent(out Car car))
            {
                Vector2 collisionDir = ((Vector2)collision.transform.position - collision.GetContact(0).point).normalized;

                rb.velocity = collision.relativeVelocity;
                car.rb.AddForce(collisionDir * collisionForce, ForceMode2D.Impulse);
                return;
            }
        }

        Coroutine currentRoutine;
        private void OnCollisionExit2D(Collision2D collision)
        {
            if(gameObject.activeSelf)
            {
                if (currentRoutine != null) 
                    StopCoroutine(currentRoutine);

                currentRoutine = StartCoroutine(StopCollidingAfterDelay(recoveryTime));
            }
                
        }

        IEnumerator StopCollidingAfterDelay(float time)
        {
            yield return new WaitForSeconds(0/*time*/);
            colliding = false;
        }
    }

}
