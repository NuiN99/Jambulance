using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarsController : MonoBehaviour
{
    public CarStatsSO playerStats;
    public CarStatsSO enemyStats;
    public CarStatsSO copStats;
    public void SwitchStats(Car car, CarStatsSO targetStats)
    {
        car.stats = targetStats;
        car.startSpeed = car.stats.moveSpeed;
        car.targetSpeed = car.startSpeed;
    }

    public void ResetSpriteSelection(Car car)
    {
        if(car.TryGetComponent(out SpriteRenderer sr))
        {
            sr.sprite = car.stats.possibleSprites[Random.Range(0, car.stats.possibleSprites.Length)];
        }
    }
}
