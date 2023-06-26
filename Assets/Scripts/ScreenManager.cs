using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{    
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected Transform backgroundImage;
    protected RectTransform canvasTransform;
    protected Vector2 lastRes;
    protected Vector2 currentRes;
    protected Vector2 referenceRes;
    protected float referenceAspectRatio;    

    protected void SetResolutionData()
    {
        referenceRes = canvas.GetComponent<CanvasScaler>().referenceResolution;
        referenceAspectRatio = referenceRes.x / referenceRes.y;
        canvasTransform = canvas.GetComponent<RectTransform>();
        currentRes = canvasTransform.sizeDelta;
        lastRes = currentRes;
        SetBackgroundSize();
    }

    protected void SetBackgroundSize()
    {
        float currentAspectRatio = currentRes.x / currentRes.y;
        RectTransform rt = backgroundImage.GetComponent<RectTransform>();
        if (currentAspectRatio > referenceAspectRatio) {
            rt.sizeDelta = new Vector2(
                currentRes.x,
                currentRes.x / referenceAspectRatio
            );
        } else if (currentAspectRatio < referenceAspectRatio) {
            rt.sizeDelta = new Vector2(
                currentRes.y * referenceAspectRatio,
                currentRes.y
            );
        } else {
            rt.sizeDelta = new Vector2(
                currentRes.x,
                currentRes.y
            );
        }
    }

    protected bool UpdateCurrentResolution() {        
        currentRes = canvasTransform.sizeDelta;
        return !currentRes.Equals(lastRes);
    }
}
