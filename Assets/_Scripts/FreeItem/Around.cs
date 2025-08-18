using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Around : MonoBehaviour
{
    [SerializeField] private float aroundSpeed = 3f;
    private void Update()
    {
        transform.Rotate(0f, 0f, aroundSpeed * Time.deltaTime);
    }
}
