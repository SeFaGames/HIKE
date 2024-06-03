using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class WandernadelManager : MonoBehaviour
{
    public TerrainManager terrainManager;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    [InspectorButton("Clear")]
    public bool clear;

    public bool regenerateOnGameStart = false;

    private Transform[] wandernadeln;

    void Start()
    {
        if(regenerateOnGameStart)   
            Regenerate();
    }

    public void Regenerate()
    {
        Clear();

        CoordinateService coordinateService = CoordinateService.GetInstance();
        HikeSettings settings = HikeSettings.GetOrCreateSettings();
        TerrainIndex index = settings.GetTerrainIndex();

        float scaleFactor = settings.stampScaleFactor;
        float mapScaleFactor = settings.mapScaleFactor;
        TextAsset stampDataFile = settings.stampDataFile;
        GameObject stampPrefab = settings.stampPrefab;


        StampData[] stempelstellen = JsonUtility.FromJson<StampDataArray>(stampDataFile.text).stempelstellen;
        wandernadeln = new Transform[stempelstellen.Length];

        //TerrainIndex index = terrainManager.Index;

        for (int i = 0; i < stempelstellen.Length; i++)
        {
            var data = stempelstellen[i];
            var stempelstelle = Instantiate(stampPrefab, this.transform);
            stempelstelle.name = i + " - " + data.name;
            stempelstelle.SetActive(true);

            Vector3 coordinates = coordinateService.convertETRSToUnity(new Vector3(data.x, data.y, data.z));
            float height = terrainManager.calculateHeightAt(Mathf.RoundToInt(coordinates.x), Mathf.RoundToInt(coordinates.z), settings.mapHeightmapResolution, index);
            coordinates.y = height;

            if(height <= 0)
                stempelstelle.SetActive(false);

            stempelstelle.transform.localPosition = coordinates;
            stempelstelle.transform.localScale = (stempelstelle.transform.localScale * scaleFactor) * mapScaleFactor;

            //Apply a random rotation around y axis
            Quaternion randomRotationRaw = Random.rotation;
            float yRotation = randomRotationRaw.eulerAngles.y;
            stempelstelle.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            wandernadeln[i] = stempelstelle.transform;
        }
    }

    private void Clear()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }
}
