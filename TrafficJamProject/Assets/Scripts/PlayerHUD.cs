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

    [SerializeField] Button resetButton;
    [SerializeField] Button quitButton;

    [SerializeField] GameObject winGameFadeSprite;

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
        DisableAll();

        titleText.gameObject.SetActive(true);
        instructionsText.gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        if (GameController.Instance.started)
        {
            progressionSlider.UpdateValue(GameController.Instance.progress01);

            int minutes = Mathf.FloorToInt(GameController.Instance.timeRemaining / 60);
            int seconds = Mathf.FloorToInt(GameController.Instance.timeRemaining % 60);
            timeRemainingText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
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

        resetButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false); 
    }

    void OnGameStarted()
    {
        DisableAll();

        progressionSlider.gameObject.SetActive(true);
        timeRemainingText.gameObject.SetActive(true);
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

        Tween.Alpha(winGameFadeSprite.GetComponent<SpriteRenderer>(), 1f, 5f, Ease.Linear);

        Invoke("ShowWinGameScreen", 5f);
    }

    void ShowGameOverItems()
    {
        gameOverText.SetActive(true);
        resetButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    void ShowWinGameScreen()
    {
        SceneManager.LoadScene("WinnerWinner");
    }
}
