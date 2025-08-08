using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float wanderChangeDirTime = 2.5f;

    [SerializeField] private WeaponAttack weaponAttack;
    [SerializeField] private AnimationController animationController;

    private Vector3 wanderDir;
    private float moveTimer = 0f;
    private float wanderTimer;
    private EnemyState state = EnemyState.Run;

    public event Action OnDie;

    void Start()
    {
        ChooseRandomDirection();

        if (animationController == null)
            animationController = GetComponentInChildren<AnimationController>();
    }

    void Update()
    {
        if (state == EnemyState.Dead) return;

        // 1. Tìm mục tiêu gần nhất trong bán kính tấn công
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            weaponAttack.GetAttackRadius(),
            weaponAttack.GetTargetLayer()
        );

        Transform closestTarget = null;
        float minDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue; // bỏ qua bản thân
            if (!hit.gameObject.activeSelf) continue;   // bỏ qua đối tượng đã disable

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestTarget = hit.transform;
            }
        }

        if (closestTarget != null && weaponAttack.GetAttackCooldown() <= 0f)
        {
            // Attack
            state = EnemyState.Attack;
            weaponAttack.SetCanAttack(true);
            moveTimer = 0f;
        }
        else
        {
            if (animationController.IsPlayingUnStopAnimation || animationController.IsPlayingSpecialAnimation)
                return;

            // Wander
            state = EnemyState.Idle;
            weaponAttack.SetCanAttack(false);

            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                ChooseRandomDirection();
            }

            if (moveTimer > 0)
            {
                state = EnemyState.Run;
                moveTimer -= Time.deltaTime;
                transform.Translate(wanderDir * moveSpeed * Time.deltaTime, Space.World);
                transform.forward = wanderDir;
                animationController.SetRunAnimation();
            }
            else
            {
                moveTimer = 0f;
                animationController.SetIdleAnimation();
            }
        }
    }

    private void ChooseRandomDirection()
    {
        moveTimer = Random.Range(2f, 4f);
        float angle = Random.Range(0f, 360f);
        wanderDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
        wanderTimer = wanderChangeDirTime + Random.Range(-0.5f, 0.5f);
    }

    public void Die()
    {
        state = EnemyState.Dead;
        OnDie?.Invoke();
        // Ẩn enemy
        gameObject.SetActive(false);
    }

    public void ResetAI()
    {
        state = EnemyState.Run;

        if (animationController != null)
            animationController.Reset();

        moveTimer = 0f;
        wanderTimer = 0f;
    }

    public bool HasListeners()
    {
        return OnDie != null;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag(Params.WallTag) || other.gameObject.CompareTag(Params.BotTag))
        {
            Debug.Log("Enemy hit a wall, changing direction");
            ChooseRandomDirection();
        }
    }
}
