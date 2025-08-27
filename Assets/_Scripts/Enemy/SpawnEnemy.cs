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

    [SerializeField] private Material[] colorMaterials;

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
    //void SpawnEnemyPrefab()
    //{
    //    if (!canSpawn) { return; }

    //    if (spawnCount >= totalEnemiesToSpawn)
    //    {
    //        canSpawn = false;
    //        CancelInvoke(nameof(SpawnEnemyPrefab));
    //        return;
    //    }

    //    Vector3 randomPos = GetRandomPosition();
    //    GameObject enemyObj = Instantiate(enemyPrefab, randomPos, Quaternion.identity);

    //    Material usedMaterial = ApplyRandomColor(enemyObj);

    //    // ✅ Lấy script EnemyAI từ prefab vừa spawn
    //    EnemyAI enemyAI = enemyObj.GetComponent<EnemyAI>();
    //    if (enemyAI != null)
    //    {
    //        EnemyIndicatorManager.instance.RegisterEnemy(enemyAI);
    //    }
    //    else
    //    {
    //        Debug.LogError("Enemy prefab thiếu component EnemyAI!");
    //    }

    //    spawnCount++;
    //    UpDateAliveUI();
    //}
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
        GameObject enemyObj = Instantiate(enemyPrefab, randomPos, Quaternion.identity);

        // ✅ LẤY ENEMYAI TRƯỚC
        EnemyAI enemyAI = enemyObj.GetComponent<EnemyAI>();
        if (enemyAI == null)
        {
            Debug.LogError("Enemy prefab thiếu component EnemyAI!");
            return;
        }

        // ✅ ĐĂNG KÝ INDICATOR TRƯỚC (với màu mặc định)
        EnemyIndicatorManager.instance.RegisterEnemy(enemyAI);

        // ✅ ÁP DỤNG MÀU SAU (sẽ tự động cập nhật indicator)
        Material usedMaterial = ApplyRandomColor(enemyObj);
        enemyAI.SetEnemyColor(usedMaterial); // Này sẽ gọi UpdateIndicatorColor()

        spawnCount++;
        UpDateAliveUI();
    }

    //private Material ApplyRandomColor(GameObject enemy)
    //{
    //    if (colorMaterials == null || colorMaterials.Length == 0)
    //    {
    //        Debug.Log("Khong co ds material");
    //        return null;
    //    }
    //    SkinnedMeshRenderer skinnedRenderer = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
    //    if(skinnedRenderer != null )
    //    {
    //        Material randomMaterial = colorMaterials[Random.Range(0, colorMaterials.Length)];
    //        skinnedRenderer.material = randomMaterial;
    //        return randomMaterial;
    //    }
    //    else
    //    {
    //        Debug.Log("Khong tim thay skinnedmeshrenderer trong enemy");
    //        return null;
    //    }

    //}
    // Trong SpawnEnemy.ApplyRandomColor(), thêm debug:
    private Material ApplyRandomColor(GameObject enemy)
    {
        if (colorMaterials == null || colorMaterials.Length == 0)
        {
            Debug.Log("Khong co ds material");
            return null;
        }

        SkinnedMeshRenderer skinnedRenderer = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedRenderer != null)
        {
            Material randomMaterial = colorMaterials[Random.Range(0, colorMaterials.Length)];

            skinnedRenderer.material = randomMaterial;
            return randomMaterial;
        }
        else
        {
            Debug.Log("Khong tim thay skinnedmeshrenderer trong enemy");
            return null;
        }
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
