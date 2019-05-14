using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyWanderState : FSMState
{
    private static float TIME_TILL_DIRECTION_CHANGE = 2;
    private Transform enemyPosition;
    private Vector3 velocity;
    private float MaxSpeed = 3;
    private float TimeSinceLastUpdate;
    Vector3 steering = new Vector3();

    private WanderBehaviour wanderBehaviour = new WanderBehaviour();

    public EnemyWanderState(Transform enemyPosition, Vector3 velocity)
    {
        this.enemyPosition = enemyPosition;
        this.velocity = velocity;

        stateID = StateID.EnemyWanderStateID;
    }
    
    public override void Reason(GameObject player, GameObject npc)
    {
        if (Time.time - TimeSinceLastUpdate >= TIME_TILL_DIRECTION_CHANGE)
        {
            steering = wanderBehaviour.getSteeringForce(velocity);
            TimeSinceLastUpdate = Time.time;
        }

        velocity = Vector3.ClampMagnitude(velocity+steering,MaxSpeed);
        
    }

    public override void Act(GameObject player, GameObject npc)
    {
        enemyPosition.position += velocity*Time.fixedDeltaTime;
        enemyPosition.forward = velocity.normalized;

    }
}
