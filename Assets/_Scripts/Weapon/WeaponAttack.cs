using System;
using System.Collections;
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
    private Vector3 defaultLocalPos;
    private EnemyHighLight lastTargetHighLight;


    private bool canAttack = false;
    private bool isDead = false;

    //Bat tat skill
    private bool specialDoubleThrow = false;
    private bool specialTripleSpread = false;
    private void Awake()
    {
        specialDoubleThrow = false;
        specialTripleSpread = false;
        if (throwOrigin != null)
        {
            // Lưu lại vị trí local ban đầu của throwOrigin (trước mặt Player)
            defaultLocalPos = throwOrigin.localPosition;
        }

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
        if (!isDead && canAttack && attackCooldown <= 0f)
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
    public void EnableDoubleThrow(bool active)
    {
        specialDoubleThrow = active;
    }
    public void EnableTripleSpread(bool active)
    {
        specialTripleSpread = active;
    }
    public void SetDead(bool dead)
    {
        isDead = dead;
        if (dead)
        {
            canAttack = false;
            if (lastTargetHighLight != null)
            {
                lastTargetHighLight.ShowCircle(false);
                lastTargetHighLight = null;
            }
        }
    }

    public void SetCanAttack(bool can)
    {
        canAttack = can;
    }

    public void FireProjectile(Collider collider)
    {
        if (weaponHandVisual != null)
            weaponHandVisual.SetActive(false);

        Vector3 direction = new Vector3(collider.transform.position.x, 0, collider.transform.position.z) - new Vector3(throwOrigin.position.x, 0, throwOrigin.position.z);
        Vector3 dir = direction.normalized;
        if (specialTripleSpread)
        {
            FireTripleSpread(dir);
        }
        else
        {
            //Nem vu khi thu nhat
            ThrowOneProjectile(collider, dir, Vector3.zero);
            SFXManager.Instance.PlayAttack();
            if (specialDoubleThrow)
            {
                StartCoroutine(ThrowSecondProjectileAfterDelay(collider, dir, 0.1f));
            }
        }
        StartCoroutine(ShowWeaponInHandAfterDelay(1f));
    }
    private IEnumerator ShowWeaponInHandAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(weaponHandVisual != null)
        {
            weaponHandVisual.SetActive(true);
        }
    }

    private void ThrowOneProjectile(Collider collider, Vector3 dir, Vector3 offset)
    {
        var projectile = Instantiate(
            currentWeapon.modelPrefab.GetComponent<WeaponProjectile>(),
            throwOrigin.position + offset,
            currentWeapon.modelPrefab.transform.rotation,
            weaponInstantiateTransform
        );
        projectile.Launch(dir, targetLayer, currentWeapon, playerTransform.gameObject);
    }
    private void FireTripleSpread(Vector3 dir)
    {
        float spreadAngle = 15f; //Goc lech moi vien;
        //Vien thang giua
        ThrowOneProjectile(null, dir, Vector3.zero);
        //Tinh huong lech trai
        Vector3 leftDir = Quaternion.Euler(0, -spreadAngle, 0) * dir;
        ThrowOneProjectile(null, leftDir, Vector3.zero);
        //Tinh huong lech phai
        Vector3 rightDir = Quaternion.Euler(0, spreadAngle, 0) * dir;
        ThrowOneProjectile(null, rightDir, Vector3.zero);
        SFXManager.Instance.PlayAttack();
    }
    private IEnumerator ThrowSecondProjectileAfterDelay(Collider collider, Vector3 dir, float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 offset = new Vector3(0.3f, 0, 0);
        ThrowOneProjectile(collider, dir, offset);
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
    public void SetAttackRadius(float radius)
    {
        attackRadius = radius;
    }
    public LayerMask GetTargetLayer()
    {
        return targetLayer;
    }
    public void SetThrowOrigin(Vector3 throwOr)
    {
        throwOrigin.transform.position = throwOr;
    }
    public Transform GetThrowOrigin()
    {
        return throwOrigin;
    }

    // Đẩy throwOrigin ra xa hơn
    public void PushThrowOrigin(float distance)
    {
        throwOrigin.localPosition = defaultLocalPos + new Vector3(0, 0, distance);
    }

    // Reset về mặc định (ngay trước mặt Player)
    public void ResetThrowOrigin()
    {
        throwOrigin.localPosition = defaultLocalPos;
    }
    public void SetSpecialDoubleThrow(bool active)
    {
        specialDoubleThrow = active;
    }

}