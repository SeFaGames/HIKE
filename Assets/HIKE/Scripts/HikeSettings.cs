using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
            AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}

//ADD NEW PROPERTIES HERE
class Styles
{
    public static GUIContent dgm_indexFile = new GUIContent("Geländemodell - Index-Datei");
    public static GUIContent dgm_dataFile = new GUIContent("Geländemodell - Höhendaten-Datei");

    public static GUIContent map_scaleFactor = new GUIContent("Skalierungsfaktor");
    public static GUIContent map_heightScaleFactor = new GUIContent("Überhöhungsfaktor");
    public static GUIContent map_heightmapResolution = new GUIContent("Auflösung der Heightmap");

    public static GUIContent stamp_scaleFactor = new GUIContent("Skalierungsfaktor (Stempelstelle)");
}

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

        //Stempelstellen
        EditorGUILayout.LabelField("Stempelstellen:");
        EditorGUILayout.PropertyField(settings.FindProperty("stampScaleFactor"), Styles.stamp_scaleFactor);

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