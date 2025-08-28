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
    public GameObject zombieDog;
    public GameObject bossLv1;
    public GameObject bossLv2;

    public int fixedSpawnCount = 5;
    public int randomSpawnCount = 25;

    [Header("Zombie color")]
    public Material[] zombieMaterials;
    public Material[] dogMaterials;

    [Header("Spawn random")]
    public float randomSpawnRadius = 50f;
    public Vector3 randomCenter = Vector3.zero;

    [Header("Điểm spawn cố định")]
    public Transform[] fixedSpawnPoints;

    [Header("NavMesh")]
    public float navMeshCheckDistance = 2f;

    [Header("Spawn Timing")]
    public float spawnDelay = 2f;
    public float bossSpawnDelay = 3f;

    [Header("UI")]
    public TextMeshProUGUI aliveCountText;

    private int zombiesKilled = 0;
    private int bossLv1Spawned = 0;
    private bool bossLv2Spawned = false;
    private int maxBossLv1 = 3;

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

        totalZombiesToSpawn = fixedSpawnCount + randomSpawnCount + 4;
        zombiesAlive = totalZombiesToSpawn;

        UpDateAliveUI();
        StartCoroutine(SpawnZombies());
    }

    private IEnumerator SpawnZombies()
    {
        // Spawn cố định
        for (int i = 0; i < fixedSpawnCount; i++)
        {
            if (!playerAlive) yield break;
            Transform spawnPoint = fixedSpawnPoints[Random.Range(0, fixedSpawnPoints.Length)];
            GameObject zombies = Random.Range(0f, 1f) < 0.7f ? zombiePrefab : zombieDog;
            Transform enemyPos = SpawnZombieAt(spawnPoint.position, Quaternion.identity, zombies);
            yield return new WaitForSeconds(spawnDelay);
        }

        // Spawn random
        for (int i = 0; i < randomSpawnCount; i++)
        {
            if(!playerAlive) yield break;
            Vector3 spawnPos = GetRandomNavMeshPoint(randomCenter, randomSpawnRadius);
            GameObject zombies = Random.Range(0f, 1f) < 0.6f ? zombiePrefab : zombieDog;
            Transform enemyPos = SpawnZombieAt(spawnPos, Quaternion.identity, zombies);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private Transform SpawnZombieAt(Vector3 pos, Quaternion rot, GameObject prefabZombie)
    {
        GameObject zombie = Instantiate(prefabZombie, pos, rot);

        ApplyRandomColor(zombie, prefabZombie);

        spawnZombies.Add(zombie);
        zombiesAlive++;
        UpDateAliveUI();
        return zombie.transform;
    }
    private void ApplyRandomColor(GameObject zombie, GameObject prefabZombie)
    {
        Material[] materialsToUse = null;

        // Chọn materials phù hợp với loại enemy
        if (prefabZombie == zombiePrefab)
            materialsToUse = zombieMaterials;
        else if (prefabZombie == zombieDog)
            materialsToUse = dogMaterials;
        // Kiểm tra có material để áp dụng không
        if (materialsToUse == null || materialsToUse.Length == 0)
        {
            Debug.LogWarning($"Không có material nào cho {prefabZombie.name}!");
            return;
        }

        // Tìm renderer trong enemy
        SkinnedMeshRenderer skinnedRenderer = zombie.GetComponentInChildren<SkinnedMeshRenderer>();
        MeshRenderer meshRenderer = zombie.GetComponentInChildren<MeshRenderer>();

        if (skinnedRenderer != null)
        {
            Material randomMaterial = materialsToUse[Random.Range(0, materialsToUse.Length)];
            skinnedRenderer.material = randomMaterial;
        }
        else if (meshRenderer != null)
        {
            Material randomMaterial = materialsToUse[Random.Range(0, materialsToUse.Length)];
            meshRenderer.material = randomMaterial;
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy Renderer trong enemy: {zombie.name}");
        }
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
        //Debug.Log("Vao day");
        NotifyEnemyKilled(isPlayer);
    }

    public void NotifyEnemyKilled(bool isPlayer = false)
    {
        if (isPlayer)
        {
            if (playerAlive)
            {
                playerAlive = false;
                StopAllCoroutines();
            }
            return;
        }

        zombiesKilled++;
        diedCount++;
        if (diedCount > totalZombiesToSpawn) diedCount = totalZombiesToSpawn;
        UpDateAliveUI();
        zombiesAlive--;

        if (!playerAlive) return;
        // Spawn Boss Level 1 mỗi khi giết được 10 zombie
        if (zombiesKilled % 10 == 0 && bossLv1Spawned < maxBossLv1)
        {
            StartCoroutine(SpawnBossLevel1());
        }

        // Spawn Boss Level 2 khi còn 5 enemies sống
        if (GetRemainingCount() == 5 && !bossLv2Spawned)
        {
            StartCoroutine(SpawnBossLevel2());
        }

        UpDateAliveUI();

        //Debug.Log($"🎯 Zombies killed: {zombiesKilled}, Remaining: {GetRemainingCount()}");
    }
    private IEnumerator SpawnBossLevel1()
    {
        yield return new WaitForSeconds(bossSpawnDelay);

        Vector3 spawnPos = GetRandomNavMeshPoint(randomCenter, randomSpawnRadius);
        SpawnZombieAt(spawnPos, Quaternion.identity, bossLv1);

        bossLv1Spawned++;
        //Debug.Log($"Boss Level 1 spawned! ({bossLv1Spawned}/{maxBossLv1})");
    }

    private IEnumerator SpawnBossLevel2()
    {
        yield return new WaitForSeconds(bossSpawnDelay);

        Vector3 spawnPos = GetRandomNavMeshPoint(randomCenter, randomSpawnRadius);
        SpawnZombieAt(spawnPos, Quaternion.identity, bossLv2);

        bossLv2Spawned = true;
        Debug.Log("Boss Level 2 (Final Boss) spawned!");
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
    // Thêm các phương thức utility để kiểm tra trạng thái
    public int GetZombiesKilled()
    {
        return zombiesKilled;
    }

    public int GetBossLevel1Count()
    {
        return bossLv1Spawned;
    }

    public bool IsBossLevel2Spawned()
    {
        return bossLv2Spawned;
    }

    public bool IsAllEnemiesDead()
    {
        return zombiesAlive <= 0;
    }
}