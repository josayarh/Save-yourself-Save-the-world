using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class EnemyController : SavableObject, IPoolableObject, BaseAI
{
    [SerializeField] private float speed;
    [SerializeField] private Transform guntipPosition;
    [SerializeField] private float detectRange;
    
    
    private bool hasBeenKilled = false;
    FSMSystem fsm;
    bool isAiOn = false;
    
    private AttackState attackState;

    public void OnPoolCreation()
    {
        makeFSM();
    }

    private void FixedUpdate()
    {
        uint frameNumber = GameObjectStateManager.Instance.FrameNumber;
        
        if (frameSaveList.Count<=0)
        {
            id = Guid.NewGuid();
            frameSaveList.Add(SaveFrame());
        }
        
        if (frameSaveList.Count <= frameNumber)
        {
            if(hasBeenKilled)
                Destroy();
            else
            {
                isAiOn = true;
                
                if( fsm.CurrentStateID == StateID.EnemyWanderStateID
                    && Vector3.Distance(GameManager.Instance.Player.transform.position, transform.position) < detectRange)
                    wanderAttackTransistion(GameManager.Instance.Player);
                
                fsm.CurrentState.Reason(GameManager.Instance.Player, gameObject);
                fsm.CurrentState.Act(GameManager.Instance.Player, gameObject);
                frameSaveList.Add(SaveDiffFrame());
            }
        }
        else
        {
            if (frameNumber == 0)
            {
                LoadFrame(frameSaveList[(int)frameNumber]);
            }
            else
            {
                LoadDiffFrame(frameSaveList[(int)frameNumber]);
            }
        }
    }

    private void makeFSM()
    {
        fsm = new FSMSystem();
        
        WanderState wanderState = new WanderState(gameObject.transform, Vector3.forward*speed,
            NpcType.PlayerBot, detectRange);
        wanderState.AddTransition(Transition.Wander_Attack, StateID.EnemyAttackStateID);
        
        attackState = new AttackState(guntipPosition,Vector3.forward*speed, 
            Resources.Load("Prefabs/shot_prefab") as GameObject, id);
        attackState.Target = GameManager.Instance.Player;
        attackState.AddTransition(Transition.Attack_Wander, StateID.EnemyWanderStateID);
        
        fsm.AddState(wanderState);
        fsm.AddState(attackState);
    }

    public void wanderAttackTransistion(GameObject target)
    {
        attackState.Target = target;
        fsm.PerformTransition(Transition.Wander_Attack);
    }

    public void attackWanderTransition()
    {
        fsm.PerformTransition(Transition.Attack_Wander);
    }

    public List<string> FrameSaveList
    {
        set
        {
            frameSaveList = value;
            LoadFrame(value[0]);
        }
    }

    public void OnRelease()
    {
        if (id != Guid.Empty)
        {
            fsm = null;
            frameSaveList.Add(SaveDiffFrame());
            GameObjectStateManager.Instance.addDynamicObject(id, GetType(), frameSaveList, 0);
            frameSaveList = new List<string>();
        }
    }

    public void Destroy()
    {
        Pool.Instance.release(gameObject, PoolableTypes.Enemy);
    }

    public override string SaveFrame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        
        EnemyFrameData frameData = new EnemyFrameData();

        frameData.id = new Byte[id.ToByteArray().Length];
        id.ToByteArray().CopyTo(frameData.id,0);

        frameData.position = VectorArrayConverter.vector3ToArray(transform.position);
        frameData.rotation = VectorArrayConverter.vector3ToArray(transform.rotation.eulerAngles);
        frameData.hasBeenKilled = hasBeenKilled;
        
        bf.Serialize(ms,frameData);

        return Convert.ToBase64String(ms.ToArray());
    }

    public override string SaveDiffFrame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        
        EnemyDiffFrameData frameData = new EnemyDiffFrameData();

        frameData.position = VectorArrayConverter.vector3ToArray(transform.position);
        frameData.rotation = VectorArrayConverter.vector3ToArray(transform.rotation.eulerAngles);
        frameData.hasBeenKilled = hasBeenKilled;
        
        bf.Serialize(ms,frameData);

        return Convert.ToBase64String(ms.ToArray());
    }

    public void LoadFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();

        EnemyFrameData data = (EnemyFrameData)bf.Deserialize(mf);
        
        id = new Guid(data.id);

        hasBeenKilled = data.hasBeenKilled;
        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));
    }
    
    public void LoadDiffFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();
        EnemyDiffFrameData data = (EnemyDiffFrameData)bf.Deserialize(mf);

        hasBeenKilled = data.hasBeenKilled;
        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));
    }

    public bool HasBeenKilled
    {
        get => hasBeenKilled;
        set => hasBeenKilled = value;
    }
}

[Serializable]
struct EnemyFrameData
{
    public byte[] id;
    public bool hasBeenKilled;
    public float[] position;
    public float[] rotation;
}

[Serializable]
struct EnemyDiffFrameData
{
    public float[] position;
    public float[] rotation;
    public bool hasBeenKilled;
}