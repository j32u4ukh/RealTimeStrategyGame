using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform main_camera_transform;
     
    // Start is called before the first frame update
    void Start()
    {
        main_camera_transform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + main_camera_transform.rotation * Vector3.forward,
                         main_camera_transform.rotation * Vector3.up);
    }
}
