using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;
    public static PoolManager Instance { get { return instance; } }

    private Dictionary<GameObject, List<GameObject>> pool = new Dictionary<GameObject, List<GameObject>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetObj(GameObject prefab)
    {
        List<GameObject> listObj = new List<GameObject>();
        if (pool.ContainsKey(prefab))
            listObj = pool[prefab];
        else
            pool.Add(prefab, listObj);

        foreach (GameObject obj in listObj)
        {
            if (obj.activeSelf)
                continue;
            return obj;
        }

        GameObject newObj = Instantiate(prefab);
        listObj.Add(newObj);
        return newObj;
    }
}