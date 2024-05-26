using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    /* DEFINITIONS:
     * 
     * 2D Array mapped onto terrain => array[z][x] <<-- first index is z coordinate, second index is x coordinate!
     * von Data auf Terrain x und z tauschen!
     */

    private TerrainIndex index;
    public TextAsset dataFile;
    public TextAsset indexFile;
    public int resolution = 513;
    public float heightScaleFactor = 1.0f;
    public float downscaleFactor = 1.0f;
    public String materialAssetPath = "Textures/Terrain/Materials";

    [InspectorButton("Regenerate")]
    public bool regenerate;

    void Start()
    {
        Regenerate();
    }

    private void Regenerate()
    {
        RemoveOldObjects();

        if (this.indexFile == null)
            throw new Exception("IndexFile must not be null");

        if (this.dataFile == null)
            throw new Exception("DataFile must not be null");

        //Import DGM Index Object from JSON
        this.index = JsonUtility.FromJson<TerrainIndex>(indexFile.text);

        if (this.index == null)
            throw new Exception("Index must not be null");

        float[,] dgmHeightmap = ImportDGM();
        float terrainSize = ((this.resolution - 1) * this.index.gitterweite) / this.downscaleFactor;
        float terrainHeight = (this.index.y.diff * this.heightScaleFactor) / this.downscaleFactor;
        Vector3 terrainDimensions = new Vector3(terrainSize, terrainHeight, terrainSize);
        int terrainAmountX = CalcTerrainAmount(this.index.x.diff);
        int terrainAmountZ = CalcTerrainAmount(this.index.z.diff);
        Terrain[,] terrains = new Terrain[terrainAmountZ, terrainAmountX];

        Debug.Log($"TerrainSize: {terrainAmountX}*{terrainAmountZ}");

        //Create Terrain for each slot in terrain array and assign a portion of the heightmap to it
        for (int z = 0; z < terrainAmountZ; z++)
        {
            for (int x = 0; x < terrainAmountX; x++)
            {
                var heightmap = GetHeightmap(dgmHeightmap, 0 + (x * (this.resolution - 1)), 0 + (z * (this.resolution - 1)), this.resolution);

                TerrainData data = new TerrainData();
                data.heightmapResolution = this.resolution;
                data.size = terrainDimensions;
                data.SetHeights(0, 0, heightmap);

                GameObject obj = Terrain.CreateTerrainGameObject(data);
                obj.name = "harz_seg_" + x + "_" + z;
                obj.transform.parent = transform;   //Set this TerrainManager as parent

                Terrain terrain = obj.GetComponent<Terrain>();
                terrain.transform.position = new Vector3(x * terrainDimensions.x, 0, z * terrainDimensions.z);
                terrain.materialTemplate = GetMaterial(x, z);

                terrains[z, x] = terrain;
            }
        }

        for (int x = 0; x < terrainAmountX && x < terrains.GetLength(0) - 1; x++)
        {
            for (int z = 0; z < terrainAmountZ && z < terrains.GetLength(1); z++)
            {
                Terrain terrain = terrains[x, z];
                Terrain north = null;
                Terrain south = null;
                Terrain east = null;
                Terrain west = null;

                if (x > 0)
                    north = terrains[x - 1, z];

                if (x < terrains.GetLength(0) - 1)
                    south = terrains[x + 1, z];

                if (z > 0)
                    west = terrains[x, z - 1];

                if (z < terrains.GetLength(1) - 1)
                    east = terrains[x, z + 1];

                terrain.SetNeighbors(west, north, east, south);
            }
        }
    }

    private int CalcTerrainAmount(float diff)
    {
        float unscaledLength = diff / this.index.gitterweite;
        float scaledLength = unscaledLength / this.resolution;
        int ceiledLength = Mathf.CeilToInt(scaledLength);
        return ceiledLength;
    }

    private float[,] ImportDGM()
    {
        float[,] data = new float[this.index.heightmap.x, this.index.heightmap.z];
        byte[] bytes = dataFile.bytes;
        int c = 0;
        for (int z = 0; z < index.heightmap.z; z++)
        {
            for (int x = 0; x < index.heightmap.x; x++)
            {
                if (c >= bytes.Length)
                    continue;
                float y = BitConverter.ToSingle(bytes, c);
                c += 4;
                data[x, z] = y;
            }
        }

        return data;
    }

    private float[,] GetHeightmap(float[,] terrainData, int startX, int startZ, int resolution)
    {
        float[,] heightmap = new float[resolution, resolution];

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int transformedX = startX + x;
                int transformedZ = startZ + z;
                if (transformedX >= terrainData.GetLength(0) || transformedZ >= terrainData.GetLength(1))
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

    public Material GetMaterial(int terrainIndexX, int terrainIndexZ)
    {
        String path = materialAssetPath + "/tile_" + terrainIndexX + "_" + terrainIndexZ;
        Material loadedMaterial = Resources.Load<Material>(path);

        if (loadedMaterial == null)
            Debug.Log($"Material with Path '{path}' not found");

        return loadedMaterial;
    }

    private void RemoveOldObjects()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    public TerrainIndex Index { get { return this.index; } }

}
