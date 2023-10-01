using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Player player;
    public bool started = false;
    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        player.enabled = false;
        player.AddComponent<Enemy>();
    }

    private void Update()
    {
        if (!started && Input.anyKeyDown)
        {
            started = true;
            CarsController carsController = FindObjectOfType<CarsController>();
            carsController.SwitchStats(player.GetComponent<Car>(), carsController.playerStats);
            Destroy(player.GetComponent<Enemy>());
            player.enabled = true;


        }
    }
}

