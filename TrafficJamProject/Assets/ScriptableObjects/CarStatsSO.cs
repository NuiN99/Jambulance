using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="CarStats", menuName ="ScriptableObjects/Cars/Stats")]
public class CarStatsSO : ScriptableObject
{
    public Sprite[] possibleSprites;

    public float moveSpeed;
    public float turnSpeed;
    
    public float brakeStrength = 2f;

    public float drag;
    public float brakeDrag;

    public float angularDrag;
    public float mass;

    public float aggression;

    public float recoveryTime;

    
    public float health;

    public float accelSpeed = 1f;
    public float maxAcceleration = 5;

    public float heavyImpactForceThreshold = 4f;
    public float heavyImpactMultiplier = 2f;
}
