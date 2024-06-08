using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeScript : MonoBehaviour
{
    public TerrainManager terrainManager;

    [InspectorButton("Regenerate")]
    public bool regenerate;

    public float tableHeight = 0.8f;
    public float edgeMargin = 0.2f;
    public float heightOffset = 0.0015f;

    public void Regenerate()
    {
        Vector3 terrainSize = terrainManager.GetTerrainTotalWorldSize();
        Vector3 scale = new Vector3(terrainSize.x + 2 * edgeMargin, tableHeight, terrainSize.z + 2 * edgeMargin);

        this.transform.localScale = scale;
        this.transform.localPosition = new Vector3(terrainSize.x / 2, (tableHeight / 2 ) + heightOffset, terrainSize.z / 2);
    }
}
