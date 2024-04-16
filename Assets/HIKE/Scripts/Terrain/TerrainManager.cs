using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    /* DEFINITIONS:
     * 
     * 2D Array mapped onto terrain => array[z][x] <<-- first index is z coordinate, second index is x coordinate!
     * von Data auf Terrain x und z tauschen!
     * 
     * Data array:      
     * +--> X           
     * |
     * v
     * Z
     * 
     */

    public TextAsset dataFile;
    public int resolution;
    public int minX;
    public int maxX;
    public float minY;
    public float maxY;
    public int minZ;
    public int maxZ;
    public int sampleSpacing;

    void Start()
    {
        float[,] dataMap = ExtractData();
        Vector3 terrainSize = new Vector3(this.resolution * this.sampleSpacing, this.maxY - this.minY, this.resolution * this.sampleSpacing);
        int terrainMapSizeX = ScaleArrayLength(this.minX, this.maxX);
        int terrainMapSizeZ = ScaleArrayLength(this.minZ, this.maxZ);
        Debug.Log($"TerrainSize: {terrainMapSizeX}*{terrainMapSizeZ}");
        Terrain[,] terrainMap = new Terrain[terrainMapSizeX, terrainMapSizeZ];

        for(int z = 0; z < terrainMapSizeZ; z++)
        {
            for(int x = 0; x < terrainMapSizeX; x++)
            {
                TerrainData data = new TerrainData();
                data.heightmapResolution = this.resolution;
                data.size = terrainSize;

                var heightmap = GetHeightmap(dataMap, 0 + (x*(this.resolution-1)), 0 + (z*(this.resolution-1)), this.resolution);
                data.SetHeights(0, 0, heightmap);
                GameObject obj = Terrain.CreateTerrainGameObject(data);
                obj.transform.parent = transform;
                Terrain terrain = obj.GetComponent<Terrain>();
                terrain.transform.position = new Vector3(x * terrainSize.x, 0, z * terrainSize.z);
                terrainMap[z, x] = terrain;
            }
        }

        for(int z = 0; z < terrainMapSizeZ; z++)
        {
            for (int x = 0; x < terrainMapSizeX; x++)
            {
                Terrain north = null;
                Terrain south = null;
                Terrain east = null;
                Terrain west = null;
                
                if (z - 1 > 0)
                    north = terrainMap[z-1, x];

                if (z + 1 < terrainMapSizeZ)
                    south = terrainMap[z + 1, x];

                if (x - 1 > 0)
                    west = terrainMap[z, x-1];

                if (x + 1 < terrainMapSizeX)
                    east = terrainMap[z, x+1];

                terrainMap[z, x].SetNeighbors(west, north, east, south);
            }
        }

        
    }

    private int ScaleArrayLength(int min, int max)
    {
        float diff = max - min;
        float unscaledLength = diff / this.sampleSpacing;

        float scaledLength = unscaledLength / this.resolution;
        int ceiledLength = Mathf.CeilToInt(scaledLength);
        return ceiledLength;
    }

    private float[,] ExtractData()
    {
        var diffX = maxX - minX;
        var diffY = maxY - minY;
        var diffZ = maxZ - minZ;

        float[,] data = new float[diffX/sampleSpacing +1, diffZ/sampleSpacing +1];
        if(dataFile == null)
        {
            Debug.Log("No Data provided");
            return data;
        }
        string[] lines = dataFile.text.Split("\n");
        foreach(string line in lines)
        {
            try
            {
                string[] coordsRaw = line.Split(" ");
                int x = Mathf.RoundToInt((float.Parse(coordsRaw[0], CultureInfo.InvariantCulture) - minX) / sampleSpacing);
                int z = Mathf.RoundToInt((float.Parse(coordsRaw[1], CultureInfo.InvariantCulture) - minZ) / sampleSpacing);
                float y = (float.Parse(coordsRaw[2], CultureInfo.InvariantCulture) - minY) / diffY;
                data[x, z] = y;
            }
            catch(FormatException) {
                Console.Error.WriteLine($"Unable to parse coordinate in line: '{line}'");
            }
        }

        return data;
    }

    private float[,] GetHeightmap(float[,] terrainData, int startX, int startZ, int resolution)
    {
        float[,] heightmap = new float[resolution, resolution];

        for(int z = 0; z < resolution; z++)
        {
            for(int x = 0; x < resolution; x++)
            {
                int transformedX = startX + x;
                int transformedZ = startZ + z;
                if(transformedX >= terrainData.GetLength(0) || transformedZ >= terrainData.GetLength(1))
                {
                    continue;
                }

                try
                {
                    heightmap[z, x] = terrainData[startX + x, startZ + z];
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.Log($"Out of bounds for indicies: x={x}, z={z}, transformedX={transformedX}, transformedZ={transformedZ}");
                }
            }
        }

        return heightmap;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
