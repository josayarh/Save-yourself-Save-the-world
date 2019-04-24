using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 direction;

    private void Start()
    {
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
}
