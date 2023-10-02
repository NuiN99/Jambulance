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

    public float startYPos;

    [SerializeField] float startTime = 90f;
    public float timeRemaining;

    public static GameController Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }


        Physics2D.queriesStartInColliders = false;
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        player.enabled = false;
        player.AddComponent<Enemy>();

        timeRemaining = startTime;
    }

    private void Update()
    {
        if (started)
        {
            timeRemaining -= Time.deltaTime;
            if(timeRemaining <= 0)
            {
                timeRemaining = 0;
                print("RAN OUT OF TIME");
            }
        }
        if (!started && Input.anyKeyDown)
        {
            started = true;
            CarsController carsController = FindObjectOfType<CarsController>();
            carsController.SwitchStats(player.GetComponent<Car>(), carsController.playerStats);
            Destroy(player.GetComponent<Enemy>());
            player.enabled = true;

            startYPos = player.transform.position.y;
            endPosY += startYPos;
        }
    }

    private void LateUpdate()
    {
        distToEnd = endPosY - player.transform.position.y;
        progress01 = 1 - (distToEnd / endPosY);
        progress01 = Mathf.Clamp01(progress01);

        if(progress01 >= 1)
        {
            print("YOU WON GOOD JOB");
        }
    }
}

