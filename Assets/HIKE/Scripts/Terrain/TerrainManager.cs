using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class TerrainManager : MonoBehaviour
{
    /* DEFINITIONS:
     * 
     * 2D Array mapped onto terrain => array[z][x] <<-- first index is z coordinate, second index is x coordinate!
     * von Data auf Terrain x und z tauschen!
     */

    private TerrainIndex index;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    [InspectorButton("Clear")]
    public bool clear;

    void Start()
    {
        Regenerate();
    }

    /// <summary>
    /// Reloads the terrain
    /// </summary>
    private void Regenerate()
    {
        Clear();
        var settings = HikeSettings.GetOrCreateSettings();
        CoordinateService coordinateService = CoordinateService.GetInstance();
        this.index = JsonUtility.FromJson<TerrainIndex>(settings.dgmIndexFile.text);

        float[,] completeHeightmap = ImportHeightmapFromDataFile(settings.dgmDataFile, this.index.heightmap.x, this.index.heightmap.z);
        float terrainSize = ((settings.mapHeightmapResolution - 1) * this.index.gitterweite) * settings.mapScaleFactor;
        Debug.Log(this.index.y.max);
        float terrainHeight = coordinateService.convertHeightETRSToUnity(this.index.y.max);// (this.index.y.diff * this.heightScaleFactor) / this.downscaleFactor;

        Vector3 terrainDimensions = new Vector3(terrainSize, terrainHeight, terrainSize);
        int xSegmentAmount = CalcSegmentsNeededForLength(this.index.x.diff, this.index.gitterweite, settings.mapHeightmapResolution);
        int zSegmentAmount = CalcSegmentsNeededForLength(this.index.z.diff, this.index.gitterweite, settings.mapHeightmapResolution);

        Terrain[,] terrains = new Terrain[zSegmentAmount, xSegmentAmount];
        Debug.Log($"TerrainSize: {xSegmentAmount}*{zSegmentAmount}");

        CreateTerrainGameObjects(terrains, completeHeightmap, terrainDimensions, xSegmentAmount, zSegmentAmount, settings);
        SetNeighboringTerrain(terrains, xSegmentAmount, zSegmentAmount);
        
    }

    /// <summary>
    /// Creates all Terrain Game Objects and applies the heightmap and terrain to it
    /// 
    /// --> Modifies elements in terrains
    /// </summary>
    /// <param name="terrains">2D Array containing, in which the terrain game objects shall be stored</param>
    /// <param name="completeHeightmap">Heightmap of the whole terrain</param>
    /// <param name="terrainDimensions">length, width and maxHeight of the terrains</param>
    /// <param name="xSegmentAmount">Amount of Segments on the X Axis</param>
    /// <param name="zSegmentAmount">Amount of Segments on the Z Axis</param>
    private void CreateTerrainGameObjects(Terrain[,] terrains, float[,] completeHeightmap, Vector3 terrainDimensions, int xSegmentAmount, int zSegmentAmount, HikeSettings settings)
    {
        int resolution = settings.mapHeightmapResolution;
        string materialAssetPath = settings.dopMaterialPath;

        for (int z = 0; z < zSegmentAmount; z++)
        {
            for (int x = 0; x < xSegmentAmount; x++)
            {
                var tileHeightmap = ExtractTileHeightmap(completeHeightmap, x * (resolution - 1), z * (resolution - 1), resolution);

                TerrainData data = new TerrainData();
                data.heightmapResolution = resolution;
                data.size = terrainDimensions;
                data.SetHeights(0, 0, tileHeightmap);

                GameObject obj = Terrain.CreateTerrainGameObject(data);
                obj.name = "harz_seg_" + x + "_" + z;
                obj.transform.parent = transform;   //Set this TerrainManager as parent

                Terrain terrain = obj.GetComponent<Terrain>();
                terrain.transform.position = new Vector3(x * terrainDimensions.x, 0, z * terrainDimensions.z);
                Material material = GetMaterial(x, z, materialAssetPath);
                terrain.materialTemplate = material;

                if(material == null)
                    obj.SetActive(false);

                terrains[z, x] = terrain;
            }
        }
    }

    /// <summary>
    /// Assigns the Neighbors to all terrains
    /// 
    /// --> Modifies elements in terrains
    /// </summary>
    /// <param name="terrains">2D array containing the terrains</param>
    /// <param name="xSegmentAmount">Amount of Segments in the X Axis</param>
    /// <param name="zSegmentAmount">Amount of Segments in the Z Axis</param>
    private void SetNeighboringTerrain(Terrain[,] terrains, int xSegmentAmount, int zSegmentAmount)
    {
        for (int x = 0; x < xSegmentAmount && x < terrains.GetLength(0) - 1; x++)
        {
            for (int z = 0; z < zSegmentAmount && z < terrains.GetLength(1); z++)
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

    /// <summary>
    /// Calculates the amount of terrain segments needed to fill the length of a axis
    /// </summary>
    /// <param name="length">Axis Length (in Meter)</param>
    /// <param name="gridWidth">Distance between terrain heightmap sample points (gitterweite)</param>
    /// <param name="tileHeightmapResolution">Tile Heightmap Resolution (Height & Width)</param>
    /// <returns>Number of Terrain Segments needed for this length (rounded up)</returns>
    private int CalcSegmentsNeededForLength(float length, int gridWidth, int tileHeightmapResolution)
    {
        float unscaledLength = length / gridWidth;
        float scaledLength = unscaledLength / tileHeightmapResolution;
        int ceiledLength = Mathf.CeilToInt(scaledLength);
        return ceiledLength;
    }

    /// <summary>
    /// Used to convert the height-bytes from the dataFile to a 2D-Array heightmap
    /// </summary>
    /// <param name="dataFile">Text Asset containing the binary height values of the digital terrain model</param>
    /// <param name="xHeightmapSize">Length of the Heightmap in the X Axis</param>
    /// <param name="zHeightmapSize">Length of the Heightmap in the Z Axis</param>
    /// <returns>
    /// Heightmap of the complete terrain
    /// </returns>
    private float[,] ImportHeightmapFromDataFile(TextAsset dataFile, int xHeightmapSize, int zHeightmapSize)
    {
        float[,] data = new float[xHeightmapSize, zHeightmapSize];
        byte[] bytes = dataFile.bytes;
        int c = 0;
        for (int z = 0; z < zHeightmapSize; z++)
        {
            for (int x = 0; x < xHeightmapSize; x++)
            {
                //Finish early if there are no more bytes available
                if (c >= bytes.Length)
                    return data;

                float y = BitConverter.ToSingle(bytes, c);
                c += 4;
                data[x, z] = y;
            }
        }

        return data;
    }

    /// <summary>
    /// Used to extract the heightmap of a single terrain segment from the whole terrain heightmap
    /// </summary>
    /// <param name="completeHeightmap">Heightmap of the whole terrain</param>
    /// <param name="startX">X Start Index of the Tile</param>
    /// <param name="startZ">Z Start Index of the Tile</param>
    /// <param name="resolution">Resolution (Height/Width) of the tile heightmap</param>
    /// <returns>Heightmap of the Tile at index X, Z</returns>
    private float[,] ExtractTileHeightmap(float[,] completeHeightmap, int startX, int startZ, int resolution)
    {
        float[,] tileHeightmap = new float[resolution, resolution];

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int transformedX = startX + x;
                int transformedZ = startZ + z;
                if (transformedX >= completeHeightmap.GetLength(0) || transformedZ >= completeHeightmap.GetLength(1))
                    continue;

                try
                {
                    tileHeightmap[z, x] = completeHeightmap[startX + x, startZ + z];
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.Log($"Out of bounds for indicies: x={x}, z={z}, transformedX={transformedX}, transformedZ={transformedZ}");
                }
            }
        }

        return tileHeightmap;
    }

    public float calculateHeightAt(float x, float z, int resolution)
    {
        HikeSettings settings = HikeSettings.GetOrCreateSettings();

        int segX = Mathf.FloorToInt(x / (((resolution - 1) * this.index.gitterweite) * settings.mapScaleFactor));
        int segZ = Mathf.FloorToInt(z / (((resolution - 1) * this.index.gitterweite) * settings.mapScaleFactor));
        String name = "harz_seg_" + segX + "_" + segZ;
        Terrain terrain = this.GetComponentsInChildren<Terrain>().ToList().Find(elem => elem.name == name);

        if(terrain == null) return 0;

        float height = terrain.SampleHeight(new Vector3(x, 0, z));
        return height;
    }

    /// <summary>
    /// Returns the Material for a specified terrain index
    /// </summary>
    /// <param name="terrainIndexX">X Terrain Index</param>
    /// <param name="terrainIndexZ">Z Terrain Index</param>
    /// <returns>Material for the terrain at index x, z; possibly null (if no material for this index exists)</returns>
    public Material GetMaterial(int terrainIndexX, int terrainIndexZ, string materialAssetPath)
    {
        String path = materialAssetPath + "/tile_" + terrainIndexX + "_" + terrainIndexZ;
        Material loadedMaterial = Resources.Load<Material>(path);

        if (loadedMaterial == null)
            Debug.Log($"Material with Path '{path}' not found. Disabling Terrain");

        return loadedMaterial;
    }

    /// <summary>
    /// Removes all child components (old terrain segments)
    /// </summary>
    private void Clear()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    public TerrainIndex Index { get { return this.index; } }

}
