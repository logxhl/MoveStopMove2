using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SpawnZombie : MonoBehaviour
{
    public static SpawnZombie instance;
    [Header("Prefab & Số lượng")]
    public GameObject zombiePrefab;
    public int fixedSpawnCount = 5;
    public int randomSpawnCount = 10;

    [Header("Spawn random")]
    public float randomSpawnRadius = 50f;
    public Vector3 randomCenter = Vector3.zero;

    [Header("Điểm spawn cố định")]
    public Transform[] fixedSpawnPoints;

    [Header("NavMesh")]
    public float navMeshCheckDistance = 2f;

    [Header("Spawn Timing")]
    public float spawnDelay = 2f;

    [Header("UI")]
    public TextMeshProUGUI aliveCountText;

    private List<GameObject> spawnZombies = new List<GameObject>();
    private int totalZombiesToSpawn;
    private int zombiesAlive;
    private bool playerAlive = true;
    private int diedCount = 0;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        totalZombiesToSpawn = fixedSpawnCount + randomSpawnCount;
        zombiesAlive = totalZombiesToSpawn;

        UpDateAliveUI();

        StartCoroutine(SpawnZombies());
    }

    private IEnumerator SpawnZombies()
    {
        // Spawn cố định
        for (int i = 0; i < fixedSpawnCount; i++)
        {
            Transform spawnPoint = fixedSpawnPoints[Random.Range(0, fixedSpawnPoints.Length)];
            Transform enemyPos = SpawnZombieAt(spawnPoint.position, Quaternion.identity);
            EnemyIndicatorManager.instance.RegisterEnemy(enemyPos);
            yield return new WaitForSeconds(spawnDelay);
        }

        // Spawn random
        for (int i = 0; i < randomSpawnCount; i++)
        {
            Vector3 spawnPos = GetRandomNavMeshPoint(randomCenter, randomSpawnRadius);
            Transform enemyPos = SpawnZombieAt(spawnPos, Quaternion.identity);
            EnemyIndicatorManager.instance.RegisterEnemy(enemyPos);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private Transform SpawnZombieAt(Vector3 pos, Quaternion rot)
    {
        GameObject zombie = Instantiate(zombiePrefab, pos, rot);

        spawnZombies.Add(zombie);
        zombiesAlive++;
        UpDateAliveUI();
        return zombie.transform;
    }

    public int GetZombieAlive()
    {
        return zombiesAlive;
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, navMeshCheckDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return center;
    }
    public void NotifyCharacterDied(bool isPlayer = false)
    {
        Debug.Log("Vao day");
        if (isPlayer)
        {
            if (playerAlive)
            {
                playerAlive = false;
            }
        }

        diedCount++;
        if (diedCount > totalZombiesToSpawn) diedCount = totalZombiesToSpawn;
        UpDateAliveUI();
    }

    public int GetRemainingCount()
    {
        int remainingEnemies = Mathf.Max(0, totalZombiesToSpawn - diedCount);
        return remainingEnemies;
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


