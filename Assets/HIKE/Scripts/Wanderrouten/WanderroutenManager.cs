using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;

public class WanderroutenManager : MonoBehaviour
{

    public String routesAssetPath = "Data/Wanderrouten";
    public GameObject routeNodePrefab;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    [InspectorButton("Clear")]
    public bool clear;

    public void Regenerate()
    {
        Debug.Log("Regenerating Routes");
        Clear();

        Route[] routes = GetRoutes();
        foreach (Route route in routes)
        {
            foreach (Content content in route.contents)
            {
                GameObject routeObj = new GameObject(content.id + ": " + content.title);
                SplineContainer splineContainer = routeObj.AddComponent<SplineContainer>();
                routeObj.transform.parent = this.transform;

                Spline spline = splineContainer.AddSpline();
                List<BezierKnot> knots = new List<BezierKnot>();
                foreach (float[] coord in content.geoJson.coordinates)
                {
                    //GameObject routeNode = Instantiate(this.routeNodePrefab, routeObj.transform);
                    //routeNode.transform.localPosition = new Vector3(coord.x, coord.y, coord.z);
                    Vector3 coordVec = new Vector3(coord[0], coord[1], coord[2]);
                    Debug.Log(coordVec);
                    knots.Add(new BezierKnot(coordVec));
                }
                spline.Knots = knots;
            }
        }
    }

    private Route[] GetRoutes()
    {
        DirectoryInfo info = new DirectoryInfo(routesAssetPath);
        Debug.Log(routesAssetPath);
        FileInfo[] fileInfo = info.GetFiles("*.json");
        Route[] routes = new Route[fileInfo.Length];
        Debug.Log($"File Count:{routes.Length}");
        
        for (int i = 0; i < fileInfo.Length; i++)
        {
            FileInfo file = fileInfo[i];
            String content = File.ReadAllText(file.FullName);
            Route route = WanderroutenReader.ReadRoute(content);
            Debug.Log(route.contents);
            routes[i] = route;
            
        }
        return routes;
    }

    void Start()
    {
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
