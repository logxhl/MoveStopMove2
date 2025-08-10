using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemy : MonoBehaviour
{
    public static SpawnEnemy Instance { get; private set; }
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public float spawnY = 0.66f;
    public bool canSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        InvokeRepeating(nameof(SpawnEnemyPrefab), 3f, spawnInterval);
    }
    void SpawnEnemyPrefab()
    {
        if (!canSpawn) { return; }
        Vector3 randomPos = GetRandomPosition();
        Instantiate(enemyPrefab, randomPos, Quaternion.identity);
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float z = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector3(x, spawnY, z);
    }
}
