using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

/// <summary>
/// This class manages the stamps (Stempelstellen).
/// Generates, transforms, places them and generates the information canvases
/// </summary>
public class WandernadelManager : MonoBehaviour
{
    public TerrainManager terrainManager;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    [InspectorButton("Clear")]
    public bool clear;

    public bool regenerateOnGameStart = false;

    public bool filterWhitelist = true;

    public int[] filter;

    public Transform lookAtOrigin;

    private Transform[] wandernadeln;

    private bool visibility = true;

    /// <summary>
    /// This will be executed on game start
    /// </summary>
    void Start()
    {
        if(regenerateOnGameStart)   
            Regenerate();
    }

    /// <summary>
    /// Toggles the visibility of this manager
    /// </summary>
    public void toggleVisibility()
    {
        visibility = !visibility;
        gameObject.SetActive(visibility);
    }

    /// <summary>
    /// Removes all existing stamps and regenerates them.
    /// Generation consists of stamp data retrieval, prefab instantiation and placement
    /// </summary>
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

        for (int i = 0; i < stempelstellen.Length; i++)
        {
            if(filter.Length > 0)
            {
                if (FilterContains(i) != filterWhitelist)
                    continue;
            }

            var data = stempelstellen[i];
            var stempelstelle = Instantiate(stampPrefab, this.transform);
            stempelstelle.name = i + " - " + data.name;
            stempelstelle.SetActive(true);

            Vector3 coordinates = coordinateService.convertETRSToUnity(new Vector3(data.x, data.y, data.z));
            float height = terrainManager.calculateHeightAt(coordinates.x, coordinates.z, settings.mapHeightmapResolution, index);
            coordinates.y = height;

            if(height <= 0)
                stempelstelle.SetActive(false);

            stempelstelle.transform.localPosition = coordinates;
            stempelstelle.transform.localScale = (stempelstelle.transform.localScale * scaleFactor) * mapScaleFactor;

            GameObject infoCanvas = CreateInfoCanvas(stempelstelle.transform, data);

            //Apply a random rotation around y axis
            Quaternion randomRotationRaw = Random.rotation;
            float yRotation = randomRotationRaw.eulerAngles.y;
            stempelstelle.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            wandernadeln[i] = stempelstelle.transform;
        }
    }

    /// <summary>
    /// Generates a canvas object that displays stamp informations
    /// </summary>
    /// <param name="stempelstelle"></param>
    /// <param name="data"></param>
    /// <returns>Canvas GameObject</returns>
    private GameObject CreateInfoCanvas(Transform stempelstelle, StampData data)
    {
        GameObject canvasWrapper = new GameObject("Info Canvas");
        canvasWrapper.transform.parent = stempelstelle;
        canvasWrapper.transform.localScale = new Vector3(60, 80, 10);
        canvasWrapper.transform.localPosition = new Vector3(0, 15000, 0);
        Canvas canvas = canvasWrapper.AddComponent<Canvas>();
        var rect = canvas.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, 300);

        var text = canvas.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 48;
        text.text = "- " + data.id + " -\n" + data.name + "\nHöhe: " + data.y + "m üNN.";
        text.fontStyle = FontStyles.Bold;

        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = lookAtOrigin;
        source.weight = -1;
        var lookAt = canvasWrapper.AddComponent<LookAtConstraint>();
        lookAt.AddSource(source);
        lookAt.constraintActive = true;

        canvasWrapper.SetActive(false);

        return canvasWrapper;
    }

    /// <summary>
    /// Checks whether or not the filter contains a stamp with the given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if the filter contains this id, false else</returns>
    private bool FilterContains(int id)
    {
        foreach (int i in this.filter) {
            if (i-1 == id) return true;
        }
        return false;
    }

    /// <summary>
    /// Destroys all children / all stamp game objects
    /// </summary>
    private void Clear()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }
}
