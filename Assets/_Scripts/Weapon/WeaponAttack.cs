using System;
using TMPro;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private GameObject weaponHandVisual;
    [SerializeField] private Transform handHoldWeaponTransform;

    [SerializeField] private float maxAttackCooldown = 5f;
    private float attackCooldown = 0f;
    [SerializeField] private float attackRadius = 7f;


    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private AnimationController animationController;

    [SerializeField] private Transform weaponInstantiateTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform throwOrigin;
    private EnemyHighLight lastTargetHighLight;


    private bool canAttack = false;

    //[SerializeField] private CoinSystem coinSystem;
    //public CoinSystem GetCoinSystem => coinSystem;

    //[SerializeField] private TextMeshProUGUI coinText;

    private void Awake()
    {
        attackCooldown = 0f;
        canAttack = false;
        //weaponInstantiateTransform = GameObject.Find("PoolManager").transform.GetChild(0).transform;
        GameObject poolManagerObj = GameObject.Find("PoolManager");
        if (poolManagerObj != null && poolManagerObj.transform.childCount > 0)
        {
            weaponInstantiateTransform = poolManagerObj.transform.GetChild(0);
        }
        else
        {
            Debug.LogWarning("PoolManager không có child!");
        }
    }

    void Update()
    {
        //if(coinText != null && coinSystem != null && gameObject.CompareTag("Player"))
        //{
        //    coinText.text = coinSystem.GetCoin().ToString();
        //} 

        //if (canAttack && attackCooldown <= 0f)
        //{
        //    Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, targetLayer);
        //    foreach (Collider hit in hits)
        //    {
        //        if (hit.gameObject == playerTransform.gameObject) continue; 
        //        if (!hit.gameObject.activeSelf) continue;  


        //        if (hit.CompareTag(Params.BotTag) || hit.CompareTag(Params.PlayerTag))
        //        {
        //            playerTransform.LookAt(hit.transform);

        //            //Debug.Log(hit.name + " bị attack!");

        //            if (animationController != null)
        //            {
        //                animationController.OnAttack += () => FireProjectile(hit);

        //                animationController.SetAttackAnimation();
        //            }

        //            attackCooldown = maxAttackCooldown;
        //            break;
        //        }
        //    }
        //}
        if (canAttack && attackCooldown <= 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, targetLayer);
            if (hits.Length > 0)
            {
                //Tim enemy gan nhat
                Collider neasrest = null;
                float minDist = Mathf.Infinity;
                foreach (Collider hit in hits)
                {
                    if (hit.gameObject == playerTransform.gameObject) continue;
                    if (!hit.gameObject.activeSelf) continue;
                    if (!hit.CompareTag(Params.BotTag) && !hit.CompareTag(Params.PlayerTag)) continue;

                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        neasrest = hit;
                    }
                }

                if (neasrest != null)
                {
                    bool isPlayerAttacking = playerTransform.CompareTag(Params.PlayerTag) && neasrest.CompareTag(Params.BotTag);

                    //Tat vong muc tieu cu neu co
                    if (lastTargetHighLight != null && lastTargetHighLight.gameObject != neasrest.gameObject)
                    {
                        lastTargetHighLight.ShowCircle(false);
                    }
                    //Bat vong muc tieu moi
                    EnemyHighLight targetHighLight = neasrest.GetComponent<EnemyHighLight>();
                    if (targetHighLight != null)
                    {
                        targetHighLight.ShowCircle(isPlayerAttacking);
                        lastTargetHighLight = targetHighLight;
                    }
                                      

                    playerTransform.LookAt(neasrest.transform);
                    if (animationController != null)
                    {
                        animationController.OnAttack += () => FireProjectile(neasrest);
                        animationController.SetAttackAnimation();
                    }
                    attackCooldown = maxAttackCooldown;
                }
            }
            else
            {
                //Khong co enemy nao -> tat highlight
                if (lastTargetHighLight != null)
                {
                    lastTargetHighLight.ShowCircle(false);
                    lastTargetHighLight = null;
                }
            }
        }
        else
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                //weaponHandVisual.SetActive(true);
                attackCooldown = 0f;
            }
        }
    }

    public void SetCanAttack(bool can)
    {
        canAttack = can;
    }

    public void FireProjectile(Collider collider)
    {
        //weaponHandVisual.SetActive(false);

        Vector3 direction = new Vector3(collider.transform.position.x, 0, collider.transform.position.z) - new Vector3(throwOrigin.position.x, 0, throwOrigin.position.z);
        Vector3 dir = direction.normalized;

        var projectile = Instantiate(currentWeapon.modelPrefab.GetComponent<WeaponProjectile>(), throwOrigin.position, currentWeapon.modelPrefab.transform.rotation, weaponInstantiateTransform);
        projectile.Launch(dir, targetLayer, currentWeapon, playerTransform.gameObject);
        SFXManager.Instance.PlayAttack();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public float GetAttackCooldown()
    {
        return attackCooldown;
    }

    public float GetAttackRadius()
    {
        return attackRadius;
    }

    public LayerMask GetTargetLayer()
    {
        return targetLayer;
    }
}