using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WandernadelManager : MonoBehaviour
{
    public TextAsset stampDataFile;
    public GameObject stampPrefab;
    public TerrainManager terrainManager;
    public float scaleFactor = 1.0f;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    private Transform[] wandernadeln;

    public void Regenerate()
    {
        RemoveOldObjects();

        StampData[] stempelstellen = JsonUtility.FromJson<StampDataArray>(stampDataFile.text).stempelstellen;
        wandernadeln = new Transform[stempelstellen.Length];

        TerrainIndex index = terrainManager.Index;

        for (int i = 0; i < stempelstellen.Length; i++)
        {
            var data = stempelstellen[i];
            var stempelstelle = Instantiate(stampPrefab, this.transform);
            stempelstelle.name = i + " - " + data.name;
            stempelstelle.SetActive(true);
            float xPos = (data.x - index.x.min) / terrainManager.downscaleFactor;
            float yPos = ((data.y - index.y.min) * terrainManager.heightScaleFactor) / terrainManager.downscaleFactor;
            float zPos = (data.z - index.z.min) / terrainManager.downscaleFactor;

            stempelstelle.transform.position = new Vector3(xPos, yPos, zPos);

            stempelstelle.transform.localScale = stempelstelle.transform.localScale * scaleFactor;

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
