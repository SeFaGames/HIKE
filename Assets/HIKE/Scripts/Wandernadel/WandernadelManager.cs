using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandernadelManager : MonoBehaviour
{
    public TextAsset dataFile;
    public GameObject wandernadelObject;
    public TerrainManager terrainManager;

    private Transform[] wandernadeln;


    // Start is called before the first frame update
    void Start()
    {
        WandernadelData[] stempelstellen = JsonUtility.FromJson<WandernadelDataArray>(dataFile.text).stempelstellen;
        wandernadeln = new Transform[stempelstellen.Length];

        TerrainIndex index = terrainManager.Index;

        for (int i = 0; i < stempelstellen.Length; i++)
        {
            var data = stempelstellen[i];
            var stempelstelle = Instantiate(wandernadelObject, this.transform);
            stempelstelle.name = i + " - " + data.name;
            stempelstelle.SetActive(true);
            stempelstelle.transform.position = new Vector3(data.x-index.x.min, (data.y-index.y.min)*terrainManager.heightScaleFactor, data.z-index.z.min);
            wandernadeln[i] = stempelstelle.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
