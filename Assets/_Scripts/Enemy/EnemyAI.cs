using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

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

    private bool isPlayerAlive = true;

    //[SerializeField] private CoinSystem coinSystem;
    //public CoinSystem GetCoinSystem => coinSystem;
    private int coin = 0;
    public TextMeshProUGUI coinText;

    void Start()
    {
        ChooseRandomDirection();

        if (animationController == null)
            animationController = GetComponentInChildren<AnimationController>();
    }

    void Update()
    {
        if (state == EnemyState.Dead) return;
        if (GameController.instance != null && !GameController.instance.IsPlayerAlive)
        {
            state = EnemyState.Idle;
            weaponAttack.SetCanAttack(false);
            animationController.SetIdleAnimation();
            return;
        }

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            weaponAttack.GetAttackRadius(),
            weaponAttack.GetTargetLayer()
        );

        Transform closestTarget = null;
        float minDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
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
        float angleDeg = Random.Range(0f, 360f);
        float angleRad = angleDeg * Mathf.Deg2Rad;
        wanderDir = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad)).normalized;
        wanderTimer = wanderChangeDirTime + Random.Range(-0.5f, 0.5f);
    }

    public void Die()
    {
        state = EnemyState.Dead;
        animationController.SetDeadAnimation();

        OnDie?.Invoke();
        SpawnEnemy.Instance?.NotifyCharacterDied(false);
        //gameObject.SetActive(false);
        Invoke(nameof(SetDeactiveGameObj), 2f);
    }
    void SetDeactiveGameObj() => gameObject.SetActive(false);
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
            ChooseRandomDirection();
        }
    }
    public void AddCoin(int amount)
    {
        coin += amount;
        coinText.text = coin.ToString();
    }
    public int GetCoin()
    {
        return coin;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hammer"))
        {
            if (SpawnEnemy.Instance.GetRemainingCount() == 1)
            {
                int coinPlayer = PlayerController.instance.GetCoin() + 100;
                GameController.instance.SaveCoin(coinPlayer);
            }
        }
    }
}
