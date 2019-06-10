using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulletController : SavableObject, IPoolableObject
{
    private uint creationFrameNumber;
    [SerializeField] private float speed;
   
    public void OnPoolCreation()
    {
        id = Guid.NewGuid();
        frameSaveList.Add(SaveFrame());
        creationFrameNumber = GameObjectStateManager.Instance.FrameNumber;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Bot Bullet touched enemy ");
            EnemyController ectrl = other.gameObject.GetComponent<EnemyController>();
            ectrl.Destroy();
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bot Bullet touched Player ");
            PlayerBotController pBCtrl = other.gameObject.GetComponent<PlayerBotController>();

            if (pBCtrl)
            {
                pBCtrl.Destroy();
            }
            else
            {
                PlayerController pc = other.gameObject.GetComponent<PlayerController>();
                if (pc)
                {
                    pc.Destroy();
                }
                else
                {
                    Debug.Log("BulletController : Invalid Player tag type was touched");
                }
            }
        }

        Destroy();
    }
    
    public void OnRelease()
    {
        if (id != Guid.Empty)
        {
            GameObjectStateManager.Instance.addDynamicObject(id, GetType(), frameSaveList, creationFrameNumber);
        }

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        frameSaveList = new List<string>();
    }

    public override string SaveFrame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        
        BulletBaseFrameSave data = new BulletBaseFrameSave();
        
        data.id = new Byte[id.ToByteArray().Length];
        id.ToByteArray().CopyTo(data.id,0);
        
        data.speed = speed;
        data.position = VectorArrayConverter.vector3ToArray(transform.position);
        data.rotation = VectorArrayConverter.vector3ToArray(transform.rotation.eulerAngles);
        data.direction = VectorArrayConverter.vector3ToArray(transform.forward);
        
        bf.Serialize(ms,data);

        return Convert.ToBase64String(ms.ToArray());
    }
    
    public override string SaveDiffFrame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        
        BulletDiffFrameData data = new BulletDiffFrameData();
        
        data.position = VectorArrayConverter.vector3ToArray(transform.position);
        data.rotation = VectorArrayConverter.vector3ToArray(transform.rotation.eulerAngles);
        
        bf.Serialize(ms,data);

        return Convert.ToBase64String(ms.ToArray());
    }
    
    public void Destroy()
    {
        Pool.Instance.release(gameObject, PoolableTypes.Bullets);
    }
}