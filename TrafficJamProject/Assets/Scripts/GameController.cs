using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    Player player;
    public bool started = false;

    public float endPosY;
    public float distToEnd;
    public float progress01;

    public float startYPos;

    [SerializeField] GameObject hospitalPrefab;

    public float startTime = 90f;
    public float timeRemaining;

    public static GameController Instance;

    public delegate void GameStarted();
    public static event GameStarted OnGameStarted;

    public delegate void PlayerWon();
    public static event PlayerWon OnPlayerWon;

    public delegate void PlayerDied();
    public static event PlayerDied OnPlayerDeath;

    public bool won = false;

    public bool gameOver = false;
    bool triggeredGameOver = false;

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

        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
        Physics2D.queriesStartInColliders = false;
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        player.enabled = false;
        player.gameObject.AddComponent<Enemy>();

        timeRemaining = startTime;
    }

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
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
        if (!started && Input.anyKeyDown && FindObjectOfType<OverUI>().overUI == false)
        {
            started = true;
            CarsController carsController = FindObjectOfType<CarsController>();
            carsController.SwitchStats(player.GetComponent<Car>(), carsController.playerStats);
            Destroy(player.GetComponent<Enemy>());
            player.enabled = true;

            startYPos = player.transform.position.y;
            endPosY += startYPos;

            Instantiate(hospitalPrefab, new Vector2(3.25f, endPosY+30), Quaternion.identity);

            OnGameStarted?.Invoke();
        }
        if (gameOver && !triggeredGameOver)
        {
            if (!won)
            {
                triggeredGameOver = true;
                OnPlayerDeath?.Invoke();
            }
        }
    }

    private void LateUpdate()
    {
        distToEnd = endPosY - player.transform.position.y;
        progress01 = 1 - (distToEnd / endPosY);
        progress01 = Mathf.Clamp01(progress01);

        if(!won && progress01 >= 1)
        {
            won = true;
            OnPlayerWon?.Invoke();
            print("YOU WON GOOD JOB");


            int score = ((int)startTime - (int)timeRemaining);
            StartCoroutine(FindObjectOfType<Leaderboard>().SubmitScoreRoutine(score));
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


   
}

