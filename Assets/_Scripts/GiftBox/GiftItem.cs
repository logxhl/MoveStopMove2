using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftItem : MonoBehaviour
{
    [SerializeField] private ObjectPool pool;
    public void SetPool(ObjectPool objectPool)
    {
        pool = objectPool;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag(Params.PlayerTag))
        {
            Debug.Log("Va cham");
            pool.ReturnToPool(gameObject);
        }
    }
}
