using System;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    private WeaponData weaponData;

    private LayerMask targetLayer;

    [SerializeField] private float maxLifeTime = 0.6f;
    private Vector3 direction;
    private float timer;
    [SerializeField] private float rotateSpeed;

    private Rigidbody _rigidbody;

    //public WeaponAttack owner;
    private GameObject owner;
    public bool checkRotate;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    //public void Launch(Vector3 direction, LayerMask targetLayer, WeaponData weaponData, WeaponAttack owner)
    //{
    //    this.direction = direction.normalized;
    //    this.targetLayer = targetLayer;
    //    this.weaponData = weaponData;
    //    this.owner = owner;
    //    this.timer = 0f;
    //    gameObject.SetActive(true);
    //}
    public void Launch(Vector3 direction, LayerMask targetLayer, WeaponData weaponData, GameObject owner)
    {
        this.direction = direction.normalized;
        this.targetLayer = targetLayer;
        this.weaponData = weaponData;
        this.owner = owner;
        this.timer = 0f;
        if(!checkRotate && this.direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(this.direction);
        }
        gameObject.SetActive(true);
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
        if (checkRotate)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion offset = Quaternion.Euler(90f, 180f, 0f);
            transform.rotation = Quaternion.LookRotation(direction) * offset;
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = direction * weaponData.speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            Deactivate();
            if (other.CompareTag(Params.PlayerTag))
            {
                EnemyAI actor = owner.GetComponent<EnemyAI>();
                //other.gameObject.SetActive(false);
                SpawnEnemy.Instance.canSpawn = false;
                actor.AddCoin(5);

                SFXManager.Instance.DeadSFX();


            }
            //else if (other.CompareTag(Params.BotTag))
            //{

            //    PlayerController actor = owner.GetComponent<PlayerController>();
            //    EnemyAI actorEnemy = owner.GetComponent<EnemyAI>();
            //    EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            //    if (actorEnemy != null)
            //    {
            //        if (enemyAI != null)
            //            enemyAI.Die();
            //        actorEnemy.AddCoin(5);
            //        Debug.Log(actor.coin.ToString());
            //        SFXManager.Instance.DeadSFX();
            //    }
            //    else if(actor != null)
            //    {
            //        if (enemyAI != null)
            //            enemyAI.Die();
            //        actor.AddCoin(5);
            //        Debug.Log(actor.coin.ToString());
            //        SFXManager.Instance.DeadSFX();
            //    }
            //}
            else if (other.CompareTag(Params.BotTag))
            {
                // Lấy AI của mục tiêu bị bắn
                EnemyAI targetEnemy = other.GetComponent<EnemyAI>();

                // Lấy thông tin chủ bắn
                PlayerController playerShooter = owner.GetComponent<PlayerController>();
                EnemyAI enemyShooter = owner.GetComponent<EnemyAI>();

                if (targetEnemy != null)
                {
                    // Giết mục tiêu
                    targetEnemy.Die();

                    // Nếu chủ bắn là Player
                    if (playerShooter != null)
                    {
                        playerShooter.AddCoin(5);
                        Debug.Log($"Player coin: {playerShooter.GetCoin()}");
                    }
                    // Nếu chủ bắn là Enemy
                    else if (enemyShooter != null)
                    {
                        enemyShooter.AddCoin(5);
                        Debug.Log($"Enemy coin: {enemyShooter.GetCoin()}");
                    }

                    SFXManager.Instance.DeadSFX();
                }
            }

        }
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}