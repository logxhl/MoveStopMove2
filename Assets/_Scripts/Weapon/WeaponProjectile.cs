﻿using System;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    private WeaponData weaponData;       //Data của vũ khí hiện tại

    private LayerMask targetLayer;

    [SerializeField] private float maxLifeTime = 0.6f;   // Sau thời gian này sẽ tự hủy (tránh bay mãi)
    private Vector3 direction;      // Hướng bay của vũ khí
    private float timer;        // Đếm cho đến thời gian biến mất
    [SerializeField] private float rotateSpeed;

    private Rigidbody _rigidbody;

    public WeaponAttack owner;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction, LayerMask targetLayer, WeaponData weaponData, WeaponAttack owner)
    {
        this.direction = direction.normalized;
        this.targetLayer = targetLayer;
        this.weaponData = weaponData;
        this.owner = owner;
        this.timer = 0f;
        gameObject.SetActive(true);

        // if (weaponData.modelPrefab != null)
        // {
        //     foreach (Transform child in transform)
        //     {
        //         Destroy(child.gameObject);
        //     }
        //     Instantiate(weaponData.visual, transform);
        //     transform.name = weaponData.modelPrefab.name;
        // }
    }

    void Update()
    {
        // Di chuyển theo hướng đã set
        //transform.position += _direction * speed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= maxLifeTime)
        {
            Deactivate();
        }

        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = direction * weaponData.speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            Deactivate(); // Tắt projectile (pooling)
            if (other.CompareTag(Params.PlayerTag))
            {
                //other.gameObject.SetActive(false);
                SpawnEnemy.Instance.canSpawn = false;
                owner.GetCoinSystem.AddCoin(5);
                SFXManager.Instance.DeadSFX();


            }
            else if (other.CompareTag(Params.BotTag))
            {
                EnemyAI enemyAI = other.GetComponent<EnemyAI>();
                if (enemyAI != null)
                    enemyAI.Die();
                owner.GetCoinSystem.AddCoin(5);
                SFXManager.Instance.DeadSFX();
            }
        }
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}