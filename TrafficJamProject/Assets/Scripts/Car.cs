using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Rigidbody2D rb;

    [SerializeField] public float moveSpeed;
    [SerializeField] public float turnSpeed;

    public float targetSpeed;

    [SerializeField] float brakeStrength = 2f;

    [SerializeField] float drag;
    [SerializeField] float brakeDrag;
    
    [SerializeField] float angularDrag;
    [SerializeField] float mass;

    [SerializeField] float aggression;

    [SerializeField] float recoveryTime;

    public bool colliding;

    [SerializeField] public float health;

    [SerializeField] float heavyImpactForceThreshold = 4f;
    [SerializeField] float heavyImpactMultiplier = 2f;

    bool braking = false;

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
        if(!braking)
            rb.AddForce(moveSpeed * dir);
    }
    public void MoveInDirection(Vector3 dir, float speed)
    {
        if(!braking)
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
        if (colliding)
        {
            Brake();
            return;
        }

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
        braking = true;
        rb.drag = brakeDrag;
    }
    public void UnBrake()
    {
        braking = false;
        rb.drag = drag;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Player>())
            colliding = true;

        Vector2 collisionPoint = collision.GetContact(0).point;
        float collisionForce = collision.relativeVelocity.magnitude;
        Vector2 collisionDir = ((Vector2)collision.transform.position - collisionPoint).normalized;

        if (collision.gameObject.TryGetComponent(out Car car))
        {
            //rb.velocity = collision.relativeVelocity;
            car.rb.AddForceAtPosition(collisionDir * collisionForce, collisionPoint, ForceMode2D.Impulse);
            car.rb.angularDrag = 0f;
            car.Brake();
        }

        health -= collisionForce * (collisionForce >= heavyImpactForceThreshold ? heavyImpactMultiplier : 1);
        if (health < 0)
        {
            //Destroy car
        }
    }

    Coroutine currentRoutine;
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (gameObject.activeSelf)
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            currentRoutine = StartCoroutine(StopCollidingAfterDelay(recoveryTime));
        }

    }

    IEnumerator StopCollidingAfterDelay(float time)
    {
        yield return new WaitForSeconds(2);
        colliding = false;
    }
}

