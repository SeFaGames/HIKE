using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingWheel : MonoBehaviour
{
    public float rotateSpeed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}
