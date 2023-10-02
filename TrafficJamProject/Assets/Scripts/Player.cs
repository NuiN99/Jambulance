using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Car car;

    public AudioSource sirenSource;

    private void Awake()
    {
        car = GetComponent<Car>();
    }

    private void Start()
    {

        CarsController carsController = FindObjectOfType<CarsController>();
    }

    private void FixedUpdate()
    {
        float moveAxis = Input.GetAxis("Vertical");
        float rotateAxis = Input.GetAxis("Horizontal");

        if (moveAxis != 0f)
        {
            car.MoveInDirection(transform.up * moveAxis, car.targetSpeed);
        }
        else
        {
            car.curAccel -= Time.fixedDeltaTime * 15 * car.stats.accelSpeed;
        }

        car.RotateInDirection(rotateAxis * moveAxis, car.stats.turnSpeed);

        car.UnBrake();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up * 2f, Color.green);
    }

    private void OnEnable()
    {
        car.startSpeed = car.stats.moveSpeed;
        car.targetSpeed = car.stats.moveSpeed;

        GetComponent<Health>().ResetStatsToCarDefault();

        GameController.OnPlayerDeath += ToggleSiren;
        GameController.OnGameStarted += PlaySiren;
    }

    private void OnDisable()
    { 
        GameController.OnGameStarted -= PlaySiren;
        GameController.OnPlayerDeath -= ToggleSiren;
    }

    void PlaySiren()
    {
        sirenSource.Play();
    }

    void ToggleSiren()
    {
        sirenSource.Pause();
    }
}
