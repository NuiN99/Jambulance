using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Player player;
    public bool started = false;

    public float endPosY;
    public float distToEnd;
    public float progress01;


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

    private void LateUpdate()
    {
        distToEnd = endPosY - player.transform.position.y;
        progress01 = 1 - (distToEnd / endPosY);
        progress01 = Mathf.Clamp01(progress01);
    }
}

