using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    public Camera cam;
    private Vector3 skinCam;
    private Vector3 originalCam;
    public float moveSpeed = 5f;
    private void Awake()
    {
        cam = Camera.main;
        skinCam = new Vector3(0.17f, 0.6f, -13.0f);
        originalCam = cam.transform.position;
    }
    public void SkinCam()
    {
        StopAllCoroutines();
        StartCoroutine(MoveCam(skinCam));
    }
    public void ResetCam()
    {
        StopAllCoroutines();
        StartCoroutine(MoveCam(originalCam));
    }

    private IEnumerator MoveCam(Vector3 target)
    {
        while(Vector3.Distance(cam.transform.position, target) > 0.01f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        cam.transform.position = target;
    }
}
