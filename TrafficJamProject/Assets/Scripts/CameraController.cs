using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    Transform player;
    public float verticalOffset;
    ScreenShake sc;

    [SerializeField] float smoothSpeed;

    public Vector3 targetPos;

    [SerializeField] float minCamSize;
    [SerializeField] float maxCamSize;
    [SerializeField] float cameraScaleExponent;

    [SerializeField] float speedFactor;

    float targetZoom = 5f;

    private void Awake()
    {
        cam = Camera.main;
        sc = GetComponent<ScreenShake>();
        player = FindObjectOfType<Player>(true).transform;   
    }

    private void OnEnable()
    {
        GameController.OnGameStarted += ZoomOut;
    }
    private void OnDisable()
    {
        GameController.OnGameStarted -= ZoomOut;
    }

    void ZoomOut() 
    {
        Tween.CameraOrthographicSize(cam, targetZoom, 1f, Ease.InOutQuad);
    }

    private void LateUpdate()
    {
        //normal smoothing
        /*Vector3 desiredPos = player.position + new Vector3(0, verticalOffset);
        targetPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);*/

        //bad scaling code
        /*if(player.TryGetComponent(out Rigidbody2D playerRB))
        {
            float speedFactor = playerRB.velocity.magnitude / maxCamSize;
            float lerpFactor = Mathf.Pow(speedFactor, cameraScaleExponent);

            cam.orthographicSize = Mathf.Lerp(minCamSize, maxCamSize, lerpFactor);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minCamSize, maxCamSize);
        }*/


        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        float verticalOffsetTemp = playerRigidbody.velocity.y * speedFactor + verticalOffset;
        Vector3 desiredPos = player.position + new Vector3(0, verticalOffsetTemp);
        targetPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        if (!sc.shaking)
            transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }
}
