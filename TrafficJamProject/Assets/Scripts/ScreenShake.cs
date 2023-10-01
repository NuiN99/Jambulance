using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    
    float currentStrength;
    [SerializeField] float maxStrength = 10f;
    [SerializeField] float strengthMultiplier = 10f;
    [SerializeField] float duration = .25f;
    [SerializeField] AnimationCurve curve;
    [SerializeField] Vector3 anchorPos;

    private void Start()
    {
        anchorPos = transform.localPosition;
    }
    public IEnumerator AddStrength(float strength)
    {
        if(strength > maxStrength)
            strength = maxStrength;

        currentStrength += strength;

        if(currentStrength > maxStrength)
            currentStrength = maxStrength;
        if(currentStrength < 0)
            currentStrength = 0;

        yield return ShakeScreen();
        currentStrength -= strength;
    }

    public IEnumerator ShakeScreen()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            Vector3 startPos = Camera.main.transform.position;
            float shakeMult = 1f;
            /*
            if (ES3.KeyExists("ScreenShake"))
                shakeMult = ES3.Load<float>("ScreenShake");
            */
            elapsedTime += Time.deltaTime;
            float curveStrength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPos + currentStrength * curveStrength * shakeMult * strengthMultiplier * Time.deltaTime * Random.insideUnitSphere;

            yield return null;
            transform.position = startPos;
        }
    }

    public IEnumerator ShakeScreenNormal( (float,float) values)
    {
        float shakeDur = values.Item1;
        float strength = values.Item2;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDur)
        {
            //Vector3 startPos = transform.position;
            float shakeMult = 1f;
            /*
            if (ES3.KeyExists("ScreenShake"))
                shakeMult = ES3.Load<float>("ScreenShake");
            */
            elapsedTime += Time.deltaTime;
            float curveStrength = curve.Evaluate(elapsedTime / shakeDur);
            transform.localPosition = anchorPos + strength * curveStrength * shakeMult * strengthMultiplier * Time.deltaTime * Random.insideUnitSphere;

            yield return null;
            transform.localPosition = anchorPos;
        }

        transform.localPosition = anchorPos;
    }
}
