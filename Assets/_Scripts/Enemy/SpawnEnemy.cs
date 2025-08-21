using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemy : MonoBehaviour
{
    public static SpawnEnemy Instance { get; private set; }

    [Header("Spawn Config")]
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public float spawnY = 0.66f;
    public bool canSpawn = true;


    [Header("Spawn Limit")]
    public int totalEnemiesToSpawn = 5;
    private int spawnCount = 0;

    [Header("Count behavior")]
    public bool includePlayerInCount = false;
    private bool playerAlive = true;
    private int diedCount = 0;

    [Header("UI")]
    public TextMeshProUGUI aliveCountText;

    private int totalAliveCharacters = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        UpDateAliveUI();
        InvokeRepeating(nameof(SpawnEnemyPrefab), 3f, spawnInterval);
    }

    private void Update()
    {
        if (aliveCountText != null)
        {
            aliveCountText.text = $"Alive: {GetRemainingCount()}";
        }
    }
    void SpawnEnemyPrefab()
    {
        if (!canSpawn) { return; }

        if (spawnCount >= totalEnemiesToSpawn)
        {
            canSpawn = false;
            CancelInvoke(nameof(SpawnEnemyPrefab));
            return;
        }        
        Vector3 randomPos = GetRandomPosition();
        GameObject enemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        EnemyIndicatorManager.instance.RegisterEnemy(enemy.transform);
        spawnCount++;
        //totalAliveCharacters++;
        UpDateAliveUI();
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float z = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector3(x, spawnY, z);
    }

    public void NotifyCharacterDied(bool isPlayer = false)
    {
        if (isPlayer)
        {
            if (playerAlive)
            {
                playerAlive = false;
            }
        }

        diedCount++;
        if (diedCount > totalEnemiesToSpawn) diedCount = totalEnemiesToSpawn;
        UpDateAliveUI();
    }

    public int GetRemainingCount()
    {
        int remainingEnemies = Mathf.Max(0, totalEnemiesToSpawn - diedCount);
        int playerPart = (includePlayerInCount && playerAlive) ? 1 : 0;
        return remainingEnemies + playerPart;
    }

    private void UpDateAliveUI()
    {
        if (aliveCountText != null)
        {
            //Debug.Log($"UI: {GetRemainingCount()}");
            aliveCountText.text = $"Alive: {GetRemainingCount()}";
        }
    }
}
