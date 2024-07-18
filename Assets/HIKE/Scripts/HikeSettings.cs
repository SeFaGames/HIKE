using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Hike Settings
/// </summary>
public class HikeSettings : ScriptableObject
{
    public const string settingsPath = "Assets/Settings/HikeSettings.asset";

    //ADD NEW PROPERTIES HERE

    //DGM (Digitales Geländemodell)
    [SerializeField]
    public TextAsset dgmIndexFile;
    [SerializeField]
    public TextAsset dgmDataFile;

    //Karte
    [SerializeField]
    public float mapScaleFactor;
    [SerializeField]
    public float mapHeightScaleFactor;
    [SerializeField]
    public int mapHeightmapResolution;

    //DOP (Digitale Orthofotos)
    [SerializeField]
    public string dopMaterialPath;


    //Stempelstellen
    [SerializeField]
    public float stampScaleFactor;
    [SerializeField]
    public TextAsset stampDataFile;
    [SerializeField]
    public GameObject stampPrefab;

    //Wanderrouten
    [SerializeField]
    public string routesAssetPath;
    [SerializeField]
    public float routesSplineRadius;
    [SerializeField]
    public float routesSplineSidesMultiplier;
    [SerializeField]
    public string routesDifficultyMaterialPath; 

    /// <summary>
    /// Creates settings and assigns default values to the settings
    /// </summary>
    /// <returns></returns>
    internal static HikeSettings GetOrCreateSettings()
    {
        Debug.Log("Retrieving Settings");
        var settings = AssetDatabase.LoadAssetAtPath<HikeSettings>(settingsPath);
        Debug.Log(settings);
        if (settings == null)
        {
            Debug.Log("Creating Settings-File");
            settings = ScriptableObject.CreateInstance<HikeSettings>();
            //Initialize Settings
            settings.dgmIndexFile = null;
            settings.dgmDataFile = null;

            settings.mapScaleFactor = 1.0f;
            settings.mapHeightScaleFactor = 1.5f;
            settings.mapHeightmapResolution = 513;

            settings.dopMaterialPath = "Textures/Terrain/Materials";

            settings.stampScaleFactor = 1.0f;
            settings.stampDataFile = null;
            settings.stampPrefab = null;

            settings.routesAssetPath = null;
            settings.routesDifficultyMaterialPath = null;
            settings.routesSplineRadius = 0.1f;
            settings.routesSplineSidesMultiplier = 2;
            

    AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    /// <summary>
    /// Serializes the settings
    /// </summary>
    /// <returns>Serialized Settings</returns>
    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }

    /// <summary>
    /// Returns a freshly read TerrainIndex Object
    /// </summary>
    /// <returns>terrain index</returns>
    internal TerrainIndex GetTerrainIndex()
    {
        return JsonUtility.FromJson<TerrainIndex>(this.dgmIndexFile.text); 
    }
}

/// <summary>
/// Creates GUIContent Objects for each Setting
/// </summary>
//ADD NEW PROPERTIES HERE
class Styles
{
    public static GUIContent dgm_indexFile = new GUIContent("Index-Datei", "Eine .json Index Datei mit Meta-Informationen über die Höhendaten-Datei");
    public static GUIContent dgm_dataFile = new GUIContent("Höhendaten-Datei", "Eine .bytes Datei mit Höhendaten (zwischen 0 und 1)");

    public static GUIContent map_scaleFactor = new GUIContent("Skalierungsfaktor", "Bestimmt in welchem Größenverhältnis die Miniatur des Harzes zu den realen Dimensionen steht");
    public static GUIContent map_heightScaleFactor = new GUIContent("Überhöhungsfaktor", "Bestimmt einen Multiplikator mit welchem die Höhe des Terrains skaliert");
    public static GUIContent map_heightmapResolution = new GUIContent("Auflösung der Heightmap", "Bestimmt die Anzahl an Samplepunkten pro Seite pro Terrain. Dies beeinflusst die Anzahl an Terainsegmenten die zur Darstellung des gesamten Modells genutzt werden sollen");

    public static GUIContent dop_MaterialPath = new GUIContent("Pfad zum Materialordner", "Lokaler Pfad unter Assets zum Ordner mit den DOP-Materialien");

