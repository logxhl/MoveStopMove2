using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuScene : MonoBehaviour
{
    public static PlayerMenuScene instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
