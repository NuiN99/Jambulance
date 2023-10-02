using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LootLocker.Requests; 

public class Leaderboard : MonoBehaviour
{
    public float playerFastestTime = 999;

    [SerializeField] TextMeshProUGUI highscoreText;

    static readonly string leaderboardKey = "Leaderboard";
    int leaderboardID = 18088;

    private void Start()
    {
        if (ES3.KeyExists("Highscore"))
        {
            playerFastestTime = ES3.Load<float>("Highscore");

            int minutes = Mathf.FloorToInt(playerFastestTime / 60);
            int seconds = Mathf.FloorToInt(playerFastestTime % 60);
            string milli = Mathf.FloorToInt((playerFastestTime * 1000) % 1000).ToString("0");
            highscoreText.text = string.Format("{0:0}:{1:00}:{2:0}", minutes, seconds, milli);
        }

        StartCoroutine(LoginRoutine());
    }

    private void OnEnable()
    {
        GameController.OnPlayerWon += SaveScore;
    }
    private void OnDisable()
    {
        GameController.OnPlayerWon -= SaveScore;
    }

    void SaveScore()
    {
        float time = GameController.Instance.startTime - GameController.Instance.timeRemaining;
        if (time < playerFastestTime)
        {
            playerFastestTime = time;
            ES3.Save("Highscore", playerFastestTime);
        }
    }

    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Player Was logged in");
                ES3.Save("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("could not start session");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
}
