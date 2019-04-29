using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum DynamicTags
{
    Player,
    Dynamic,
}

public class GameObjectStateManager : MonoBehaviour
{
    private static GameObjectStateManager instance;
    private List<List<Tuple<Type, string>>> frameObjectList = new List<List<Tuple<Type, string>>>();
    private List<GameObject> dynamicGameObjectlist;
    private uint frameNumber = 0;
    private List<GameObject> previousFrameObjects = new List<GameObject>();
    
    private Dictionary<Guid, Tuple<Type ,List<string>>> frameDataDictionary = new Dictionary<Guid, Tuple<Type, List<string>>>();
    private Dictionary<uint, List<Guid>> objectApperanceDictionnary = new Dictionary<uint, List<Guid>>();
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            instance.frameNumber = 0;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        /*foreach (var VARIABLE in previousFrameObjects)
        {
            Destroy(VARIABLE);
        }*/
        reloadObjects();
        // saveFrame();
        frameNumber++;
    }

    public void initializeDynamicObjects()
    {
        dynamicGameObjectlist = new List<GameObject>();
        GameObject[] tab = GameObject.FindObjectsOfType<GameObject>();

        if (tab != null)
        {
            DynamicTags tmp;
            foreach (var gameObject in tab)
            {
                bool isDynamic = false;
                foreach (var name in Enum.GetNames(typeof(DynamicTags)))
                {
                    if (gameObject.CompareTag(name))
                        isDynamic = true;
                }
                if(isDynamic)
                    dynamicGameObjectlist.Add(gameObject);
            }
        }
    }

    /*private void saveFrame()
    {
        List<Tuple<Type, string>> frameList;
        if(frameObjectList.Count < frameNumber + 1)
            frameObjectList.Add(new List<Tuple<Type, string>>());
        
        frameList = frameObjectList[frameNumber];
        
        foreach (GameObject go in dynamicGameObjectlist)
        {
            switch (go.tag)
            {
                case "Player":
                    PlayerController playerController = go.GetComponent<PlayerController>();
                    string binarySave = playerController.SaveFrame();
                    Type type = playerController.GetType();
                    frameList.Add(new Tuple<Type, string>(type,String.Copy(binarySave)));
                    break;
            }
        }
    }*/

    private void reloadObjects()
    {
        List<Guid> objectList;
        if (objectApperanceDictionnary.TryGetValue(frameNumber, out objectList))
        {
            foreach(Guid id in objectList)
            {
                Tuple<Type, List<string>> gameObjectTuple;
                if (frameDataDictionary.TryGetValue(id, out gameObjectTuple))
                {
                    if (gameObjectTuple.Item1 == typeof(PlayerController))
                    {
                        GameObject go = Instantiate(Resources.Load("Prefabs/Player Bot") as GameObject);
                        PlayerBotController playerBotController = go.GetComponent<PlayerBotController>();
                        playerBotController.FrameSteps = gameObjectTuple.Item2;
                    }
                }
            }
        }
    }

    public static GameObjectStateManager Instance
    {
        get => instance;
    }

    public void addDynamicObject(Guid guid, Type type, List<String> frameSave)
    {
        addDynamicObject(guid, type, frameSave, frameNumber);
    }
    
    public void addDynamicObject(Guid guid, Type type, List<String> frameSave, uint saveFrameNumber)
    {
        Tuple<Type ,List<string>> couple = new Tuple<Type, List<string>>(type, frameSave);
        List<Guid> list;

        if (!objectApperanceDictionnary.TryGetValue(saveFrameNumber, out list))
        {
            list = new List<Guid>();
            objectApperanceDictionnary.Add(saveFrameNumber, list);
        }
        list.Add(guid);
        frameDataDictionary.Add(guid, couple);
    }

    public uint FrameNumber
    {
        get => frameNumber;
    }
}

public class DynamicObjectCreatedEventArgs : EventArgs
{
    public GameObject go;
}