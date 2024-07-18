using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// This class provides methods to convert real world coordinates to unity coordinates while considering the model scale and height scale.
/// </summary>
public class CoordinateService
{    
    private TerrainIndex index;
    private float scaleFactor;
    private float heightScaleFactor;

    public CoordinateService()
    {
        HikeSettings settings = HikeSettings.GetOrCreateSettings();
        TextAsset indexFile = settings.dgmIndexFile;

        this.index = JsonUtility.FromJson<TerrainIndex>(indexFile.text);
        this.scaleFactor = settings.mapScaleFactor;
        this.heightScaleFactor = settings.mapHeightScaleFactor;
    }

    /// <summary>
    /// Returns a Instance of this class
    /// </summary>
    /// <returns>new CoordinateService Instance</returns>
    public static CoordinateService GetInstance()
    {
        return new CoordinateService();
    }

    /// <summary>
    /// Converts ETRS89 Coordinates into Unity World Coordinates
    /// </summary>
    /// <param name="etrsCoordinate"></param>
    /// <returns>Vector3 - Unity World Coordinates</returns>
    public Vector3 convertETRSToUnity(Vector3 etrsCoordinate)
    {
        Debug.Log($"Converting ETRS {etrsCoordinate}");
        float xPos = convertETRSToUnity(etrsCoordinate.x, Axis.X);
        float yPos = convertHeightETRSToUnity(etrsCoordinate.y);
        float zPos = convertETRSToUnity(etrsCoordinate.z, Axis.Z);

        Debug.Log($"ETRS = x={xPos}, y={yPos}, z={zPos}");
        return new Vector3 (xPos, yPos, zPos);
    }

    /// <summary>
    /// Converts Unity World Coordinates into ETRS89 Geographic Coordiantes
    /// </summary>
    /// <param name="unityCoordinate"></param>
    /// <returns>Vector3 ETRS89 Coordinates</returns>
    public Vector3 convertUnityToETRS(Vector3 unityCoordinate)
    {
        float xPos = convertUnityToETRS(unityCoordinate.x, Axis.X);
        float yPos = convertHeightUnityToETRS(unityCoordinate.y);
        float zPos = convertUnityToETRS(unityCoordinate.z, Axis.Z);

        return new Vector3(xPos, yPos, zPos);
    }

    /// <summary>
    /// Converts a ETRS89 Coordinate into Unity World Coordinate
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="axis"> World Axis in which the convertation takes place (y is treated diffrent to x and z)</param>
    /// <returns>A Unity World Coordinate </returns>
    /// <exception cref="Exception"></exception>
    public float convertETRSToUnity(float coord, Axis axis)
    {
        Debug.Log($"Axis {axis}");
        if (axis == Axis.None)
            throw new Exception("Axis must be X, Y, or Z");

        if (axis.Equals(Axis.Y))
            return convertHeightETRSToUnity(coord);

        float min;
        if (axis.Equals(Axis.Z))
            min = this.index.z.min;
        else
            min = this.index.x.min;

        Debug.Log($"coord={coord} min={min} scaleFactor={this.scaleFactor}");

        return (coord - min) * this.scaleFactor;
    }

    /// <summary>
    /// Converts a Unity World Coordinate Coordinate into a ETRS89 Coordinate
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="axis">World Axis in which the convertation takes place (y is treated diffrent to x and z)</param>
    /// <returns>A ETRS89 Coordinate</returns>
    /// <exception cref="Exception"></exception>
    public float convertUnityToETRS(float coord, Axis axis)
    {
        if (axis == Axis.None)
            throw new Exception("Axis must be X, Y, or Z");

        if (axis.Equals(Axis.Y))
            return convertHeightUnityToETRS(coord);

        float min;
        if (axis.Equals(Axis.Z))
            min = this.index.z.min;
        else
            min = this.index.x.min;

        return (coord / this.scaleFactor) + min;
    }

    /// <summary>
    /// Converts a ETRS y Coordinate to a Unity World y Coordinate
    /// </summary>
    /// <param name="height"></param>
    /// <returns>Unity World y Coordinate</returns>
    public float convertHeightETRSToUnity(float height)
    {
        Debug.Log("Axis y");
        Debug.Log($"height={height}");
        Debug.Log($"min = {this.index.y.min}");
        Debug.Log($"heightScaleFactor = {this.heightScaleFactor}");
        Debug.Log($"scaleFactor = {this.scaleFactor}");
        return ((height - this.index.y.min) * this.heightScaleFactor) * this.scaleFactor;
    }

    /// <summary>
    /// Converts a Unity World y Coordinate to a ETRS89 y Coordinate
    /// </summary>
    /// <param name="height"></param>
    /// <returns>ETRS89 y Coordinate</returns>
    public float convertHeightUnityToETRS(float height)
    {
        return (height / (this.heightScaleFactor * this.scaleFactor)) + this.index.y.min;
    }

    public float ScaleFactor { get { return this.scaleFactor; } }
}
