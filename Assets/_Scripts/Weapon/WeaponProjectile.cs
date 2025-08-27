using System;
using System.Collections;
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
    private GameObject owner;
    public bool checkRotate;

    // Thêm các biến để xử lý ignore collision
    private Collider shooterCollider;
    private Collider projectileCollider;
    [SerializeField] private float ignoreCollisionTime = 0.5f; // Thời gian ignore collision

    [Header("Gift Scaling Settings")]
    [SerializeField] private float scaleUpDuration = 0.3f;
    private Vector3 originalScale;
    private Vector3 normalScale = new Vector3(20, 20, 20);      // Scale bình thường
    private Vector3 giftScale = new Vector3(100, 100, 100);        // Scale khi có gift

    private Coroutine scaleCoroutine;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
    }

    public void Launch(Vector3 direction, LayerMask targetLayer, WeaponData weaponData, GameObject owner)
    {
        this.direction = direction.normalized;
        this.targetLayer = targetLayer;
        this.weaponData = weaponData;
        this.owner = owner;
        this.timer = 0f;

        // Thiết lập ignore collision với người bắn
        SetupIgnoreCollision();

        if (!checkRotate && this.direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(this.direction);
        }
        gameObject.SetActive(true);

        // Kiểm tra xem owner có đang trong trạng thái Gift không
        CheckAndApplyGiftEffect();
    }
    private void CheckAndApplyGiftEffect()
    {
        // Kiểm tra owner có implement IGiftReceiver không
        IGiftReceiver giftReceiver = owner.GetComponent<IGiftReceiver>();

        if (giftReceiver != null && giftReceiver.HasGift())
        {
            // Scale từ bình thường lên Gift size
            StartSmoothScaling(normalScale, giftScale);
            Debug.Log($"🎁 {owner.name} has Gift! Projectile scaling up!");
        }
        else
        {
            // Scale bình thường
            transform.localScale = normalScale;
        }
    }

    //// Hàm check xem player có gift không (cần access đến biến private của PlayerController)
    //private bool IsPlayerHasGift(PlayerController player)
    //{
    //    // Có thể dùng reflection hoặc tạo public method trong PlayerController
    //    // Ở đây tôi sẽ suggest tạo method public trong PlayerController
    //    return player.HasGift(); // Method này cần được thêm vào PlayerController
    //}

    private void StartSmoothScaling(Vector3 fromScale, Vector3 toScale)
    {
        // Stop coroutine cũ nếu có
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        // Set scale ban đầu
        transform.localScale = fromScale;

        // Start scaling
        scaleCoroutine = StartCoroutine(SmoothScaleCoroutine(fromScale, toScale));
    }
    private IEnumerator SmoothScaleCoroutine(Vector3 fromScale, Vector3 toScale)
    {
        float elapsed = 0f;

        while (elapsed < scaleUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleUpDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            transform.localScale = Vector3.Lerp(fromScale, toScale, smoothT);

            yield return null;
        }

        // Đảm bảo scale chính xác
        transform.localScale = toScale;
        scaleCoroutine = null;
    }
   
    private void SetupIgnoreCollision()
    {
        if (owner != null && projectileCollider != null)
        {
            // Lấy collider của người bắn
            shooterCollider = owner.GetComponent<Collider>();

            if (shooterCollider != null)
            {
                // Bỏ qua va chạm với người bắn
                Physics.IgnoreCollision(projectileCollider, shooterCollider, true);
                //Debug.Log($"🚀 Projectile ignoring collision with shooter: {owner.name}");

                // Sau một khoảng thời gian thì cho phép va chạm trở lại
                StartCoroutine(EnableCollisionAfterDelay());
            }
            else
            {
                Debug.LogWarning($"⚠️ Shooter {owner.name} không có Collider!");
            }
        }
    }

    private IEnumerator EnableCollisionAfterDelay()
    {
        yield return new WaitForSeconds(ignoreCollisionTime);

        // Kiểm tra lại để đảm bảo các object vẫn tồn tại
        if (shooterCollider != null && projectileCollider != null && gameObject.activeInHierarchy)
        {
            Physics.IgnoreCollision(projectileCollider, shooterCollider, false);
            //Debug.Log($"✅ Projectile can now collide with shooter: {owner.name}");
        }
    }

    void Update()
    {
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
        // Kiểm tra xem có phải target layer không
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // Debug log để theo dõi va chạm
            //Debug.Log($"💥 Projectile hit: {other.name}, Owner: {owner?.name}");

            Deactivate();

            if (other.CompareTag(Params.PlayerTag))
            {
                EnemyAI actor = owner.GetComponent<EnemyAI>();
                SpawnEnemy.Instance.canSpawn = false;
                actor.AddCoin(5);
                SFXManager.Instance.DeadSFX();
            }
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
        // Reset ignore collision trước khi deactivate
        if (shooterCollider != null && projectileCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, shooterCollider, false);
        }

        // Stop tất cả coroutines
        StopAllCoroutines();

        gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        // Đảm bảo reset collision khi object bị disable
        if (shooterCollider != null && projectileCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, shooterCollider, false);
        }
    }
}