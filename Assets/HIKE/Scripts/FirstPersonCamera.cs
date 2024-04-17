using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float mouseSensitiviy = 2;
    public bool lockedCursor = true;
    public float speed = 5;

    float cameraVerticalRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (lockedCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Component camera = this.GetComponentInChildren<Camera>();

        float inputX = Input.GetAxis("Mouse X") * mouseSensitiviy;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitiviy;
        

        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        camera.transform.localEulerAngles=Vector3.right*cameraVerticalRotation;

        transform.Rotate(Vector3.up * inputX);

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical"));
        transform.position += transform.TransformDirection(movement * speed * Time.deltaTime);
    }
}
