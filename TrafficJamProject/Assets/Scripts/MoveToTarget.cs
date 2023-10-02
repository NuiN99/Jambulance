using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = target.position;
    }
}
