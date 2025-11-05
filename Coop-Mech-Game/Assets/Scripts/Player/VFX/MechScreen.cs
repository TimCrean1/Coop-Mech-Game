using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechScreen : MonoBehaviour
{
    public Camera targetCamera;
    public float renderInterval = 0.1f; // Render every 0.1 seconds (10 FPS)

    void Start()
    {
        if (targetCamera != null)
        {
            targetCamera.enabled = false; // Disable the camera component
            StartCoroutine(RenderCameraRoutine());
        }
    }

    IEnumerator RenderCameraRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(renderInterval);
            targetCamera.Render(); // Manually render the camera
        }
    }
}
