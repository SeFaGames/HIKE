using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class WandernadelManager : MonoBehaviour
{
    public TextAsset stampDataFile;
    public GameObject stampPrefab;
    public TerrainManager terrainManager;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    [InspectorButton("Clear")]
    public bool clear;

    private Transform[] wandernadeln;

    public void Regenerate()
    {
        Clear();

        CoordinateService coordinateService = CoordinateService.GetInstance();
        var settings = HikeSettings.GetOrCreateSettings();

        float scaleFactor = settings.stampScaleFactor;
        float mapScaleFactor = settings.mapScaleFactor;

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
            float height = terrainManager.calculateHeightAt(Mathf.RoundToInt(coordinates.x), Mathf.RoundToInt(coordinates.z), settings.mapHeightmapResolution);
            coordinates.y = height;

            if(height <= 0)
                stempelstelle.SetActive(false);

            stempelstelle.transform.position = coordinates;
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
