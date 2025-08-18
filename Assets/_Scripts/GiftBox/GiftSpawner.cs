using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool giftPool;
    [SerializeField] private float spawnInterval = 20f; //thoi gian giua cac lan spawn
    [SerializeField] private Vector2 spawnRangeX = new Vector2(-10, 10);
    [SerializeField] private float spawnHeight = 10f;

    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            SpawnGift();
            timer = spawnInterval;
        }
    }

    private void SpawnGift()
    {
        GameObject gift = giftPool.GetFromPool();
        //random vi tri roi
        float randomX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        Vector3 spawnPos = new Vector3(randomX, spawnHeight, 0f);
        gift.transform.position = spawnPos;
        gift.transform.rotation = Quaternion.identity;

        Rigidbody rb = gift.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
