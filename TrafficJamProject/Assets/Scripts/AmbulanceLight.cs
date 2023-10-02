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
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastBlink > blinkRate)
        {
            lastBlink = Time.time;
            SwapLights();
        }
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
