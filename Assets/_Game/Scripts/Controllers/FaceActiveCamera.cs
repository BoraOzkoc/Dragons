using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceActiveCamera : MonoBehaviour
{
    void Update()
    {
        // Find the active camera
        Camera activeCamera = Camera.main;

        if (activeCamera != null)
        {
            // Face the active camera
            transform.LookAt(transform.position + activeCamera.transform.rotation * Vector3.forward,
                activeCamera.transform.rotation * Vector3.up);
        }
        else
        {
            Debug.LogWarning("No active camera found.");
        }
    }
}
