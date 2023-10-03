using PrimeTween;
using System.Collections;
using UnityEngine;

public class Car : MonoBehaviour, IDestructable
{
    public CarStatsSO stats;
    public Rigidbody2D rb;
    SpriteRenderer sr;

    public float startSpeed;
    public float targetSpeed;

    public bool colliding;

    public bool braking = false;

    public float curAccel;

    bool dead = false;

    [SerializeField] float smokeEmissionMax = 20f;

    [SerializeField] GameObject explosionEffect;
    [SerializeField] GameObject smokeEffect;

    ParticleSystem currentSmokeEffect;

    [SerializeField] AudioClip[] crashSounds;

    AudioSource source;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = GetComponentInChildren<Rigidbody2D>();


        sr = GetComponent<SpriteRenderer>();
        if(sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        source = GetComponent<AudioSource>();
    }


    void Start()
    {
        if(!GetComponent<Player>())
        {
            sr.sprite = stats.possibleSprites[Random.Range(0, stats.possibleSprites.Length)];
        }

        source.clip = stats.possibleDrivingLoopClips[Random.Range(0, stats.possibleDrivingLoopClips.Length)];
        source.Play();

        currentSmokeEffect = Instantiate(smokeEffect).GetComponent<ParticleSystem>();
        currentSmokeEffect.GetComponent<MoveToTarget>().target = transform;

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

        source.pitch = rb.velocity.magnitude / 2;
        source.pitch = Mathf.Clamp(source.pitch, 0, 3);

        if(curAccel > stats.maxAcceleration)
        {
            curAccel = stats.maxAcceleration;
        }

        if(TryGetComponent(out Health health))
        {
            var emission = currentSmokeEffect.emission;
            float lerpFactor = 1 - (health.health / stats.health);

            emission.rateOverTime = Mathf.Lerp(0, smokeEmissionMax, lerpFactor);
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
            else if (collisionForce > 2f || collision.gameObject.GetComponent<Car>().stats.health == 1)
            {
                car.rb.AddForceAtPosition(2 * collisionForce * collisionDir, collisionPoint, ForceMode2D.Impulse);
                car.rb.angularVelocity += collisionForce * 50 * Random.Range(-1f, 1f);
                car.rb.angularDrag = 0f;
                car.Brake();
                car.rb.drag = car.stats.drag / 2f;
                car.colliding = true;

                curAccel -= stats.maxAcceleration / 1.25f;
            }

            ScreenShake.Instance.ShakeScreen(collisionForce, .25f);
        }
        else if(GetComponent<Player>() && collisionForce > 2f)
        {
            curAccel -= stats.maxAcceleration / 1.25f;
        }

        curAccel -= collisionForce / 50;
    }

    void PlayCrashSoundsBasedOnForce(float force)
    {
        float distFromCam = Vector2.Distance(Camera.main.transform.position, transform.position);
        if (distFromCam > 12.5f) return;

        distFromCam = Mathf.Clamp(distFromCam, 1f, 12.5f);
        float vol = (0.1f * force) / distFromCam;
        if (vol <= 0.0025f) return;

        if (force >= 7.5f)
            AudioController.Instance.PlaySpatialSound(crashSounds[0], transform.position, vol);
        else if (force >= 5f)
            AudioController.Instance.PlaySpatialSound(crashSounds[1], transform.position, vol);
        else if (force >= 2.5f)
            AudioController.Instance.PlaySpatialSound(crashSounds[2], transform.position, vol);
        else
            AudioController.Instance.PlaySpatialSound(crashSounds[3], transform.position, vol);
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

        if (!GetComponent<Player>())
            Destroy(gameObject, 30f);
        else
            GameController.Instance.gameOver = true;

        GetComponent<Health>().health = 0f;

        AudioClip clip = stats.explosionSounds[Random.Range(0, stats.explosionSounds.Length)];
        float dist = Vector2.Distance(transform.position, (Vector2)Camera.main.transform.position);
        float vol = 0.35f;
        if (dist <= 0.5f) vol = 0.15f;
        AudioController.Instance.PlaySpatialSound(clip, transform.position, vol);

        dead = true;
        Tween.Color(sr, sr.color, new Color(.2f, .2f, .2f, 2f), 2f, Ease.OutCubic);
        var effect = Instantiate(explosionEffect, transform.position, Random.rotation);
        
        effect.AddComponent<MoveToTarget>().target = transform;

        Invoke(nameof(SpawnSmokeDelayed), 2f);

        Destroy(effect, 2f);

        float distFromCam = Vector2.Distance((Vector2)Camera.main.transform.position, (Vector2)transform.position);
        ScreenShake.Instance.ShakeScreen(10f / distFromCam, 0.25f);

        AddForceToCarsHitByExplosion();   
    }

    void AddForceToCarsHitByExplosion()
    {
        RaycastHit2D[] circleCast = Physics2D.CircleCastAll(transform.position, 4f, Vector3.zero, 0);
        foreach (var hit in circleCast)
        {
            if (hit.collider.gameObject == gameObject) continue;
            if (hit.collider.TryGetComponent(out Car car)/* && !car.GetComponent<Player>()*/)
            {
                Vector2 forceDir = (hit.point - (Vector2)transform.position).normalized;
                float forceDist = (Vector2.Distance(transform.position, hit.point));
                car.rb.AddForce((stats.explosionForce / forceDist) * forceDir, ForceMode2D.Impulse);
            }
        }
    }

    void SpawnSmokeDelayed()
    {
        var smoke = Instantiate(smokeEffect, transform.position, Quaternion.identity);
        smoke.AddComponent<MoveToTarget>().target = transform;

        if (!GetComponent<Player>())
            Destroy(smoke, 30f);
    }
}

