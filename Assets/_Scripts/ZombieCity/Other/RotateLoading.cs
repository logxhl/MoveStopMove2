using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoading : MonoBehaviour
{
    public float speed = 200f;
    public GameObject panelLoad;
    private void Start()
    {
        StartCoroutine(HidePanelAfterTime(3f));
    }
    private void Update()
    {
        transform.Rotate(0f, 0f, -speed * Time.deltaTime);
    }
    private IEnumerator HidePanelAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        panelLoad.SetActive(false);
    }
}
