using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : FSMState
{
    private Transform gunTipPosition;
    private Vector3 velocity;
    
    private float max_velocity = 1.5f;
    private float max_force = 1.5f;
    private float mass = 2;
    private float max_speed = 3;
    
    private GameObject laserPrefab;

    private Vector3 steering;
    private float timer = 0.0f;


    public EnemyAttackState(Transform gunTipPosition, Vector3 velocity, GameObject laserPrefab)
    {
        this.gunTipPosition = gunTipPosition;
        this.velocity = velocity;
        this.laserPrefab = laserPrefab;

        stateID = StateID.EnemyAttackStateID;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        Vector3 dersired_velocity = Vector3.Normalize(player.transform.position - npc.transform.position) * max_velocity;
        steering = dersired_velocity - velocity;
        steering = Vector3.ClampMagnitude(steering, max_force);
        steering = steering / mass;

        velocity = Vector3.ClampMagnitude(velocity + steering, max_speed);
            
        timer += Time.deltaTime;
    }

    public override void Act(GameObject player, GameObject npc)
    {
        npc.transform.position += velocity*Time.fixedDeltaTime;
        npc.transform.forward = velocity.normalized;

        float angle = Vector3.Angle(npc.transform.forward, player.transform.position - npc.transform.position);

        if (Mathf.Abs(angle) < 90 && timer > 1.0f)
        {
            GameObject.Instantiate(laserPrefab, gunTipPosition.position, gunTipPosition.rotation);
            timer = 0.0f;
        }
    }
}
