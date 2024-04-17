using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

[Serializable]
public class CoordinateBound
{
    public float min;
    public float max;
    public float diff;
}

[Serializable]
public class HeightmapBounds
{
    public int x;
    public int z;
}