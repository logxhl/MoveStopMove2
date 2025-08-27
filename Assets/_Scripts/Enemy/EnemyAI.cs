//using System;
//using TMPro;
//using UnityEngine;
//using Random = UnityEngine.Random;
//using UnityEngine.UI;

//public class EnemyAI : MonoBehaviour, IGiftReceiver
//{
//    public static EnemyAI instance;
//    [SerializeField] private float moveSpeed = 5f;
//    [SerializeField] private float wanderChangeDirTime = 2.5f;

//    [SerializeField] private WeaponAttack weaponAttack;
//    [SerializeField] private AnimationController animationController;

//    private Vector3 wanderDir;
//    private float moveTimer = 0f;
//    private float wanderTimer;
//    private EnemyState state = EnemyState.Run;

//    public event Action OnDie;

//    private bool isPlayerAlive = true;
//    private bool isGift = false;

//    private int coin = 0;
//    public TextMeshProUGUI coinText;
//    public WeaponProjectile weaponProjectile;
//    private Vector3 defaultThrow;

//    // Gift system variables (tương tự PlayerController)
//    private bool originalWeaponRotateState = false; // Lưu trạng thái xoay ban đầu của vũ khí

//    private void Awake()
//    {
//        //if (instance != null && instance != this)
//        //{
//        //    Destroy(gameObject);
//        //    return;
//        //}
//        instance = this;
//    }

//    void Start()
//    {
//        if (weaponAttack != null && weaponAttack.GetThrowOrigin() != null)
//        {
//            defaultThrow = weaponAttack.GetThrowOrigin().position;
//        }

//        ChooseRandomDirection();

//        if (animationController == null)
//            animationController = GetComponentInChildren<AnimationController>();

//        // Lưu trạng thái xoay ban đầu của vũ khí
//        SaveOriginalWeaponRotateState();
//    }

//    // Lưu trạng thái xoay ban đầu của vũ khí (giống PlayerController)
//    private void SaveOriginalWeaponRotateState()
//    {
//        //// Lấy trạng thái xoay thực tế từ weaponProjectile hiện tại
//        //if (weaponProjectile != null)
//        //{
//        //    //originalWeaponRotateState = weaponProjectile.checkRotate;
//        //    originalWeaponRotateState = true;
//        //    Debug.Log($"🔧 Enemy {gameObject.name} saved original weapon rotate state: {originalWeaponRotateState}");
//        //}
//        //else
//        //{
//        //    // Fallback nếu không có weaponProjectile
//        //    originalWeaponRotateState = true; // Mặc định weapon xoay
//        //    Debug.LogWarning($"⚠️ Enemy {gameObject.name} no weaponProjectile found, using default rotate: true");
//        //}
//        originalWeaponRotateState = true;
//    }

//    void Update()
//    {
//        if (state == EnemyState.Dead) return;

//        // Cập nhật weapon rotation dựa trên gift status (giống PlayerController)
//        UpdateWeaponRotation();

//        if (GameController.instance != null && !GameController.instance.IsPlayerAlive)
//        {
//            state = EnemyState.Idle;
//            weaponAttack.SetCanAttack(false);
//            animationController.SetIdleAnimation();
//            return;
//        }

//        Collider[] hits = Physics.OverlapSphere(
//            transform.position,
//            weaponAttack.GetAttackRadius(),
//            weaponAttack.GetTargetLayer()
//        );

//        Transform closestTarget = null;
//        float minDist = float.MaxValue;

//        foreach (Collider hit in hits)
//        {
//            if (hit.gameObject == gameObject) continue;
//            if (!hit.gameObject.activeSelf) continue;   // bỏ qua đối tượng đã disable

//            float dist = Vector3.Distance(transform.position, hit.transform.position);
//            if (dist < minDist)
//            {
//                minDist = dist;
//                closestTarget = hit.transform;
//            }
//        }

//        if (closestTarget != null && weaponAttack.GetAttackCooldown() <= 0f)
//        {
//            // Attack
//            state = EnemyState.Attack;
//            weaponAttack.SetCanAttack(true);
//            moveTimer = 0f;
//        }
//        else
//        {
//            if (animationController.IsPlayingUnStopAnimation || animationController.IsPlayingSpecialAnimation)
//                return;

//            // Wander
//            state = EnemyState.Idle;
//            weaponAttack.SetCanAttack(false);

//            wanderTimer -= Time.deltaTime;
//            if (wanderTimer <= 0)
//            {
//                ChooseRandomDirection();
//            }

