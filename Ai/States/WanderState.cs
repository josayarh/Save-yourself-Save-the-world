using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class WanderState : FSMState
{
    private static float TIME_TILL_DIRECTION_CHANGE = 2;
    
    private Transform enemyPosition;
    private Vector3 velocity;
    private NpcType typeToDetect;
    private float detectrange;
    
    private float MaxSpeed = 3;
    private float TimeSinceLastUpdate;
    Vector3 steering = new Vector3();
    private NPCDetector npcDetector;

    private WanderBehaviour wanderBehaviour = new WanderBehaviour();

    public WanderState(Transform enemyPosition, Vector3 velocity, NpcType typeToDetect, float detectrange)
    {
        this.enemyPosition = enemyPosition;
        this.velocity = velocity;
        this.detectrange = detectrange;
        this.typeToDetect = typeToDetect;

        npcDetector = new NPCDetector();
        stateID = StateID.EnemyWanderStateID;
    }
    
    public override void Reason(GameObject player, GameObject npc)
    {
        GameObject target = npcDetector.getNpcInRange(typeToDetect, npc.transform.position, detectrange);

        if (target != null)
        {
            BaseAI baseAi = npc.GetComponent<BaseAI>();
            baseAi.wanderAttackTransistion(target);
        }
        else
        {
            if (Time.time - TimeSinceLastUpdate >= TIME_TILL_DIRECTION_CHANGE)
            {
                steering = wanderBehaviour.getSteeringForce(velocity);
                TimeSinceLastUpdate = Time.time;
            }

            velocity = Vector3.ClampMagnitude(velocity+steering,MaxSpeed);
        }
    }

    public override void Act(GameObject player, GameObject npc)
    {
        enemyPosition.position += velocity*Time.fixedDeltaTime;
        enemyPosition.forward = velocity.normalized;

    }
}
