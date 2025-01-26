using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class AdjustResolution : MonoBehaviour
{
    PixelPerfectCamera pixelPerfectCamera;
    void Start()
    {
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        
        AdjustResolutionByScreen();
    }

    void AdjustResolutionByScreen()
    {
        float targetAspect = 1920f / 1080f; // Your target resolution's aspect ratio (16:9)
        float currentAspect = (float)Screen.width / Screen.height;

        if (currentAspect > targetAspect)
        {
            // Wider screens (e.g., 16:10 or ultrawide)
            pixelPerfectCamera.refResolutionX = Mathf.RoundToInt(270f * currentAspect);
            pixelPerfectCamera.refResolutionY = 270;
        }
        else
        {
            // Taller screens
            pixelPerfectCamera.refResolutionX = 480;
            pixelPerfectCamera.refResolutionY = Mathf.RoundToInt(480f / currentAspect);
        }
    }
}
