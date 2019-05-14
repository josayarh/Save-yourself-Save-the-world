using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class PlayerBotController : Bot
{
    private float speed;
    private float rotateSpeedH;
    private float rotateSpeedV;
    private float fireRate;
    private float turnSpeed;
    private float accelerationFactor;
    [SerializeField] private Transform gunTipPosition;
    
    private Rigidbody rigidBody;
    private GameObject laser;
    
    FSMSystem fsm = new FSMSystem();
    bool isAiOn = false;
    
    private float timer = 0.0f;

    private AttackState attackState;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        laser = Resources.Load("Prefabs/shot_prefab") as GameObject;
        makeFSM();
    }
    
    private void makeFSM()
    {
        EnemyWanderState wanderState = new EnemyWanderState(gameObject.transform, Vector3.forward*speed);
        wanderState.AddTransition(Transition.Wander_Attack, StateID.EnemyAttackStateID);
        
        attackState = new AttackState(gunTipPosition,Vector3.forward*speed, 
            Resources.Load("Prefabs/shot_prefab") as GameObject);
        attackState.Target = GameManager.Instance.Player;
        attackState.AddTransition(Transition.Attack_Wander, StateID.EnemyWanderStateID);
        
        fsm.AddState(wanderState);
        fsm.AddState(attackState);
    }

    public override void FixedUpdate()
    {
        uint frameNumber = GameObjectStateManager.Instance.FrameNumber;
        
        if (frameSteps.Count <= frameNumber)
        {
            isAiOn = true;
            fsm.CurrentState.Reason(GameManager.Instance.Player, gameObject);
            fsm.CurrentState.Act(GameManager.Instance.Player, gameObject);
            frameSteps.Add(SaveDiffFrame());
        }
        else
        {
            if (frameNumber == 0)
            {
                LoadFrame(frameSteps[(int)frameNumber]);
            }
            else
            {
                LoadDiffFrame(frameSteps[(int)frameNumber]);
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        Debug.Log(other.gameObject.name);
        Destroy(this);
    }
    
    public string SaveDiffFrame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        
        PlayerDiffFrameData frameData = new PlayerDiffFrameData();

        frameData.position = VectorArrayConverter.vector3ToArray(transform.position);
        frameData.rotation = VectorArrayConverter.vector3ToArray(transform.rotation.eulerAngles);
        
        bf.Serialize(ms,frameData);

        return Convert.ToBase64String(ms.ToArray());
    }

    public override void LoadFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();
        PlayerBaseFrameData data = (PlayerBaseFrameData)bf.Deserialize(mf);
        
        id = new Guid(data.id);

        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));
    }

    public override void LoadDiffFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();
        PlayerDiffFrameData data = (PlayerDiffFrameData)bf.Deserialize(mf);

        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && isAiOn && fsm.CurrentState.ID == StateID.EnemyWanderStateID)
        {
            fsm.PerformTransition(Transition.Wander_Attack);
            attackState.Target = other.gameObject;
        }
    }
}
