using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandernadelManager : MonoBehaviour
{
    public TextAsset dataFile;
    public GameObject wandernadelObject;
    public int xOffset = 0;
    public float yOffset = 0;
    public float yMax = 1;
    public int zOffset = 0;

    private Transform[] wandernadeln;


    // Start is called before the first frame update
    void Start()
    {
        WandernadelData[] stempelstellen = JsonUtility.FromJson<WandernadelDataArray>(dataFile.text).stempelstellen;
        wandernadeln = new Transform[stempelstellen.Length];
        for(int i = 0; i < stempelstellen.Length; i++)
        {
            var data = stempelstellen[i];
            var stempelstelle = Instantiate(wandernadelObject, this.transform);
            stempelstelle.name = i + " - " + data.name;
            stempelstelle.SetActive(true);
            stempelstelle.transform.position = new Vector3(data.x-xOffset, ((data.y-yOffset)/(yMax-yOffset))*yMax, data.z-zOffset);
            wandernadeln[i] = stempelstelle.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
