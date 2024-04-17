using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WandernadelData
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
public class WandernadelDataArray
{
    public WandernadelData[] stempelstellen;
}
