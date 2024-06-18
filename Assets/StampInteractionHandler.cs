using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class StampInteractionHandler : MonoBehaviour
{
    public void SetCanvasVisibility(bool state)
    {
        Canvas canvas = GetComponentInChildren<Canvas>(true);
        if (canvas != null)
            canvas.gameObject.SetActive(state);
        else
            Debug.Log("Canvas not found");
        return;
    }

    public void ToggleCanvasVisibility()
    {
        bool state = this.gameObject.activeSelf;
        this.gameObject.SetActive(!state);
        return;
    }
}
