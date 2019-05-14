using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyWanderState : FSMState
{
    private static float CIRCLE_RADIUS = 1.5f;
    private static float CIRCLE_DISTANCE = 3;
    private static float TIME_TILL_DIRECTION_CHANGE = 2;
    private Transform enemyPosition;
    private Vector3 velocity;
    private float speed;
    private float MaxSpeed = 3;
    private float MaxForce = 1.5f;
    private float TimeSinceLastUpdate;
    Vector3 steering = new Vector3();

    public EnemyWanderState(Transform enemyPosition, Vector3 velocity, float speed)
    {
        this.enemyPosition = enemyPosition;
        this.velocity = velocity;
        this.speed = speed;

        stateID = StateID.EnemyWanderStateID;
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
    }

    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();
    }
    
    /***
     * 
     */
    public override void Reason(GameObject player, GameObject npc)
    {
        if (Time.time - TimeSinceLastUpdate >= TIME_TILL_DIRECTION_CHANGE)
        {
            steering = Vector3.ClampMagnitude(applyDisplacement(), MaxForce);
            TimeSinceLastUpdate = Time.time;
        }

        velocity = Vector3.ClampMagnitude(velocity+steering,MaxSpeed);
        
    }

    public override void Act(GameObject player, GameObject npc)
    {
        enemyPosition.position += velocity*Time.fixedDeltaTime;
        enemyPosition.forward = velocity.normalized;

    }

    private Vector3 calcCircle()
    {
        Vector3 circlecenter = velocity;
        
        circlecenter.Normalize();
        circlecenter *= CIRCLE_DISTANCE;
        
        return circlecenter;
    }

    private Vector3 applyDisplacement()
    {
        Vector3 circleCenter = calcCircle();
        Vector3 randomPoint = Random.insideUnitCircle;
        
        randomPoint.Normalize();
        randomPoint *= CIRCLE_RADIUS;

        return circleCenter + Quaternion.LookRotation(velocity) * randomPoint;
    }
}
