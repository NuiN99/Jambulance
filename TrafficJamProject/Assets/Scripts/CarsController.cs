using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarsController : MonoBehaviour
{
    public CarStatsSO playerStats;
    public CarStatsSO enemyStats;
    public void SwitchStats(Car car, CarStatsSO targetStats)
    {
        car.stats = targetStats;
        car.startSpeed = car.stats.moveSpeed;
        car.targetSpeed = car.startSpeed;
    }
}
