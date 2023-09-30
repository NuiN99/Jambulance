using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driving : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void MoveInDirection(Vector3 dir, float speed)
    {
        rb.AddForce(speed * dir);
    }
    void RotateInDirection(float dir, float speed)
    {
        float targetAngle = transform.eulerAngles.z + (speed * rb.velocity.magnitude * - dir) ;
        rb.MoveRotation(targetAngle);
    }

    private void FixedUpdate()
    {
        float moveAxis = Input.GetAxis("Vertical");
        float rotateAxis = Input.GetAxis("Horizontal");

        if(moveAxis != 0f)
        {
            MoveInDirection(transform.up * moveAxis, moveSpeed);
        }

        RotateInDirection(rotateAxis, turnSpeed);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up * 2f, Color.green);
    }
}
