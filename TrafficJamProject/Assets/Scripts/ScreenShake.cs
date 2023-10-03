using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float collisionShakeDuration;
    [SerializeField] float strengthMultiplier = 10f;
    [SerializeField] AnimationCurve curve;

    CameraController camControl;

    public bool shaking;

    public static ScreenShake Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        camControl = GetComponent<CameraController>();
    }

    public IEnumerator ShakeScreenRoutine(float strength, float dur)
    {
        if (strength >= 0.25f)
        {
            shaking = true;
            float elapsedTime = 0f;

            while (elapsedTime < dur)
            {
                Vector3 startPos = new(camControl.targetPos.x, camControl.targetPos.y, transform.position.z);

                float shakeMult = 1f;

                elapsedTime += Time.deltaTime;
                float curveStrength = curve.Evaluate(elapsedTime / dur);
                float calculatedStrength = (strength * curveStrength * shakeMult * strengthMultiplier * Time.deltaTime);
                transform.position = startPos + Random.insideUnitSphere * Mathf.Clamp(calculatedStrength, 0f, .75f);

                yield return null;
                transform.position = startPos;
            }
            shaking = false;
        }

        
    }

    public void ShakeScreen(float strength, float dur)
    {
        StartCoroutine(ShakeScreenRoutine(strength, dur));
    }
}
