using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class WandernadelManager : MonoBehaviour
{
    public TextAsset stampDataFile;
    public GameObject stampPrefab;
    //public TerrainManager terrainManager;
    public CoordinateService coordinateService;
    public float scaleFactor = 1.0f;
    //public float downscaleFactor = 0.01f;
    //public float heightScaleFactor = 1.0f;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    private Transform[] wandernadeln;

    public void Regenerate()
    {
        RemoveOldObjects();

        StampData[] stempelstellen = JsonUtility.FromJson<StampDataArray>(stampDataFile.text).stempelstellen;
        wandernadeln = new Transform[stempelstellen.Length];

        //TerrainIndex index = terrainManager.Index;

        for (int i = 0; i < stempelstellen.Length; i++)
        {
            var data = stempelstellen[i];
            var stempelstelle = Instantiate(stampPrefab, this.transform);
            stempelstelle.name = i + " - " + data.name;
            stempelstelle.SetActive(true);

            stempelstelle.transform.position = coordinateService.convertETRSToUnity(new Vector3(data.x, data.y, data.z));
            stempelstelle.transform.localScale = (stempelstelle.transform.localScale * scaleFactor) * coordinateService.scaleFactor;

            //Apply a random rotation around y axis
            Quaternion randomRotationRaw = Random.rotation;
            float yRotation = randomRotationRaw.eulerAngles.y;
            stempelstelle.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            wandernadeln[i] = stempelstelle.transform;
        }
    }

    private void RemoveOldObjects()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }
}
