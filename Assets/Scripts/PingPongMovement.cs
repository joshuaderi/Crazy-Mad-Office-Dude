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
	
	// Use this for initialization
	IEnumerator Start () 
	{
	    if (Speed < 0) throw new InvalidOperationException("Speed should be positive");

	    //Get cached transform
		thisTransform = transform;

		//Loop forever
		while(true)
		{
			//Invert direction
			MoveDir = MoveDir * -1;

			//Start movement
			yield return StartCoroutine(Travel());
		}
	}
	
	//Travel full distance in direction, from current position
	IEnumerator Travel()
	{
		//Distance traveled so far
		float distanceTravelled = 0;

	    bool hasReachEndPoint = false;

		//Move
		while(!hasReachEndPoint)
		{
			//Get new position based on speed and direction
			Vector3 distToTravel = MoveDir * Speed * Time.deltaTime;
            
            //prevent from moving beyond the end point
            if (distToTravel.magnitude + distanceTravelled >= TravelDistance)
		    {
		        hasReachEndPoint = true;
		        distToTravel = (TravelDistance - distanceTravelled) * MoveDir;
		    }

			//Update position
			thisTransform.position += distToTravel;

			//Update distance traveled so far
			distanceTravelled += distToTravel.magnitude;

			//Wait until next update
			yield return null;
		}
	}
}
