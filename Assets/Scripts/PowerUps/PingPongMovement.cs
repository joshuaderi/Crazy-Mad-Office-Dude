using System;
using UnityEngine;
using System.Collections;

public class PingPongMovement : MonoBehaviour
{
    //Direction to move
    public Vector3 MoveDir = Vector3.zero;

    /// <summary>
    /// Speed to move - units per second;
    /// should be positive
    /// </summary>
    public float Speed = 0.0f;

    //Distance to travel in world units (before inverting direction and turning back)
    public float TravelDistance = 0.0f;

    //Cached Transform
    private Transform thisTransform;

    private Vector3 startPos;
    private Vector3 targetPos;

    // Use this for initialization
    void Start()
    {
        if (Speed < 0) throw new InvalidOperationException("Speed should be positive");

        //Get cached transform
        thisTransform = transform;

        //calc end points
        startPos = thisTransform.position;
        targetPos = startPos + MoveDir * TravelDistance;

        //Start movement
        StartCoroutine(MoveBackAndForth());
    }

    //Travel full distance in direction, from current position
    IEnumerator MoveBackAndForth()
    {
        while (true)
        {
            float step = Speed * Time.deltaTime;
            thisTransform.position = Vector3.MoveTowards(thisTransform.position, targetPos, step);

            if (thisTransform.position == targetPos)
            {
                //swap positions
                var temp = targetPos;
                targetPos = startPos;
                startPos = temp;
            }

            //Wait until next update
            yield return null;
        }
    }
}
