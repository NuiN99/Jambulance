using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbulanceLight : MonoBehaviour
{
    [SerializeField] GameObject leftLight;
    [SerializeField] GameObject rightLight;

    [SerializeField] float blinkRate;
    float lastBlink = 0f;
    bool toggle = false;

    Health health;

    private void Start()
    {
        leftLight.SetActive(false);
        rightLight.SetActive(false);

        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        GameController.OnGameStarted += RenableLights;   
    }
    private void OnDisable()
    {
        GameController.OnGameStarted -= RenableLights;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.started && Time.time - lastBlink > blinkRate)
        {
            lastBlink = Time.time;
            SwapLights();
        }
    }

    void RenableLights()
    {
        leftLight.SetActive(true);
        rightLight.SetActive(true);
    }

    void SwapLights()
    {
        if (health.health > 0)
        {
            toggle = !toggle;
            leftLight.SetActive(toggle);
            rightLight.SetActive(!toggle);
        }
        else
        {
            leftLight.SetActive(false);
            rightLight.SetActive(false);
        }
    }
}
