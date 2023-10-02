using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float collisionShakeDuration;
    [SerializeField] float strengthMultiplier = 10f;
    [SerializeField] AnimationCurve curve;

    CameraController camControl;

    public bool shaking;

    private void Awake()
    {
        camControl = GetComponent<CameraController>();
    }

    public IEnumerator ShakeScreen(float strength)
    {
        shaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < collisionShakeDuration)
        {
            Vector3 startPos = new(camControl.targetPos.x, camControl.targetPos.y, transform.position.z);

            float shakeMult = 1f;

            //if (ES3.KeyExists("ScreenShake"))
                //shakeMult = ES3.Load<float>("ScreenShake");

            elapsedTime += Time.deltaTime;
            float curveStrength = curve.Evaluate(elapsedTime / collisionShakeDuration);
            transform.position = startPos + (strength * curveStrength * shakeMult * strengthMultiplier * Time.deltaTime * Random.insideUnitSphere);

            yield return null;
            transform.position = startPos;
        }
        shaking = false;
    }
}
