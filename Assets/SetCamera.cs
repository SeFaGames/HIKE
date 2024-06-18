using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Animations;

public class SetCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera camera = Object.FindObjectOfType<Camera>();
        LookAtConstraint lookAt = GetComponentInChildren<LookAtConstraint>();
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = camera.transform;
        lookAt.AddSource(source);
        lookAt.constraintActive = true;
    }
}
