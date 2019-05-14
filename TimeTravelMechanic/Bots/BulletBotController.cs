﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BulletBotController : Bot
{
    private float speed;
    
    public override void LoadFrame(string binarySave)
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
    
    public override void LoadDiffFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();
        BulletDiffFrameData data = (BulletDiffFrameData)bf.Deserialize(mf);

        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Bot Bullet touched enemy ");
            //To change to damage system 
            Destroy(other.gameObject); 
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bot Bullet touched Player ");
            Destroy(other.gameObject); 
        }
        
        Destroy(this.gameObject);
    }
}
