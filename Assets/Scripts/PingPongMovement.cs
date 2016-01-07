using UnityEngine;
using System.Collections;

public class PingPongMovement : MonoBehaviour
{
	//Direction to move
	public Vector3 MoveDir = Vector3.zero;

	//Speed to move - units per second
	public float Speed = 0.0f;

	//Distance to travel in world units (before inverting direction and turning back)
	public float TravelDistance = 0.0f;
	
	//Cached Transform
	private Transform thisTransform;
	
	// Use this for initialization
	IEnumerator Start () 
	{
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

		//Move
		while(true)
		{
			//Get new position based on speed and direction
			Vector3 distToTravel = MoveDir * Speed * Time.deltaTime;

			//Update position
			thisTransform.position += distToTravel;

			//Update distance traveled so far
			distanceTravelled += distToTravel.magnitude;

            //stop movement, when reach the end point
		    if (distanceTravelled > TravelDistance)
		    {
                float superfluousDist = distanceTravelled - TravelDistance;
                thisTransform.position -= MoveDir * superfluousDist;
                yield break;
		    }

			//Wait until next update
			yield return null;
		}
	}
}
