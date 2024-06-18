using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class WanderroutenManager : MonoBehaviour
{

    //public String routesAssetPath = "Data/Wanderrouten";
    //public GameObject routeNodePrefab;
    public TerrainManager terrainManager;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    [InspectorButton("Clear")]
    public bool clear;

    public bool regenerateOnGameStart = false;

    public long[] filter;

    public bool filterWhitelist = true;

    public TextAsset alternativeFilterFile;

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
        }
    }

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

    void Start()
    {
        if (regenerateOnGameStart)
            Regenerate();
    }

    private void Clear()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    private bool FilterContains(long[] filter, long id)
    {
        foreach (long i in filter)
        {
            if (i == id) return true;
        }
        return false;
    }
}
