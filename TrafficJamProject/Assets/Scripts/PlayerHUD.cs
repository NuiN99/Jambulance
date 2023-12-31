using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] CustomSlider progressionSlider;

    [SerializeField] TextMeshProUGUI timeRemainingText;

    [SerializeField] TextMeshProUGUI titleText;

    [SerializeField] TextMeshProUGUI instructionsText;

    [SerializeField] GameObject gameOverText;
    [SerializeField] GameObject gameOverFadeSprite;
    [SerializeField] GameObject pauseMenu;

    [SerializeField] Button resetButton;
    [SerializeField] Button quitButton;

    [SerializeField] GameObject winGameFadeSprite;

    [SerializeField] GameObject highScoreText;

    [SerializeField] GameObject winScreen;
    [SerializeField] TextMeshProUGUI timeLeftAfterWin;

    private void OnEnable()
    {
        GameController.OnGameStarted += OnGameStarted;
        GameController.OnPlayerDeath += OnGameOver;
        GameController.OnPlayerWon += OnWinGame;
    }
    private void OnDisable()
    {
        GameController.OnGameStarted -= OnGameStarted;
        GameController.OnPlayerDeath -= OnGameOver;
        GameController.OnPlayerWon -= OnWinGame;
    }

    private void Start()
    {
        Tween.Scale(titleText.rectTransform, 1.25f, 2.5f, Ease.OutElastic, -1, CycleMode.Yoyo);
        Tween.EulerAngles(titleText.transform, new Vector3(0f, 0f, -10f), new Vector3(0f, 0f, 10f), 1.5f, Ease.InOutCubic, -1, CycleMode.Yoyo);

        Tween.Rotation(gameOverText.transform, new Vector3(0f, 0f, -5f), new Vector3(0f, 0f, 5f), 2f, Ease.InOutBounce, -1, CycleMode.Yoyo);
    }

    private void Awake()
    {
        Time.timeScale = 1f;

        DisableAll();

        titleText.gameObject.SetActive(true);
        instructionsText.gameObject.SetActive(true);
        highScoreText.gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        if (GameController.Instance.started && !GameController.Instance.won)
        {
            progressionSlider.UpdateValue(GameController.Instance.progress01);

            timeRemainingText.text = TimeRemainingText();
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) && GameController.Instance.started)
        {
            if(Time.timeScale == 0)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();

            }
            
           
        }
    }

    void DisableAll()
    {
        progressionSlider.gameObject.SetActive(false);
        timeRemainingText.gameObject.SetActive(false);

        titleText.gameObject.SetActive(false);
        instructionsText.gameObject.SetActive(false);

        gameOverText.SetActive(false);
        gameOverFadeSprite.SetActive(false);

        pauseMenu.gameObject.SetActive(false);

        resetButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        highScoreText.gameObject.SetActive(false);

        winScreen.SetActive(false);
    }

    void OnGameStarted()
    {
        DisableAll();

        progressionSlider.gameObject.SetActive(true);
        timeRemainingText.gameObject.SetActive(true);

        Tween.Scale(timeRemainingText.transform, timeRemainingText.transform.localScale * 1.25f, .25f, Ease.OutElastic, -1, CycleMode.Yoyo, 0, 0.75f);
    }

    void OnGameOver()
    {
        DisableAll();
        gameOverFadeSprite.SetActive(true);

        Tween.Alpha(gameOverFadeSprite.GetComponent<SpriteRenderer>(), 0.6f, 3f, Ease.Linear);

        Invoke("ShowGameOverItems", 3f);
    }

    void OnWinGame()
    {
        DisableAll();
        winGameFadeSprite.SetActive(true);

        timeLeftAfterWin.text = TimeRemainingText() + "s";

        Tween.Alpha(winGameFadeSprite.GetComponent<SpriteRenderer>(), 1f, 3f, Ease.Linear);

        Invoke("ShowWinScreen", 4f);
    }

    void ShowGameOverItems()
    {
        gameOverText.SetActive(true);
        resetButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    void ShowWinScreen()
    {
        Time.timeScale = 0;

        winScreen.SetActive(true);
    }

    string TimeRemainingText()
    {
        int minutes = Mathf.FloorToInt(GameController.Instance.timeRemaining / 60);
        int seconds = Mathf.FloorToInt(GameController.Instance.timeRemaining % 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
