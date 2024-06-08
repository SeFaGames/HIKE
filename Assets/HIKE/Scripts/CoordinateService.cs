using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

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

    public static CoordinateService GetInstance()
    {
        return new CoordinateService();
    }

    public Vector3 convertETRSToUnity(Vector3 etrsCoordinate)
    {
        Debug.Log($"Converting ETRS {etrsCoordinate}");
        float xPos = convertETRSToUnity(etrsCoordinate.x, Axis.X);
        float yPos = convertHeightETRSToUnity(etrsCoordinate.y);
        float zPos = convertETRSToUnity(etrsCoordinate.z, Axis.Z);

        Debug.Log($"ETRS = x={xPos}, y={yPos}, z={zPos}");
        return new Vector3 (xPos, yPos, zPos);
    }

    public Vector3 convertUnityToETRS(Vector3 unityCoordinate)
    {
        float xPos = convertUnityToETRS(unityCoordinate.x, Axis.X);
        float yPos = convertHeightUnityToETRS(unityCoordinate.y);
        float zPos = convertUnityToETRS(unityCoordinate.z, Axis.Z);

        return new Vector3(xPos, yPos, zPos);
    }

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

    public float convertHeightETRSToUnity(float height)
    {
        Debug.Log("Axis y");
        Debug.Log($"height={height}");
        Debug.Log($"min = {this.index.y.min}");
        Debug.Log($"heightScaleFactor = {this.heightScaleFactor}");
        Debug.Log($"scaleFactor = {this.scaleFactor}");
        return ((height - this.index.y.min) * this.heightScaleFactor) * this.scaleFactor;
    }

    public float convertHeightUnityToETRS(float height)
    {
        return (height / (this.heightScaleFactor * this.scaleFactor)) + this.index.y.min;
    }

    public float ScaleFactor { get { return this.scaleFactor; } }
}
