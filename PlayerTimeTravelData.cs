using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class PlayerTimeTravelData
{
    public float[] position;
    public float[] rotation;
    public float speed;
    public float rotateSpeedH;
    public float rotateSpeedV;
    public float fireRate;
    public float turnSpeed;
    public float accelerationFactor;
    public float[] gunTipPosition;
    public float[] gunTipRotation;
}