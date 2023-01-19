using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationMover2 : MonoBehaviour
{
    private Transform blop;
    private bool arrivedAtTree;
    private bool animationDone;
    private int initialX;
    private int initialZ;
 
    void Start()
    {
        blop = GetComponent<Transform>();
        arrivedAtTree = false;
        animationDone = false;
        initialX = (int)blop.position.x;
        initialZ = (int)blop.position.z;
    }

    void FixedUpdate()
    {
        if(!animationDone)
        {
            if(!arrivedAtTree)
            {
                if((int)blop.position.x != 0 || (int)blop.position.z != 0)
                {
                    blop.Translate(new Vector3(-initialX,0,-initialZ)*Time.deltaTime);
                }
                else
                {
                    arrivedAtTree = true;
                }
            }
            else if(arrivedAtTree)
            {
                if((int)blop.position.x != initialX || (int)blop.position.z != initialZ)
                {
                    blop.Translate(new Vector3(initialX,0,initialZ)*Time.deltaTime);
                }
                else
                {
                    animationDone = true;
                }
            }
        }
    }
}
