using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject giftPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private void Awake()
    {
        //Khoi tao pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(giftPrefab);
            obj.SetActive(false);

            GiftItem giftItem = obj.GetComponent<GiftItem>();
            if (giftItem != null)
            {
                giftItem.SetPool(this);
            }
            pool.Enqueue(obj);
        }
    }

    public GameObject GetFromPool()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            //Neu het obj thi tao them
            GameObject obj = Instantiate(giftPrefab);
            GiftItem giftItem = obj.GetComponent<GiftItem>();
            if (giftItem != null)
            {
                giftItem.SetPool(this);
            }
            return obj;
        }
    }
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
