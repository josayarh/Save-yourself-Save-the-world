using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulletManager : MonoBehaviour
{
    private Guid id;
    
    [SerializeField] private float speed;
    private Vector3 direction;

    private void Start()
    {
        id = Guid.NewGuid();
        direction = Camera.main.transform.forward;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy has been hit");
            //To change to damage system 
            Destroy(other.gameObject); 
        }
        
        Destroy(this.gameObject);
    }

    public string SaveFrame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        
        BulletBaseFrameSave data = new BulletBaseFrameSave();
        
        id.ToByteArray().CopyTo(data.id,0);
        data.speed = speed;
        data.position = VectorArrayConverter.vector3ToArray(transform.position);
        data.rotation = VectorArrayConverter.vector3ToArray(transform.rotation.eulerAngles);
        
        bf.Serialize(ms,data);

        return Convert.ToBase64String(ms.ToArray());
    }
    
    public string SaveDiffFrame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        
        BulletDiffFrameData data = new BulletDiffFrameData();
        
        data.position = VectorArrayConverter.vector3ToArray(transform.position);
        data.rotation = VectorArrayConverter.vector3ToArray(transform.rotation.eulerAngles);
        
        bf.Serialize(ms,data);

        return Convert.ToBase64String(ms.ToArray());
    }
    
    public void LoadFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();
        BulletBaseFrameSave data = (BulletBaseFrameSave)bf.Deserialize(mf);
        
        id = new Guid(data.id);

        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));

        speed = data.speed;
    }
    
    public void LoadDiffFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();
        BulletDiffFrameData data = (BulletDiffFrameData)bf.Deserialize(mf);

        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));
    }
}

[Serializable]
struct BulletBaseFrameSave
{
    public byte[] id;
    public float speed;
    public float[] position;
    public float[] rotation;
}

[Serializable]
struct BulletDiffFrameData
{
    public float[] position;
    public float[] rotation;
}