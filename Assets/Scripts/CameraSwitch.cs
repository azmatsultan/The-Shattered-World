using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera camera2D;
    public CinemachineVirtualCamera camera3D;

    private bool is3D = false;

    void Start()
    {
        // Start with the 2D camera active
        camera2D.Priority = 10;
        camera3D.Priority = 0;
    }

    void Update()
    {
        // Toggle view on Tab key press
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            is3D = !is3D;
            ToggleCameraView(is3D);
        }
    }

    void ToggleCameraView(bool switchTo3D)
    {
        if (switchTo3D)
        {
            camera3D.Priority = 10;
            camera2D.Priority = 0;
        }
        else
        {
            camera2D.Priority = 10;
            camera3D.Priority = 0;
        }
    }
}
