using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
    public float health;
    [SerializeField] float heavyImpactForceThreshold;
    [SerializeField] float heavyImpactMultiplier;

    bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetStatsToCarDefault();
    }

    public void ResetStatsToCarDefault()
    {
        //if it's a car use car stats
        if (TryGetComponent<Car>(out Car car))
        {
            health = car.stats.health;
            heavyImpactForceThreshold = car.stats.heavyImpactForceThreshold;
            heavyImpactMultiplier = car.stats.heavyImpactMultiplier;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;

        if(collisionForce > 1.5f)
            health -= collisionForce * (collisionForce >= heavyImpactForceThreshold ? heavyImpactMultiplier : 1);
        if (!dead && health < 0 && TryGetComponent(out IDestructable destructable))
        {
            dead = true;
            destructable.Destroy();
        }
    }
}
