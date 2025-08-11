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
            Deactivate(); 
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