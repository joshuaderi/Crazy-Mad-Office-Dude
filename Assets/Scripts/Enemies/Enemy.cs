﻿//Sets up FSM for enemy AI
//------------------------------------------------

using System.Collections;
using UnityEngine;

//------------------------------------------------
public class Enemy : MonoBehaviour
{
    //Enemy types
    public enum EnemyType
    {
        Drone = 0,
        ToughGuy = 1,
        Boss = 2
    }

    //Type of this enemy
    public EnemyType Type = EnemyType.Drone;
    
    //Current health of this enemy
    public int Health = 100;

    //Attack Damage - amount of damage this enemy deals to player when attacking
    public int AttackDamage = 10;

    //Recovery delay in seconds after launching an attack
    public float RecoveryDelay = 1.0f;

    //Enemy cached transform
    protected Transform ThisTransform;

    //------------------------------------------------
    //AI Properties

    //Reference to NavMesh Agent component
    protected NavMeshAgent Agent;

    //Reference to active PlayerController component for player
    protected PlayerController PlayerController;

    //Reference to Player Transform
    protected Transform PlayerTransform;

    //Total distance in Unity Units from current position that agent can wander when patrolling
    public float PatrolDistance = 10.0f;

    //Total distance enemy must be from player, in Unity Units, before chasing them (entering chase state)
    public float ChaseDistance = 10.0f;

    //Total distance enemy must be from player before attacking them
    public float AttackDistance = 0.1f;

    //Enum of states for FSM
    public enum EnemyState
    {
        Patrol = 0,
        Chase = 1,
        Attack = 2
    }

    //Current state of enemy - default is patrol
    public EnemyState ActiveState = EnemyState.Patrol;
    protected PingPongSpriteColor PingPongSpriteColor;
    public SpriteAnimator PatrolAnimator;
    public SpriteAnimator AttackAnimator;

    //------------------------------------------------
    //Called on object start
    protected virtual void Start()
    {
        PingPongSpriteColor = GetComponent<PingPongSpriteColor>();

        //Get NavAgent Component
        Agent = GetComponent<NavMeshAgent>();

        //Get Player Controller Component
        PlayerController = GameObject.Find("Player").GetComponentInChildren<PlayerController>();

        //Get Player Transform
        PlayerTransform = PlayerController.transform;

        //Get Enemy Transform
        ThisTransform = transform;

        //Set default state
        ChangeState(ActiveState);
    }

    //------------------------------------------------
    //Change AI State
    public void ChangeState(EnemyState state)
    {
        //Stops all AI Processing
        StopAllCoroutines();

        //Set new state
        ActiveState = state;

        //Activates new state
        switch (ActiveState)
        {
            case EnemyState.Attack:
                StartCoroutine(AI_Attack());
                SendMessage("Attack", SendMessageOptions.DontRequireReceiver); //Notify Game Object
                return;

            case EnemyState.Chase:
                StartCoroutine(AI_Chase());
                SendMessage("Chase", SendMessageOptions.DontRequireReceiver); //Notify Game Object
                return;

            case EnemyState.Patrol:
                StartCoroutine(AI_Patrol());
                SendMessage("Patrol", SendMessageOptions.DontRequireReceiver); //Notify Game Object
                return;
        }
    }

    //------------------------------------------------
    //AI Function to handle patrol behaviour for enemy
    //Can exit this state and enter chase
    IEnumerator AI_Patrol()
    {
        //Stop Agent
        Agent.Stop();

        //Loop forever while in patrol state
        while (ActiveState == EnemyState.Patrol)
        {
            //Get random destination on map
            Vector3 randomPosition = Random.insideUnitSphere * PatrolDistance;

            //Add as offset from current position
            randomPosition += ThisTransform.position;

            //Get nearest valid position
            NavMeshHit hit;
            bool nearestPointFound = NavMesh.SamplePosition(randomPosition, out hit, PatrolDistance, 1);

            //Set destination
            Agent.SetDestination(hit.position);

            //Set distance range between object and destination to classify as 'arrived'
            float ArrivalDistance = 2.0f;

            //Set timeout before new path is generated (5 seconds)
            float TimeOut = 5.0f;

            //Elapsed Time
            float ElapsedTime = 0;

            //Wait until enemy reaches destination or times-out, and then get new position
            while (Vector3.Distance(ThisTransform.position, hit.position) > ArrivalDistance &&
                   ElapsedTime < TimeOut)
            {
                //Update ElapsedTime
                ElapsedTime += Time.deltaTime;

                //Check if should enter chase state
                if (PlayerDistance < ChaseDistance)
                {
                    //Exit patrol and enter chase state
                    ChangeState(EnemyState.Chase);
                    yield break;
                }

                yield return null;
            }
        }
    }

    //------------------------------------------------
    //AI Function to handle chase behaviour for enemy
    //Can exit this state and enter either patrol or attack
    IEnumerator AI_Chase()
    {
        //Stop Agent
        Agent.Stop();

        //Loop forever while in chase state
        while (ActiveState == EnemyState.Chase)
        {
            //Set destination to player
            Agent.SetDestination(PlayerTransform.position);

            //Check distances and state exit conditions

            //If within attack range, then change to attack state
            if (PlayerDistance < AttackDistance)
            {
                ChangeState(EnemyState.Attack);
                yield break;
            }

            //If outside chase range, then revert to patrol state
            if (PlayerDistance > ChaseDistance)
            {
                ChangeState(EnemyState.Patrol);
                yield break;
            }

            //Wait until next frame
            yield return null;
        }
    }

    private float PlayerDistance
    {
        get { return Vector3.Distance(ThisTransform.position, PlayerTransform.position); }
    }

    //------------------------------------------------
    //AI Function to handle attack behaviour for enemy
    //Can exit this state and enter either patrol or chase
    IEnumerator AI_Attack()
    {
        //Stop Agent
        Agent.Stop();

        //Elapsed time - to calculate strike intervals
        float ElapsedTime = RecoveryDelay;

        //Loop forever while in chase state
        while (ActiveState == EnemyState.Attack)
        {
            //Update elapsed time
            ElapsedTime += Time.deltaTime;

            //Check distances and state exit conditions

            //If outside chase range, then revert to patrol state
            if (PlayerDistance > ChaseDistance)
            {
                ChangeState(EnemyState.Patrol);
                yield break;
            }

            //If within attack range, then change to attack state
            if (PlayerDistance > AttackDistance)
            {
                ChangeState(EnemyState.Chase);
                yield break;
            }

            //Make strike
            if (ElapsedTime >= RecoveryDelay)
            {
                //Reset elapsed time
                ElapsedTime = 0;
                SendMessage("Strike", SendMessageOptions.DontRequireReceiver);
            }

            //Wait until next frame
            yield return null;
        }
    }
}

//------------------------------------------------