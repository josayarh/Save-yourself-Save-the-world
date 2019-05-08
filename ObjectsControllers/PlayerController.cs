using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class PlayerController : SavableObject
{
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeedH;
    [SerializeField] private float rotateSpeedV;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private float turnSpeed = 30.0f;
    [SerializeField] private float accelerationFactor = 2;
    [SerializeField] private Transform gunTipPosition;
    
    private Camera mainCamera;
    private Rigidbody rigidBody;
    private GameObject laser;
    
    private float timer = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        id = Guid.NewGuid();
        
        mainCamera = Camera.main;
        rigidBody = GetComponent<Rigidbody>();
        laser = Resources.Load("Prefabs/shot_prefab") as GameObject;
        
        frameSaveList.Add(SaveFrame());
    }


    private void FixedUpdate()
    {
        float fireKey = Input.GetAxis("Fire1");
        timer += Time.deltaTime;
        
        rigidBody.velocity=Vector3.zero;
        move();
        rotation();

        if (timer > fireRate && fireKey != 0)
        {
            fire();
        }
        
        frameSaveList.Add(SaveDiffFrame());
    }

    private void move()
    {
        float vertical = Input.GetAxis("Vertical");
        float acceleration = Input.GetAxis("Fire3");
        
        Vector3 movement = new Vector3(/*horizontal*/ 0 , 0, vertical);
        transform.Translate(movement * speed * Time.fixedDeltaTime + 
                            movement * acceleration * accelerationFactor);
    }

    private void rotation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float roll = turnSpeed * Time.deltaTime * horizontal;
        float yaw = rotateSpeedH * Input.GetAxis("Mouse X");
        float pitch = rotateSpeedV * Input.GetAxis("Mouse Y");
        Quaternion AddRot = Quaternion.identity;
        
        AddRot.eulerAngles = new Vector3(-pitch, yaw, -roll);
        rigidBody.rotation *= AddRot;
    }

    private void fire()
    {
        Instantiate(laser, gunTipPosition.position, gunTipPosition.rotation);
        timer = 0.0f;
    }

    public override string SaveFrame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        
        PlayerBaseFrameData frameData = new PlayerBaseFrameData();

        frameData.id = new Byte[id.ToByteArray().Length];
        id.ToByteArray().CopyTo(frameData.id,0);

        frameData.position = VectorArrayConverter.vector3ToArray(transform.position);
        frameData.rotation = VectorArrayConverter.vector3ToArray(transform.rotation.eulerAngles);
        frameData.speed = speed;
        frameData.rotateSpeedH = rotateSpeedH;
        frameData.rotateSpeedV = rotateSpeedV;
        frameData.fireRate = fireRate;
        frameData.turnSpeed = turnSpeed;
        frameData.accelerationFactor = accelerationFactor;
        frameData.gunTipPosition = VectorArrayConverter.vector3ToArray(gunTipPosition.position);
        frameData.gunTipRotation = VectorArrayConverter.vector3ToArray(gunTipPosition.rotation.eulerAngles);
        
        bf.Serialize(ms,frameData);

        return Convert.ToBase64String(ms.ToArray());
    }
    
    public override string SaveDiffFrame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        
        PlayerDiffFrameData frameData = new PlayerDiffFrameData();

        frameData.position = VectorArrayConverter.vector3ToArray(transform.position);
        frameData.rotation = VectorArrayConverter.vector3ToArray(transform.rotation.eulerAngles);
        
        bf.Serialize(ms,frameData);

        return Convert.ToBase64String(ms.ToArray());
    }
    
    private void OnDestroy()
    {
        GameObjectStateManager.Instance.addDynamicObject(id, GetType(),frameSaveList,0);
    }
}
