using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Car : MonoBehaviour
{
    public CarStatsSO stats;
    public Rigidbody2D rb;

    public float startSpeed;
    public float targetSpeed;

    public bool colliding;

    bool braking = false;

    public float curAccel;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = GetComponentInChildren<Rigidbody2D>();
    }


    void Start()
    {
        if(!GetComponent<Player>() && TryGetComponent(out SpriteRenderer sr))
        {
            sr.sprite = stats.possibleSprites[Random.Range(0, stats.possibleSprites.Length)];
        }

        rb.drag = stats.drag;
        rb.angularDrag = stats.angularDrag;
        rb.mass = stats.mass;
    }

    private void Update()
    {
        if (curAccel < 0)
        {
            curAccel = 0;
        }
            

        if(curAccel > stats.maxAcceleration)
        {
            curAccel = stats.maxAcceleration;
        }
            
    }

    public void MoveInDirection(Vector2 dir, float speed)
    {
        if (colliding) return;

        curAccel += (Time.fixedDeltaTime / 3f) * stats.accelSpeed;
        Vector2 force = curAccel * speed * dir;
        rb.AddForce(force);
        if(braking)
        {
            curAccel -= Time.fixedDeltaTime * stats.accelSpeed / 2;
        }
    }
    public void RotateInDirection(float dir, float speed)
    {
        if (colliding)
            return;
           
        float targetAngle = transform.eulerAngles.z + (-dir * 5);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        float step = speed * rb.velocity.magnitude;
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        transform.rotation = newRotation;

        
        //rb.MoveRotation(targetAngle);
    }

    public void RotateToDirection(Vector3 target, float speed)
    {
        if (colliding)
        {
            Brake();
            return;
        }
        Vector3 targetDir = (target - transform.position).normalized;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, targetDir), stats.turnSpeed * rb.velocity.magnitude);
    }

    public void Brake()
    {
        braking = true;
        //rb.drag = stats.brakeDrag;
    }
    public void UnBrake()
    {
        braking = false;
        //rb.drag = stats.drag;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print(GetComponent<Rigidbody2D>().velocity.magnitude);

        Vector2 collisionPoint = collision.GetContact(0).point;
        float collisionForce = collision.relativeVelocity.magnitude;
        Vector2 collisionDir = ((Vector2)collision.transform.position - collisionPoint).normalized;

        if (GetComponent<Player>() && collision.gameObject.TryGetComponent(out Car car))
        {
            if (car.rb.velocity.magnitude * car.rb.mass > rb.velocity.magnitude * rb.mass && collisionForce > 3)
            {
                rb.AddForceAtPosition(-collisionDir * collisionForce, collisionPoint, ForceMode2D.Impulse);
                rb.angularVelocity += collisionForce * 2.5f * Random.Range(-1f, 1f);
                //Brake();
                rb.drag = stats.drag / 2f;
                colliding = true;
                curAccel = 0;
            }

            else if (collisionForce > 2f)
            {
                car.rb.AddForceAtPosition(collisionDir * collisionForce * 2, collisionPoint, ForceMode2D.Impulse);
                car.rb.angularVelocity += collisionForce * 50 * Random.Range(-1f, 1f);
                car.rb.angularDrag = 0f;
                car.Brake();
                car.rb.drag = car.stats.drag / 2f;
                car.colliding = true;

                curAccel -= stats.maxAcceleration / 1.25f;
            }
        }

        stats.health -= collisionForce * (collisionForce >= stats.heavyImpactForceThreshold ? stats.heavyImpactMultiplier : 1);
        if (stats.health < 0)
        {
            //Destroy car
        }

        curAccel -= collisionForce / 50;
    }

    Coroutine currentRoutine;
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (gameObject.activeSelf)
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            currentRoutine = StartCoroutine(StopCollidingAfterDelay(stats.recoveryTime));
        }

    }

    IEnumerator StopCollidingAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        colliding = false;

        rb.angularDrag = stats.angularDrag;
        rb.drag = stats.drag;
    }
}

