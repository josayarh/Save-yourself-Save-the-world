using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class EnemyController : SavableObject
{
    [SerializeField] private float speed;
    
    private bool hasBeenKilled = false;
    FSMSystem fsm = new FSMSystem();
    
    // Start is called before the first frame update
    void Start()
    {
        makeFSM();
    }

    private void FixedUpdate()
    {
        uint frameNumber = GameObjectStateManager.Instance.FrameNumber;
        
        if (frameSaveList.Count<=0 && frameNumber == 0)
        {
            id = new Guid();
            frameSaveList.Add(SaveFrame());
        }
        
        if (frameSaveList.Count <= frameNumber)
        {
            if(hasBeenKilled)
                Destroy(gameObject);
            else
            {
                fsm.CurrentState.Reason(GameManager.Instance.gameObject, gameObject);
                fsm.CurrentState.Act(GameManager.Instance.gameObject, gameObject);
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
        EnemyMoveState moveState = new EnemyMoveState(gameObject.transform, speed);
        fsm.AddState(moveState);
    }
    
    public List<string> FrameSaveList
    {
        set
        {
            frameSaveList = value;
            LoadFrame(value[0]);
        }
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

    private void OnDestroy()
    {
        frameSaveList.Add(SaveDiffFrame());
        GameObjectStateManager.Instance.addDynamicObject(id, GetType(),frameSaveList,0);
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