//            if (moveTimer > 0)
//            {
//                state = EnemyState.Run;
//                moveTimer -= Time.deltaTime;
//                transform.Translate(wanderDir * moveSpeed * Time.deltaTime, Space.World);
//                transform.forward = wanderDir;
//                animationController.SetRunAnimation();
//            }
//            else
//            {
//                moveTimer = 0f;
//                animationController.SetIdleAnimation();
//            }
//        }
//    }

//    // Cập nhật weapon rotation dựa trên gift status (giống PlayerController)
//    private void UpdateWeaponRotation()
//    {
//        if (weaponProjectile != null)
//        {
//            // Nếu đang có Gift thì luôn bay thẳng, nếu không thì theo cấu hình ban đầu
//            if (isGift)
//            {
//                Debug.Log("Thang");
//                weaponProjectile.checkRotate = false; // Gift mode: luôn bay thẳng
//            }
//            else
//            {
//                Debug.Log("Xoay");
//                weaponProjectile.checkRotate = originalWeaponRotateState; // Normal mode: theo cấu hình ban đầu
//            }
//        }
//    }

//    private void ChooseRandomDirection()
//    {
//        moveTimer = Random.Range(2f, 4f);
//        float angleDeg = Random.Range(0f, 360f);
//        float angleRad = angleDeg * Mathf.Deg2Rad;
//        wanderDir = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad)).normalized;
//        wanderTimer = wanderChangeDirTime + Random.Range(-0.5f, 0.5f);
//    }

//    public void Die()
//    {
//        state = EnemyState.Dead;
//        animationController.SetDeadAnimation();

//        DisablePhysics();

//        OnDie?.Invoke();
//        SpawnEnemy.Instance?.NotifyCharacterDied(false);
//        EnemyIndicatorManager.instance.UnregisterEnemy(transform);
//        Invoke(nameof(SetDeactiveGameObj), 2f);
//    }

//    void SetDeactiveGameObj() => gameObject.SetActive(false);

//    public void ResetAI()
//    {
//        state = EnemyState.Run;

//        if (animationController != null)
//            animationController.Reset();

//        moveTimer = 0f;
//        wanderTimer = 0f;
//    }

//    public bool HasListeners()
//    {
//        return OnDie != null;
//    }

//    private void OnCollisionStay(Collision other)
//    {
//        if (other.gameObject.CompareTag(Params.WallTag) || other.gameObject.CompareTag(Params.BotTag))
//        {
//            ChooseRandomDirection();
//        }
//    }

//    public void AddCoin(int amount)
//    {
//        coin += amount;
//        coinText.text = coin.ToString();
//    }

//    public int GetCoin()
//    {
//        return coin;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Hammer"))
//        {
//            DisablePhysics();
//            if (SpawnEnemy.Instance.GetRemainingCount() == 1)
//            {
//                int coinPlayer = PlayerController.instance.GetCoin() + 100;
//                GameController.instance.SaveCoin(coinPlayer);
//            }
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag(Params.GiftTag))
//        {
//            ActivateGift();
//        }
//    }

//    // Implement IGiftReceiver interface
//    public bool HasGift()
//    {
//        return isGift;
//    }

//    public Vector3 GetGiftProjectileScale()
//    {
//        return new Vector3(100, 100, 100); // Scale lớn hơn cho Enemy khi có Gift
//    }

//    public Vector3 GetNormalProjectileScale()
//    {
//        return new Vector3(20, 20, 20);
//    }

//    public void ActivateGift()
//    {
//        if (!isGift)
//        {
//            isGift = true;
//            Debug.Log($"🎁 Enemy {gameObject.name} Gift activated! Weapon now flies straight.");

//            // Tăng sức mạnh tấn công
//            weaponAttack.PushThrowOrigin(1f);
//            weaponAttack.SetAttackRadius(7f);

//            // Chuyển vũ khí sang chế độ bay thẳng khi nhận Gift
//            UpdateWeaponRotation();

//            // Scale up projectile sẽ được xử lý trong WeaponProjectile.Launch()
//        }
//    }

//    public void DeactivateGift()
//    {
//        if (isGift)
//        {
//            isGift = false;
//            Debug.Log($"🔄 Enemy {gameObject.name} Gift ended. Weapon behavior reset to original.");

//            // Reset lại sức mạnh tấn công
//            weaponAttack.ResetThrowOrigin();
//            weaponAttack.SetAttackRadius(5f);

//            // Khôi phục trạng thái xoay ban đầu của vũ khí
//            UpdateWeaponRotation();

