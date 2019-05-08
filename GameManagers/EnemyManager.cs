using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Transform> enemyPositions = new List<Transform>();
    private static EnemyManager instance = null;
    //private List<Tuple<Guid, Transform>> enemySpawnlist;
    private GameObject enemyPrefab;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            enemyPrefab = Resources.Load("Prefabs/Enemy") as GameObject;
            instance = this;
            foreach (var position in enemyPositions)
            {
                Instantiate(enemyPrefab, position.position, Quaternion.identity);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public EnemyManager Instance => instance;
}
