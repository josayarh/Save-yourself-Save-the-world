using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulletManager : SavableObject
{
    private uint creationFrameNumber;
    [SerializeField] private float speed;
    private Vector3 direction;

    private void Start()
    {
        frameSaveList = new List<string>();
        
        id = Guid.NewGuid();
        direction = Camera.main.transform.forward;
        frameSaveList.Add(SaveFrame());
        creationFrameNumber = GameObjectStateManager.Instance.FrameNumber;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
        frameSaveList.Add(SaveDiffFrame());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy has been hit");
            //To change to damage system 
            EnemyController enemyController = other.gameObject.GetComponent <EnemyController>();
            enemyController.HasBeenKilled = true;
            Destroy(other.gameObject); 
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Destroy(other.gameObject); 
        }
        
        Destroy(this.gameObject);
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
    
    private void OnDestroy()
    {
        GameObjectStateManager.Instance.addDynamicObject(id, GetType(),frameSaveList,creationFrameNumber);
    }
}