//            // Scale projectile sẽ được xử lý trong WeaponProjectile.Launch()
//        }
//    }

//    // Method cũ SetDefault() được thay thế bằng DeactivateGift()
//    public void SetDefault()
//    {
//        DeactivateGift();
//    }

//    private void DisablePhysics()
//    {
//        // Tắt tất cả Collider
//        Collider[] cols = GetComponentsInChildren<Collider>();
//        foreach (Collider col in cols)
//        {
//            col.enabled = false;
//        }

//        // Tắt tất cả Rigidbody
//        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
//        foreach (Rigidbody rb in rbs)
//        {
//            rb.isKinematic = true;   // ngừng tính toán vật lý
//            rb.detectCollisions = false; // ngừng va chạm
//        }
//    }
//}
using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IGiftReceiver
{
    public static EnemyAI instance;
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
    private bool isGift = false;

    private int coin = 0;
    public TextMeshProUGUI coinText;
    public WeaponProjectile weaponProjectile;
    private Vector3 defaultThrow;

    // Gift system variables - chỉ cần lưu trạng thái ban đầu
    private bool originalWeaponRotateState = true; // Mặc định là true (xoay)

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (weaponAttack != null && weaponAttack.GetThrowOrigin() != null)
        {
            defaultThrow = weaponAttack.GetThrowOrigin().position;
        }

        ChooseRandomDirection();

        if (animationController == null)
            animationController = GetComponentInChildren<AnimationController>();

        // Khởi tạo weapon với trạng thái xoay ban đầu
        if (weaponProjectile != null)
        {
            weaponProjectile.checkRotate = originalWeaponRotateState;
        }
    }

    void Update()
    {
        if (state == EnemyState.Dead) return;

        // ✅ BỎ DÒNG NÀY - không cần gọi UpdateWeaponRotation() mỗi frame
        // UpdateWeaponRotation();

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
            if (!hit.gameObject.activeSelf) continue;

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

    // ✅ BỎ HÀM NÀY - không cần thiết nữa
    // private void UpdateWeaponRotation() { ... }

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
        DisablePhysics();
        OnDie?.Invoke();
        SpawnEnemy.Instance?.NotifyCharacterDied(false);
        EnemyIndicatorManager.instance.UnregisterEnemy(transform);
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
        if (other.CompareTag("Hammer"))
        {
            DisablePhysics();
            if (SpawnEnemy.Instance.GetRemainingCount() == 1)
            {
                int coinPlayer = PlayerController.instance.GetCoin() + 100;
                GameController.instance.SaveCoin(coinPlayer);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Params.GiftTag))
        {
            ActivateGift();
        }
    }

    // IGiftReceiver interface
    public bool HasGift()
    {
        return isGift;
    }

    public Vector3 GetGiftProjectileScale()
    {
        return new Vector3(100, 100, 100);
    }

    public Vector3 GetNormalProjectileScale()
    {
        return new Vector3(20, 20, 20);
    }

    public void ActivateGift()
    {
        if (!isGift)
        {
            isGift = true;
            Debug.Log($"🎁 Enemy {gameObject.name} Gift activated! Weapon now flies straight.");

            // Tăng sức mạnh tấn công
            weaponAttack.PushThrowOrigin(1f);
            weaponAttack.SetAttackRadius(7f);

            // ✅ THAY ĐỔI TRỰC TIẾP - Weapon bay thẳng khi có gift
            if (weaponProjectile != null)
            {
                weaponProjectile.checkRotate = false; // Gift mode: bay thẳng
                Debug.Log("🎯 Weapon set to fly straight (checkRotate = false)");
            }
        }
    }

    public void DeactivateGift()
    {
        if (isGift)
        {
            isGift = false;
            Debug.Log($"🔄 Enemy {gameObject.name} Gift ended. Weapon behavior reset to original.");

            // Reset lại sức mạnh tấn công
            weaponAttack.ResetThrowOrigin();
            weaponAttack.SetAttackRadius(5f);

            // ✅ KHÔI PHỤC TRẠNG THÁI BAN ĐẦU - Weapon xoay lại như cũ
            if (weaponProjectile != null)
            {
                weaponProjectile.checkRotate = originalWeaponRotateState; // Về trạng thái ban đầu (true)
                Debug.Log($"🔄 Weapon restored to original state (checkRotate = {originalWeaponRotateState})");
            }
        }
    }

    // SetDefault() gọi DeactivateGift()
    public void SetDefault()
    {
        DeactivateGift();
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
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }
}