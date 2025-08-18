using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHighLight : MonoBehaviour
{
    [SerializeField] private GameObject selectionCircle;
    private void Awake()
    {
        if(selectionCircle != null)
        {
            selectionCircle.SetActive(false);
        }
    }

    public void ShowCircle(bool show)
    {
        if(selectionCircle != null)
        {
            selectionCircle.SetActive(show);
        }
    }
}
