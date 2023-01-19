using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationMover : MonoBehaviour
{
    private Rigidbody ball;
 
    void Start()
    {
        ball = GetComponent<Rigidbody>();
    }
 
    void FixedUpdate()
    {
        ball.AddForce(-(ball.transform.position.x/2) ,0 , -(ball.transform.position.z/2));
    }
}
