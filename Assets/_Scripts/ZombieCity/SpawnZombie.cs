using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnZombie : MonoBehaviour
{
    [Header("Prefab & So luong")]
    public GameObject zombiePrefab;
    public int fixedSpawnCount = 5;
    public int randomSpawnCount = 10;

    [Header("Spawn random")]
    public float randomSpawnRadius = 50f;
    public Vector3 randomCenter = Vector3.zero;

    [Header("Diem spawn co dinh")]
    public Transform[] fixedSpawnPoints;

    [Header("NavMesh")]
    public float navMeshCheckDistance = 2f;
    public LayerMask navMeshMask;

    private List<GameObject> spawnZombies = new List<GameObject>();

    private void Start()
    {
        SpawnFixedZombies();
        SpawnRandomZombies();
    }

    private void SpawnRandomZombies()
    {
        for(int i = 0; i< randomSpawnCount; i++)
        {
            Vector3 spawnPos = GetRandomNavMeshPoint(randomCenter, randomSpawnRadius);
            GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
            spawnZombies.Add(zombie);
        }
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        for(int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(randomPos, out hit, navMeshCheckDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return center;
    }

    private void SpawnFixedZombies()
    {
        if (fixedSpawnPoints.Length == 0) return;
        for(int i = 0; i < fixedSpawnCount; i++)
        {
            Transform spawnPoint = fixedSpawnPoints[Random.Range(0, fixedSpawnPoints.Length)];
            GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
            spawnZombies.Add(zombie);
        }
    }

}
