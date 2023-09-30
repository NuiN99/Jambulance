using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cai
{
    public class Player : MonoBehaviour
    {
        Car car;

        private void Awake()
        {
            car = GetComponent<Car>();
        }

        private void FixedUpdate()
        {
            float moveAxis = Input.GetAxis("Vertical");
            float rotateAxis = Input.GetAxis("Horizontal");

            if (moveAxis != 0f)
            {

                car.MoveInDirection(transform.up * moveAxis, car.moveSpeed);
            }

            car.RotateInDirection(rotateAxis, car.turnSpeed);
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + transform.up * 2f, Color.green);
        }
    }

}
