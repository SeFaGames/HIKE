using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StampData
{
    public int id;
    public string name;
    public float y;
    public float x;
    public float z;
    public float lat;
    public float lon;
}

[Serializable]
public class StampDataArray
{
    public StampData[] stempelstellen;
}
