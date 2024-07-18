using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

/// <summary>
/// This class manages hiking trails (wanderrouten)
/// It creates the splines, assigns a material for each difficulty, 
/// </summary>
public class WanderroutenManager : MonoBehaviour
{
    public TerrainManager terrainManager;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    [InspectorButton("Clear")]
    public bool clear;

    public bool regenerateOnGameStart = false;

    public Transform ParentTextfields;
    private Vector3 textPosition = new Vector3(1, 1, 1);
    private float textSize = 0.25f;
    private static int counterRoutes = 0;
    private bool visibility = true;
    
    public long[] filter;

    public bool filterWhitelist = true;

    public TextAsset alternativeFilterFile;

    /// <summary>
    /// Deactivates all Routes except the provided one
    /// </summary>
    /// <param name="selectedStamp"></param>
    public void DeactivateAllBut(GameObject selectedStamp)
    {
        GameObject[] stamps = this.GetComponentsInChildren<GameObject>();
        foreach (GameObject stamp in stamps)
        {
            if (stamp.name.Equals(selectedStamp.name))
                continue;
           
            stamp.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles the visibility of the wanderoutenManager and therefore all its childs
    /// </summary>
    public void ToggleVisibilty()
    {
        visibility = !visibility;
        gameObject.SetActive(visibility);
    }

    /// <summary>
    /// Removes all existing routes and regenerates them.
    /// This method iterates over the list of all saved routes, creates splines and populates the splines with the routes waypoints.
    /// It assigns a material which depends on the difficulty of the route and activates / deactivates the route depending on if the route extends beyond the boundries of the map
    /// </summary>
    public void Regenerate()
    {
        Debug.Log("Regenerating Routes");
        Clear();

        HikeSettings settings = HikeSettings.GetOrCreateSettings();
        CoordinateService coordinateService = CoordinateService.GetInstance();

        Route[] routes = GetRoutes(settings.routesAssetPath);
        foreach (Route route in routes)
        {

            foreach (Content content in route.contents)
            {
                GameObject routeObj = new GameObject(content.id + ": " + content.title);

                SplineContainer splineContainer = routeObj.AddComponent<SplineContainer>();
                routeObj.transform.parent = this.transform;
                routeObj.transform.localPosition = Vector3.zero;
                routeObj.isStatic = true;

                bool invalidRoute = false;
                Spline spline = splineContainer.AddSpline();
                List<BezierKnot> knots = new List<BezierKnot>();
                foreach (Coord coord in content.geoJson.coordinates)
                {
                    //GameObject routeNode = Instantiate(this.routeNodePrefab, routeObj.transform);
                    //routeNode.transform.localPosition = new Vector3(coord.x, coord.y, coord.z);
                    Vector3 coordVec = coordinateService.convertETRSToUnity(new Vector3((float)coord.x, (float)coord.y, (float)coord.z));
                    float height = terrainManager.calculateHeightAt(coordVec.x, coordVec.z, settings.mapHeightmapResolution, settings.GetTerrainIndex());
                    if (height <= 0)
                        invalidRoute = true;

                    knots.Add(new BezierKnot(coordVec));
                }
                routeObj.SetActive(!invalidRoute);
                spline.Knots = knots;
                SplineExtrude extrude = routeObj.AddComponent<SplineExtrude>();
                extrude.Radius = settings.routesSplineRadius;
                extrude.SegmentsPerUnit = Mathf.RoundToInt(settings.routesSplineSidesMultiplier * knots.Count);
                extrude.Rebuild();

                int difficulty = content.ratingInfo.difficulty;
                Debug.Log("Difficulty = " +  difficulty);
                Material material;
                switch (difficulty)
                {
                    case 1: 
                        material = Resources.Load<Material>(settings.routesDifficultyMaterialPath + "/easy");
                        break;
                    case 0:
                    case 2:
                        material = Resources.Load<Material>(settings.routesDifficultyMaterialPath + "/medium");
                        break;
                    case 3:
                        material = Resources.Load<Material>(settings.routesDifficultyMaterialPath + "/hard");
                        break;
                    default: 
                        material = Resources.Load<Material>(settings.routesDifficultyMaterialPath + "/unknown");
                        break;
                }

                MeshRenderer renderer = routeObj.GetComponent<MeshRenderer>();
                renderer.material = material;
            }

            
            
            DrawText(counterRoutes, route.contents[0].title, route.contents[0].id, route.contents[0].ratingInfo.difficulty);
            counterRoutes++;
        }
    }
    
    /// <summary>
    /// Creates a new Canvas to display route information
    /// </summary>
    /// <param name="counter"></param>
    /// <param name="title"></param>
    /// <param name="idRoute"></param>
    /// <param name="rating"></param>
    private void DrawText(int counter, string title, string idRoute, int rating)         //Canvas für Darstellung der Informationen der jeweiligen Wanderroute
    {
        GameObject textObject = new GameObject("TextField" + counter);

        textObject.transform.parent = ParentTextfields;

        ParentTextfields.transform.position = textPosition;
        
        textObject.SetActive(false);

        TextMeshPro textDisplayed = textObject.AddComponent<TextMeshPro>();

        textDisplayed.text = title + "\n" + idRoute + "\n" + rating;

        textDisplayed.rectTransform.transform.position = textPosition;

        textDisplayed.fontSize = textSize;

        textDisplayed.alignment = TextAlignmentOptions.Center;

        // Position des Textes setzen

        // Optional: Textfeld als Kindobjekt des aktuellen Objekts setzen
        //textObject.transform.SetParent(this.transform);
    }

    /// <summary>
    /// Reads the contents of the filter file
    /// </summary>
    /// <returns>array of route ids - the filter</returns>
    private long[] ReadFilterFile()
    {
        if (alternativeFilterFile == null)
            return this.filter;

        string[] lines = alternativeFilterFile.text.Split("\n");
        long[] filter = new long[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            try
            {
                filter[i] = long.Parse(lines[i]);
            }
            catch
            { 
                continue;
            }
        }
        return filter;
    }

    /// <summary>
    /// Returns a Array containing all routes (filtered) which are stored in the provided asset path
    /// </summary>
    /// <param name="routesAssetPath"></param>
    /// <returns></returns>
    private Route[] GetRoutes(string routesAssetPath)
    {
        DirectoryInfo info = new DirectoryInfo(routesAssetPath);
        //Debug.Log(routesAssetPath);
        //DEBUG Filter nur nach Goetheweg (Torfhaus - Brocken)
        FileInfo[] fileInfo = info.GetFiles("*.json");   //Sonst *.json nutzen
        List<FileInfo> filteredInfo = new List<FileInfo>();
        long[] filter = ReadFilterFile();

        foreach (FileInfo fi in fileInfo)
        {
            String idRaw = fi.Name.Substring(0, fi.Name.Length -5);
            Debug.Log(idRaw);
            long id = long.Parse(idRaw);

            if (filter.Length > 0)
            {
                if (FilterContains(filter, id) == filterWhitelist)
                    filteredInfo.Add(fi);
            }
        }

        Route[] routes = new Route[filteredInfo.Count];
        //Debug.Log($"File Count:{routes.Length}");
        
        for (int i = 0; i < filteredInfo.Count; i++)
        {
            FileInfo file = filteredInfo[i];

            String content = File.ReadAllText(file.FullName);
            Route route = WanderroutenReader.ReadRoute(content);

            routes[i] = route;
            
        }
        return routes;
    }

    /// <summary>
    /// Executed on startup
    /// </summary>
    void Start()
    {
        if (regenerateOnGameStart)
            Regenerate();
    }
    
    /// <summary>
    /// Destroys all children / routes
    /// </summary>
    private void Clear()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Checks whether or not the filter contains a route id 
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="id"></param>
    /// <returns>true, if the filter contains this id, else false</returns>
    private bool FilterContains(long[] filter, long id)
    {
        foreach (long i in filter)
        {
            if (i == id) return true;
        }
        return false;
    }
}
