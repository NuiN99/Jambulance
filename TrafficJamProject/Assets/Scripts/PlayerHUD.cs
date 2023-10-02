using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] CustomSlider progressionSlider;
    GameController controller;

    private void Awake()
    {
        controller = FindObjectOfType<GameController>();
    }

    private void LateUpdate()
    {
        progressionSlider.UpdateValue(controller.progress01);
    }
}
