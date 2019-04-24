using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;
using Quaternion = UnityEngine.Quaternion;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float timeLeft;
    [SerializeField] private GameObjectStateManager gameObjectStateManager;

    private float currentTimeLeft;
    private TextMeshProUGUI timerText = null;
    private int restartNumber=0;
    private static GameManager gMInstance = null;
    private GameObject playerPrefab;
    private GameObject player;
    
    void Awake()
    {
        DontDestroyOnLoad(this);
        
        playerPrefab = Resources.Load("Prefabs/Player") as GameObject;

        if (gMInstance == null)
        {
            gMInstance = this;
            reloadObject();
        }
        else
        {
            //From here we are sure that the scene has correctly reloaded 
            gMInstance.reloadObject();
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeLeft -= Time.deltaTime;
        timerText.text = currentTimeLeft.ToString("#.##");

        if (currentTimeLeft <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void reloadObject()
    {
        GameObject textMeshPro = GameObject.Find("Time Counter");
        if (textMeshPro)
            timerText = textMeshPro.GetComponent<TextMeshProUGUI>();
        
        if(timerText == null)
            Debug.LogError("Time Counter TextMeshPro or it's UGUI component could not be found");
        
        currentTimeLeft = timeLeft;

        spawnPlayer();

        restartNumber++;
    }

    private void spawnPlayer()
    {
        UnityEngine.Vector3 pos = UnityEngine.Random.insideUnitSphere * 20 + transform.position;
        if(player != null) 
            Destroy(player);
        player = Instantiate(playerPrefab, pos, Quaternion.identity);
        GameObjectStateManager.Instance.initializeDynamicObjects();
    }
}