    public static GUIContent stamp_scaleFactor = new GUIContent("Skalierungsfaktor", "Bestimmt um welchen Faktor die Miniatur der Stempelstelle skaliert wird");
    public static GUIContent stamp_dataFile = new GUIContent("Quelldatei", "Eine .json Datei mit Stempelstelleninformationen");
    public static GUIContent stamp_prefab = new GUIContent("Prefab", "Das Stempelstellen Prefab (GameObject)");

    public static GUIContent routes_assetPath = new GUIContent("Pfad zu den Quelldateien", "Lokaler Pfad unter Assets zum Ordner mit den Wanderrouten");
    public static GUIContent routes_difficultyMaterialPath = new GUIContent("Materialpfad", "Lokaler Pfad unter Assets zum Ordner mit den Schwierigkeitsgrad-Materialien");
    public static GUIContent routes_splineRadius = new GUIContent("Basis-Radius der Splines", "Bestimmt die Dicke der Splines");
    public static GUIContent routes_splineSidesMultiplier = new GUIContent("Kantenmultiplikator der Splines", "Bestimmt den Detaillgrad der Splines, 1 entspricht 1 Knoten pro Samplepunkt, alles > 1 führt zu Interpolation, alles < 1 sorgt für Detaillverlust");
}

/// <summary>
/// Unity Settings Provider.
/// Adds Settings to Project Settigns
/// </summary>
class HikeSettingsProvider : SettingsProvider
{
    private SerializedObject settings;

    const string hikeSettingsPath = "Assets/Settings/HikeSettings.asset";

    public HikeSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base (path, scope) { }

    public static bool IsSettingsAvailable()
    {
        return File.Exists(hikeSettingsPath);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        settings = HikeSettings.GetSerializedSettings();
    }

    public override void OnGUI(string searchContext)
    {
        //ADD NEW PROPERTIES HERE

        //DGM
        EditorGUILayout.LabelField("Geländemodell:");
        EditorGUILayout.PropertyField(settings.FindProperty("dgmIndexFile"), Styles.dgm_indexFile);
        EditorGUILayout.PropertyField(settings.FindProperty("dgmDataFile"), Styles.dgm_dataFile);

        //Karte
        EditorGUILayout.LabelField("Karte:");
        EditorGUILayout.PropertyField(settings.FindProperty("mapScaleFactor"), Styles.map_scaleFactor);
        EditorGUILayout.PropertyField(settings.FindProperty("mapHeightScaleFactor"), Styles.map_heightScaleFactor);
        EditorGUILayout.PropertyField(settings.FindProperty("mapHeightmapResolution"), Styles.map_heightmapResolution);

        //DOP
        EditorGUILayout.LabelField("Orthofotos / Terraintexturen:");
        EditorGUILayout.PropertyField(settings.FindProperty("dopMaterialPath"), Styles.dop_MaterialPath);

        //Stempelstellen
        EditorGUILayout.LabelField("Stempelstellen:");
        EditorGUILayout.PropertyField(settings.FindProperty("stampScaleFactor"), Styles.stamp_scaleFactor);
        EditorGUILayout.PropertyField(settings.FindProperty("stampDataFile"), Styles.stamp_dataFile);
        EditorGUILayout.PropertyField(settings.FindProperty("stampPrefab"), Styles.stamp_prefab);

        //Stempelstellen
        EditorGUILayout.LabelField("Wanderrouten:");
        EditorGUILayout.PropertyField(settings.FindProperty("routesAssetPath"), Styles.routes_assetPath);
        EditorGUILayout.PropertyField(settings.FindProperty("routesDifficultyMaterialPath"), Styles.routes_difficultyMaterialPath);
        EditorGUILayout.PropertyField(settings.FindProperty("routesSplineRadius"), Styles.routes_splineRadius);
        EditorGUILayout.PropertyField(settings.FindProperty("routesSplineSidesMultiplier"), Styles.routes_splineSidesMultiplier);

        settings.ApplyModifiedPropertiesWithoutUndo();
    }

    [SettingsProvider]
    public static SettingsProvider CreateHikeSettingsProvider()
    {
        if(IsSettingsAvailable())
        {
            var provider = new HikeSettingsProvider("Project/HikeSettingsProvider", SettingsScope.Project);

            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }
        return null;
    }
}