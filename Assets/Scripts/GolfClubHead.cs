using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfClubHead : MonoBehaviour
{

    private Vector3 posI;
    private Vector3 posF;
    private Vector3 vel;
    private float velMag;
    public float maxVel;

    void Start()
    {
        //Store initial and final position at start
        posI = posF = transform.position;
    }

    private void FixedUpdate()
    {
        //Update initial and final positions
        posI = posF;
        posF = transform.position;
    }

    public Vector3 getVelocity()
    {
        //Calculate velocity
        vel = (posF - posI) / Time.deltaTime;
        velMag = vel.magnitude;
        //Limit velocity
        if (velMag > maxVel) 
        {
            vel = vel * (maxVel / velMag);
        }
        return vel;
    }
}
