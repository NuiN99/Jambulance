using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform target;

    [SerializeField] float verticalOffset;

    private void Awake()
    {
        target = FindObjectOfType<Player>(true).transform;   
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
        //transform.position = new Vector3(target.position.x, target.position.y + verticalOffset, transform.position.z);
    }
}
