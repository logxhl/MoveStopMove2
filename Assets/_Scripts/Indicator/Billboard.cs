using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }
    private void LateUpdate()
    {
        if(cam != null)
        {
            transform.forward = cam.transform.forward;
        }
    }
}
