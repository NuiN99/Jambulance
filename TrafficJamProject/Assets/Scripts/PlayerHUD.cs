using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] CustomSlider progressionSlider;

    [SerializeField] TextMeshProUGUI timeRemainingText;

    private void LateUpdate()
    {
        progressionSlider.UpdateValue(GameController.Instance.progress01);

        int minutes = Mathf.FloorToInt(GameController.Instance.timeRemaining / 60);
        int seconds = Mathf.FloorToInt(GameController.Instance.timeRemaining % 60);
        timeRemainingText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
