using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// This class is used to handle a xr interaction with a stamp (Stempelstelle)
/// </summary>
public class StampInteractionHandler : MonoBehaviour
{
    /// <summary>
    /// Sets the visibiltiy of the information canvas to the provided state
    /// </summary>
    /// <param name="state">visibility state - true = visibile</param>
    public void SetCanvasVisibility(bool state)
    {
        Canvas canvas = GetComponentInChildren<Canvas>(true);
        if (canvas != null)
            canvas.gameObject.SetActive(state);
        else
            Debug.Log("Canvas not found");
        return;
    }

    /// <summary>
    /// Toggles the visibility of the information canvas
    /// </summary>
    public void ToggleCanvasVisibility()
    {
        bool state = this.gameObject.activeSelf;
        this.gameObject.SetActive(!state);
        return;
    }
}
