using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

    private Route[] GetRoutes(string routesAssetPath)
    {
        DirectoryInfo info = new DirectoryInfo(routesAssetPath);
        //Debug.Log(routesAssetPath);
        //DEBUG Filter nur nach Goetheweg (Torfhaus - Brocken)
        FileInfo[] fileInfo = info.GetFiles("*.json");   //Sonst *.json nutzen
        Route[] routes = new Route[fileInfo.Length];
        //Debug.Log($"File Count:{routes.Length}");
        
        for (int i = 0; i < fileInfo.Length; i++)
        {
            FileInfo file = fileInfo[i];

            String content = File.ReadAllText(file.FullName);
            Route route = WanderroutenReader.ReadRoute(content);
            //Debug.Log(route.contents);
            //Debug.Log(route.contents.Length);
            //Debug.Log(route.contents[0].geoJson);
            //Debug.Log(route.contents[0].geoJson.coordinates);
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
}
