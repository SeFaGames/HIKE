using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to store meta-information abount the digital terrain modell
/// </summary>
[Serializable]
public class TerrainIndex
{
    public CoordinateBound x;
    public CoordinateBound y;
    public CoordinateBound z;
    public HeightmapBounds heightmap;
    public int gitterweite;
    public int dgm_sample_size;
}

/// <summary>
/// Contains the minimum, maximum value of a coordinate in the dgm and provides their diffrence
/// </summary>
[Serializable]
public class CoordinateBound
{
    public float min;
    public float max;
    public float diff;
}

/// <summary>
/// Contains the x and z bounds of the heightmap
/// </summary>
[Serializable]
public class HeightmapBounds
{
    public int x;
    public int z;
}