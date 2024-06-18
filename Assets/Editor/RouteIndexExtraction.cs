using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class RouteIndexExtraction
{
    private static string path = "Z:/u37052/indizes.txt";

    [MenuItem("Assets/Extract Index")]
    private static void ExtractIndex()
    {
        var file = File.CreateText(path);

        foreach (UnityEngine.Object o in Selection.objects)
        {

            if (o.GetType() != typeof(GameObject))
            {
                Debug.LogError("This isn't a GameObject: " + o);
                continue;
            }

            GameObject gameObject = (GameObject)o;
            string name = gameObject.name;
            string indexRaw = name.Substring(0, name.IndexOf(':'));
            file.WriteLine(indexRaw);

        }
        file.Close();
        Debug.Log("Done!");
    }
}
