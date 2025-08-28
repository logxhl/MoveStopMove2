using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIOfZombie : MonoBehaviour
{
    public static AIOfZombie instance;
    [Header("References")]
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolTolerance = 0.5f;
    public int patrolIndex = 0;

    [Header("Detection Settings")]
    public float sightRange = 10f;
    public LayerMask detectionLayers;
    public GameObject effectDeadZombie;
    //public GameObject levelUp;
    public bool isBoss;
    public bool isBossEnd;
    private int countAttackIsBoss = 4;
    private int countAttackIsBossEnd = 5;

    private ZombieState state = ZombieState.Walk;
    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        instance = this;
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        //Tu tim patrol point
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            GameObject pointsParent = GameObject.Find("PatrolPoints");
            if (pointsParent != null)
            {
                List<Transform> list = new List<Transform>();
                foreach (Transform t in pointsParent.GetComponentsInChildren<Transform>())
                {
                    if (t != pointsParent.transform) list.Add(t);
                }
                patrolPoints = list.ToArray();
            }
        }
    }
    private void Start()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.Warp(hit.position);
            }
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }
    private void Update()
    {
        if (!player) return;
        switch (state)
        {
            case ZombieState.Walk:
                Patrol();
                //Debug.Log("Walk");
                if (CanSeePlayer()) SwitchState(ZombieState.Run);
                break;
            case ZombieState.Run:
                ChasePlayer();
                //Debug.Log("Run");
                break;
            case ZombieState.Victory:
                agent.ResetPath();
                //Debug.Log("Victory");
                break;
        }

        if (anim)
        {
            anim.SetBool("IsWalk", state == ZombieState.Walk);
            //anim.SetBool("IsRun", state == ZombieState.Run);
            anim.SetBool("IsWin", state == ZombieState.Victory);
        }

    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }

        if (!CanSeePlayer() && patrolPoints.Length > 0)
        {
            SwitchState(ZombieState.Walk);
        }
    }

    public void SwitchState(ZombieState newState)
    {
        state = newState;
    }

    private bool CanSeePlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRange, detectionLayers);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                player = hit.transform;
                return true;
            }
        }
        return false;
    }

    private void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= patrolTolerance)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == ZombieState.Run && other.CompareTag("Player"))
        {
            if (PlayerSceneZombie.instance != null)
            {
                PlayerSceneZombie.instance.OnHitByZombie(this);
            }
        }
        if (other.CompareTag("Hammer"))
        {
            if (isBoss)
            {
                countAttackIsBoss -= 1;
                if (countAttackIsBoss <= 0)
                {
                    DieZombie();
                }
                else
                {
                    //Hit zombie
                }
            }
            else if (isBossEnd)
            {
                countAttackIsBossEnd -= 1;
                if (countAttackIsBossEnd <= 0)
                {
                    DieZombie();
                }
            }
            else
            {
                DieZombie();
            }
        }
    }
    private void DieZombie()
    {
        DisablePhysics();
        SFXManager.Instance.DeadZombieSFX();

        GameObject effect = Instantiate(effectDeadZombie);
        effect.transform.rotation = Quaternion.identity;
        effect.transform.position = new Vector3(transform.position.x, 6f, transform.position.z);
        effect.GetComponent<ParticleSystem>().Play();

        SpawnZombie.instance?.NotifyCharacterDied(false);
        EnemyIndicatorManager.instance.UnregisterEnemy(transform);

        Destroy(gameObject);
        PlayerSceneZombie.instance.AddCoin(5);

        int coin = PlayerSceneZombie.instance.GetCoin();
        if (coin == 10)
        {
            PlayerSceneZombie.instance.levelUp.gameObject.SetActive(true);
            PlayerSceneZombie.instance.ShowLevelUp();
        }
        if (SpawnZombie.instance.GetRemainingCount() == 0)
        {
            int coinPlayer = PlayerSceneZombie.instance.GetCoin() + 100;
            ControllerSceneZombie.instance.SaveCoin(coinPlayer);
        }

        Debug.Log("Coin player: " + PlayerSceneZombie.instance.GetCoin());
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    private void DisablePhysics()
    {
        // Tắt tất cả Collider
        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            col.enabled = false;
        }

        // Tắt tất cả Rigidbody
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;   // ngừng tính toán vật lý
            rb.detectCollisions = false; // ngừng va chạm
        }
    }
}
