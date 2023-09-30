using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyCar : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    Rigidbody2D mRigidBody2D; 
    // Start is called before the first frame update
    void Start()
    {
        mRigidBody2D =  GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //also need to check for impacts!
        mRigidBody2D.velocity = transform.up * -(speed);

        if(transform.position.y < -1)
        {
            Destroy(gameObject);
        }

    }
}
