using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIOfZombie : MonoBehaviour
{
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
    public float viewAngle = 120f;
    public LayerMask detectionLayers;
    public LayerMask obstructionLayers;

    private ZombieState state = ZombieState.Walk;

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        if(!agent) agent = GetComponent<NavMeshAgent>();
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        //Tu tim patrol point
        if(patrolPoints == null || patrolPoints.Length == 0)
        {
            GameObject pointsParent = GameObject.Find("PatrolPoints");
            if(pointsParent != null)
            {
                List<Transform> list = new List<Transform>();
                foreach(Transform t in pointsParent.GetComponentsInChildren<Transform>())
                {
                    if(t != pointsParent.transform) list.Add(t);
                }
                patrolPoints = list.ToArray();
            }
        }
    }
    private void Start()
    {
        if(patrolPoints != null && patrolPoints.Length > 0)
        {
            if(NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.Warp(hit.position);
            }
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
        //if(patrolPoints.Length > 0)
        //{
        //    agent.SetDestination(patrolPoints[patrolIndex].position);
        //}
    }
    private void Update()
    {
        if (!player) return;
        switch(state)
        {
            case ZombieState.Walk:
                Patrol();
                Debug.Log("Walk");
                if (CanSeePlayer()) SwitchState(ZombieState.Run);
                break;
            case ZombieState.Run:
                ChasePlayer();
                Debug.Log("Run");
                break;
            case ZombieState.Victory:
                agent.ResetPath();
                Debug.Log("Victory");
                break;
        }

        if(anim)
        {
            anim.SetBool("IsWalk", state == ZombieState.Walk);
            anim.SetBool("IsRun", state == ZombieState.Run);
            anim.SetBool("IsWin", state == ZombieState.Victory);
        }

    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        if(!CanSeePlayer())
        {
            if(patrolPoints.Length > 0)
            {
                SwitchState(ZombieState.Walk);
            }
        }
    }

    private void SwitchState(ZombieState newState)
    {
        state = newState;
    }

    private bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        if (dist > sightRange) return false;
        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
        if (angle > viewAngle / 2f) return false;

        if(Physics.Raycast(transform.position + Vector3.up * 1.6f, toPlayer.normalized, out RaycastHit hit, sightRange, detectionLayers | obstructionLayers))
        {
            if (hit.transform == player) return true;
        }
        return false;
    }

    private void Patrol()
    {
        if(!agent.pathPending && agent.remainingDistance <= patrolTolerance)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(state == ZombieState.Run && other.CompareTag("Player"))
        {
            Debug.Log("Player Dead");
            SwitchState(ZombieState.Victory);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
