using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains data about a stamp (Stempelstelle)
/// </summary>
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

/// <summary>
/// Wrapper for StampData Arrays
/// </summary>
[Serializable]
public class StampDataArray
{
    public StampData[] stempelstellen;
}
