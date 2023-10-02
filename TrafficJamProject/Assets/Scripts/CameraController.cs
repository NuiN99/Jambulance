using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform player;
    public float verticalOffset;
    ScreenShake sc;

    public Vector3 targetPos;

    private void Awake()
    {
        sc = GetComponent<ScreenShake>();
        player = FindObjectOfType<Player>(true).transform;   
    }

    private void LateUpdate()
    {
        targetPos = player.position + Vector3.up * verticalOffset;

        if(!sc.shaking)
            transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }
}
