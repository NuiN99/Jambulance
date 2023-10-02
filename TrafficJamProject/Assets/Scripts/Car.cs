using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class Car : MonoBehaviour, IDestructable
{
    public CarStatsSO stats;
    public Rigidbody2D rb;
    public ScreenShake sc;
    SpriteRenderer sr;

    public float startSpeed;
    public float targetSpeed;

    public bool colliding;

    bool braking = false;

    public float curAccel;

    bool dead = false;

    [SerializeField] GameObject explosionEffect;
    [SerializeField] GameObject smokeEffect;

    [SerializeField] AudioClip[] crashSounds;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = GetComponentInChildren<Rigidbody2D>();

        sc = Camera.main.GetComponent<ScreenShake>();

        sr = GetComponent<SpriteRenderer>();
        if(sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();
    }


    void Start()
    {
        if(!GetComponent<Player>())
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
        if (colliding || dead) return;

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
        if (colliding || dead)
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
        if (colliding || dead)
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

        PlayCrashSoundsBasedOnForce(collisionForce);

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
                car.rb.AddForceAtPosition(2 * collisionForce * collisionDir, collisionPoint, ForceMode2D.Impulse);
                car.rb.angularVelocity += collisionForce * 50 * Random.Range(-1f, 1f);
                car.rb.angularDrag = 0f;
                car.Brake();
                car.rb.drag = car.stats.drag / 2f;
                car.colliding = true;

                curAccel -= stats.maxAcceleration / 1.25f;
            }

            //screen shake
            sc.StartCoroutine(sc.ShakeScreen(collisionForce));
        }

        curAccel -= collisionForce / 50;
    }

    void PlayCrashSoundsBasedOnForce(float force)
    {
        float distFromCam = Vector2.Distance(Camera.main.transform.position, transform.position);
        if (distFromCam > 12.5f) return;

        if (force >= 7.5f)
            AudioController.Instance.PlaySpatialSound(crashSounds[0], transform.position, (0.2f * force) / distFromCam);
        else if (force >= 5f)
            AudioController.Instance.PlaySpatialSound(crashSounds[1], transform.position, (0.1f * force) / distFromCam);
        else if (force >= 2.5f)
            AudioController.Instance.PlaySpatialSound(crashSounds[2], transform.position, (0.1f * force) / distFromCam);
        else
            AudioController.Instance.PlaySpatialSound(crashSounds[3], transform.position, (0.1f * force) / distFromCam);
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

    void IDestructable.Destroy()
    {
        if (dead) return;

        dead = true;
        Tween.Color(sr, sr.color, new Color(.1f, .1f, .1f, 1f), .5f, Ease.OutCubic);
        var effect = Instantiate(explosionEffect, transform.position, Random.rotation);
        
        effect.AddComponent<MoveToTarget>().target = transform;

        Invoke(nameof(SpawnSmokeDelayed), 2f);

        Destroy(effect, 2f);
    }

    void SpawnSmokeDelayed()
    {
        var smoke = Instantiate(smokeEffect, transform.position, Quaternion.identity);
        smoke.AddComponent<MoveToTarget>().target = transform;
        Destroy(smoke, 30f);
    }
}

