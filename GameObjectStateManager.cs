using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum DynamicTags
{
    Player,
    Enemy,
    Dynamic
}

public class GameObjectStateManager : MonoBehaviour
{
    private static GameObjectStateManager instance;
    private List<List<Tuple<Type, string>>> frameObjectList = new List<List<Tuple<Type, string>>>();
    private List<GameObject> dynamicGameObjectlist;
    private int frameNumber = 0;
    private List<GameObject> previousFrameObjects = new List<GameObject>();
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            instance.frameNumber = 0;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        foreach (var VARIABLE in previousFrameObjects)
        {
            Destroy(VARIABLE);
        }
        reloadObjects();
        saveFrame();
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

    private void saveFrame()
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
    }

    private void reloadObjects()
    {
        if (frameObjectList.Count >= frameNumber + 1)
        {
            foreach (Tuple<Type, string> tuple in frameObjectList[frameNumber])
            {
                if (tuple.Item1.IsEquivalentTo(typeof(PlayerController)))
                {
                    GameObject go = Instantiate(Resources.Load("Prefabs/Player Bot") as GameObject);
                    PlayerBotController playerBotController = go.GetComponent<PlayerBotController>();
                    playerBotController.LoadFrame(tuple.Item2);
                    previousFrameObjects.Add(go);
                }
            }
        }
    }

    public static GameObjectStateManager Instance
    {
        get => instance;
    }
}
