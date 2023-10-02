using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.Rendering;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public float playerFastestTime = 999;

    [SerializeField] TextMeshProUGUI playerNames;
    [SerializeField] TextMeshProUGUI playerScores;

    [SerializeField] TextMeshProUGUI highscoreText;

    [SerializeField] TMP_InputField playerNameInput;
 
    static readonly string leaderboardKey = "Leaderboard";

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

        StartCoroutine(SetUpRoutine());
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

    public IEnumerator SubmitScoreRoutine(int score)
    {
        bool done = false;
        string playerID = ES3.Load<string>("PlayerID");

        LootLockerSDKManager.SubmitScore(playerID, score, leaderboardKey, (response) =>
        {
            if (response.success)
            {
                Debug.Log("uploaded score");
            }
            else
            {
                Debug.LogError("Failed" + response.errorData.message);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);   
    }

    public IEnumerator FetchTopHighscoresRoutine()
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardKey, 10, 0, (response) =>
        {
            if(response.success)
            {
                string tempPlayerNames = string.Empty;
                string tempPlayerScores = string.Empty;

                LootLockerLeaderboardMember[] members = response.items;

                for (int i = 0; i < members.Length; i++)
                {
                    if (members[i].player.name != "")
                    {
                        tempPlayerNames += $"{i + 1}. " + members[i].player.name;
                    }
                    else
                    {
                        tempPlayerNames += members[i].player.id;
                    }
                    tempPlayerScores += members[i].score + " seconds" + "\n";
                    tempPlayerNames += "\n";
                }
                done = true;
                playerNames.text = tempPlayerNames;
                playerScores.text = tempPlayerScores;
            }
            else
            {
                Debug.LogError("Failed" + response.errorData.message);
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public void SetPlayerName()
    {
        LootLockerSDKManager.SetPlayerName(playerNameInput.text, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Set player name");
            }
            else
            {
                Debug.LogError("could not set player name" +  response.errorData.message);
            }
        });
    }

    IEnumerator SetUpRoutine()
    {
        yield return LoginRoutine();
        yield return FetchTopHighscoresRoutine();
    }
}
