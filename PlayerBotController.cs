using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class PlayerBotController : MonoBehaviour
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
    
    private float timer = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        laser = Resources.Load("Prefabs/shot_prefab") as GameObject;
    }

    public void LoadFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();
        PlayerTimeTravelData data = (PlayerTimeTravelData)bf.Deserialize(mf);

        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));
    }
}
