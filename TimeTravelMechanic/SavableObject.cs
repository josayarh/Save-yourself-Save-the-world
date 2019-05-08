using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SavableObject : MonoBehaviour
{
    protected Guid id;
    protected List<String> frameSaveList = new List<string>();

    public abstract string SaveFrame();
    public abstract string SaveDiffFrame();

    public Guid Id
    {
        get => id;
    }
}
