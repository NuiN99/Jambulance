using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


public class Health : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] float heavyImpactForceThreshold;
    [SerializeField] float heavyImpactMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        //if it's a car use car stats
        if(TryGetComponent<Car>(out Car car))
        {
             health = car.stats.health;
             heavyImpactForceThreshold = car.stats.heavyImpactForceThreshold;
             heavyImpactMultiplier = car.stats.heavyImpactMultiplier;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;

        health -= collisionForce * (collisionForce >= heavyImpactForceThreshold ? heavyImpactMultiplier : 1);
        if (health < 0)
        {
            //Destroy self
        }
    }
}
