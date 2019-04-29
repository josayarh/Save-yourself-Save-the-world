using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class PlayerBotController : MonoBehaviour
{
    private Guid id;
    
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

    private List<String> frameSteps;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        laser = Resources.Load("Prefabs/shot_prefab") as GameObject;
    }

    private void FixedUpdate()
    {
        uint frameNumber = GameObjectStateManager.Instance.FrameNumber;
        if ( frameNumber < frameSteps.Count)
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
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();
        PlayerBaseFrameData data = (PlayerBaseFrameData)bf.Deserialize(mf);
        
        id = new Guid(data.id);

        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));
    }

    public void LoadDiffFrame(string binarySave)
    {
        byte[] byteArray = Convert.FromBase64String(binarySave);
        MemoryStream mf = new MemoryStream(byteArray);
        BinaryFormatter bf = new BinaryFormatter();
        PlayerDiffFrameData data = (PlayerDiffFrameData)bf.Deserialize(mf);

        transform.position = VectorArrayConverter.arrayToVector3(data.position);
        transform.rotation = Quaternion.Euler(VectorArrayConverter.arrayToVector3(data.rotation));
    }

    public List<string> FrameSteps
    {
        set
        {
            frameSteps = value;
            LoadFrame(value[0]);
        }
    }

    public Guid Id
    {
        get => id;
    }
}
