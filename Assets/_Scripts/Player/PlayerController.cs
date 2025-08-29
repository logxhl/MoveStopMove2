using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IGiftReceiver
{
    public static PlayerController instance;
    [SerializeField] private AnimationController animationController;
    [SerializeField] private JoystickController joystick; // Kéo JoystickBG vào đây
    [SerializeField] private WeaponAttack weaponAttack;
    [SerializeField] private GameObject fixedJoyStick;
    [SerializeField] private GameObject topUI;
    [SerializeField] private GameObject panelCountDown;
    [SerializeField] private TextMeshProUGUI textRank;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI coinDeadScene;
    [SerializeField] private ParticleSystem particleUpScale;
    [SerializeField] private WeaponTableObject weaponData;

    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rigid;

    private PlayerState playerState;

    private Vector2 dir;
    private Vector3 move;
    [Header("SetWeapon")]
    public ListWeapon listWeapon;
    public GameObject currentWeapon;
    [Header("SetPant")]
    public PantTableObject listPant;
    public GameObject currentPant;

    public WeaponProjectile projectile;

    // Biến lưu trạng thái xoay ban đầu của vũ khí
    private bool originalWeaponRotateState;

    private int coin = 0;
    public int countUpCoin = 0;
    private bool isGift = false;

    //Obstacle
    public float checkRadius = 2f;
    public LayerMask checkLayer;   // layer chứa obstacle
    public Material transparentMat;
    private Dictionary<GameObject, Material> originalMats = new Dictionary<GameObject, Material>();

    public bool isDeadPlayer = false;
    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        rigid = GetComponent<Rigidbody>();
        if (animationController == null)
            animationController = GetComponentInChildren<AnimationController>();

        // Lưu trạng thái xoay ban đầu
        SaveOriginalWeaponRotateState();
    }

    void Update()
    {
        CheckObstacle();

        ChangeWeapon();
        dir = joystick.inputDir; // Lấy hướng từ joystick
        move = new Vector3(dir.x, 0, dir.y);

        if (move != Vector3.zero)
            if (!animationController.IsPlayingUnStopAnimation)
                animationController.OnSpecialAnimationEnd();

        if (!animationController.IsPlayingSpecialAnimation && !animationController.IsPlayingUnStopAnimation)
        {
            transform.Translate(move.normalized * moveSpeed * Time.deltaTime, Space.World);

            if (move != Vector3.zero)
            {
                playerState = PlayerState.Run;
                transform.forward = move.normalized;
                animationController.SetRunAnimation();
                weaponAttack.SetCanAttack(false);
            }
            else
            {
                playerState = PlayerState.Idle;
                animationController.SetIdleAnimation();
                weaponAttack.SetCanAttack(true);
            }
        }
    }

    private void CheckObstacle()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius, checkLayer);

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Obstacle"))
            {
                Renderer rend = col.GetComponent<Renderer>();
                if (rend != null)
                {
                    // Lưu material gốc để sau có thể khôi phục
                    if (!originalMats.ContainsKey(col.gameObject))
                    {
                        originalMats[col.gameObject] = rend.material;
                        rend.material = transparentMat;  // đổi sang transparent
                    }
                }
            }
        }

        // Khôi phục material cho những object không còn trong vùng Overlap nữa
        List<GameObject> toRestore = new List<GameObject>();
        foreach (var kvp in originalMats)
        {
            bool stillInside = false;
            foreach (Collider col in hitColliders)
            {
                if (col.gameObject == kvp.Key)
                {
                    stillInside = true;
                    break;
                }
            }

            if (!stillInside)
            {
                Renderer rend = kvp.Key.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = kvp.Value; // restore lại material gốc
                }
                toRestore.Add(kvp.Key);
            }
        }

        // Xóa những object đã restore
        foreach (var obj in toRestore)
        {
            originalMats.Remove(obj);
        }
    }

    // Lưu trạng thái xoay ban đầu của vũ khí hiện tại
    private void SaveOriginalWeaponRotateState()
    {
        int indexWp = PlayerPrefs.GetInt("LoadWeapon", 0);

        for (int i = 0; i < listWeapon.weaponList.Length; i++)
        {
            if (indexWp == listWeapon.weaponList[i].index)
            {
                originalWeaponRotateState = listWeapon.weaponList[i].isRotate;
                break;
            }
        }
    }

    private void ChangeWeapon()
    {
        int indexWp = PlayerPrefs.GetInt("LoadWeapon", 0);
        int indMaterial = PlayerPrefs.GetInt("MaterialOfWp" + indexWp, 0);

        // Kiểm tra indexWp có hợp lệ không
        if (indexWp < 0 || indexWp >= weaponData.listMaterials.Length)
        {
            Debug.LogError($"❌ indexWp={indexWp} vượt quá giới hạn weaponData.listMaterials.Length={weaponData.listMaterials.Length}");
            return;
        }

        if (indMaterial < 0 || indMaterial >= weaponData.listMaterials[indexWp].materialOfHammer.Length)
        {
            Debug.LogError($"❌ indMaterial={indMaterial} vượt quá giới hạn materialOfHammer.Length={weaponData.listMaterials[indexWp].materialOfHammer.Length}");
            return;
        }

        MeshRenderer meshRenderer = currentWeapon.GetComponent<MeshRenderer>();
        MeshRenderer meshProjectile = projectile.GetComponent<MeshRenderer>();

        Material[] mats = meshRenderer.materials;
        Material[] matsOfBullet = meshProjectile.sharedMaterials;

        for (int i = 0; i < listWeapon.weaponList.Length; i++)
        {
            if (indexWp == listWeapon.weaponList[i].index)
            {
                // Set mesh
                currentWeapon.GetComponent<MeshFilter>().mesh = listWeapon.weaponList[i].meshWepon;
                projectile.GetComponent<MeshFilter>().mesh = listWeapon.weaponList[i].meshWepon;

                // Lấy số lượng material hợp lệ
                Material[] srcMats = weaponData.listMaterials[indexWp].materialOfHammer[indMaterial].materials;

                // Số lượng material hợp lệ nhỏ nhất
                int matCount = Mathf.Min(mats.Length, matsOfBullet.Length, srcMats.Length);

                for (int j = 0; j < matCount; j++)
                {
                    mats[j] = srcMats[j];
                    matsOfBullet[j] = srcMats[j];
                }

                meshRenderer.materials = mats;
                meshProjectile.materials = matsOfBullet;

                // Lưu trạng thái xoay ban đầu
                originalWeaponRotateState = listWeapon.weaponList[i].isRotate;

                // Set rotate - Nếu đang có Gift thì luôn bay thẳng, nếu không thì theo cấu hình ban đầu
                if (isGift)
                {
                    projectile.checkRotate = false; // Gift mode: luôn bay thẳng
                }
                else
                {
                    projectile.checkRotate = originalWeaponRotateState; // Normal mode: theo cấu hình vũ khí
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hammer"))
        {
            isDeadPlayer = true;
            projectile.transform.localScale = new Vector3(20, 20, 20);
            EnemyAI.instance.weaponProjectile.transform.localScale = new Vector3(20, 20, 20);

            //SFX
            SFXManager.Instance.DeadSFX();
            SFXManager.Instance.LoseSFX();

            playerState = PlayerState.Die;
            animationController.SetDeadAnimation();
            GetComponentInChildren<WeaponAttack>().SetDead(true);
            Invoke(nameof(SetDeactiveGameObj), 2);

            if (GameController.instance != null)
                GameController.instance.SetPlayerAlive(false);

            SpawnEnemy.Instance?.NotifyCharacterDied(true);
            textRank.text = "#" + (SpawnEnemy.Instance?.GetRemainingCount() + 1);
            fixedJoyStick.SetActive(false);
            topUI.SetActive(false);
            panelCountDown.SetActive(true);
            int coinGet = coin;

            //Save coin
            GameController.instance.SaveCoin(coin);
            coinDeadScene.text = coin.ToString();
            UIManager.instance.isDead = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gift"))
        {
            ActivateGift();
        }
    }

    public void SetDefault()
    {
        DeactivateGift();
    }

    public void AddCoin(int amount)
    {
        coin += amount;
        coinText.text = coin.ToString();
        countUpCoin++;
        UpScale();
    }

    public int GetCoin()
    {
        return coin;
    }

    public void SetCoin(int coinSet)
    {
        this.coin = coinSet;
    }

    public void UpScale()
    {
        if (countUpCoin % 2 == 0 && countUpCoin != 0)
        {
            particleUpScale.gameObject.SetActive(true);
            particleUpScale.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleUpScale.Play();
            transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            CircleAroundPlayer circle = GetComponentInChildren<CircleAroundPlayer>();
            if (circle != null)
            {
                circle.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
                circle.DrawCircle();
            }
            Invoke(nameof(DisUpScale), 2f);
        }
    }

    public void DisUpScale()
    {
        particleUpScale.gameObject.SetActive(false);
    }

    void SetDeactiveGameObj() => gameObject.SetActive(false);
    public bool HasGift()
    {
        return isGift;
    }

    // Cũng có thể thêm method để get gift scale values
    public Vector3 GetGiftProjectileScale()
    {
        return new Vector3(50, 50, 50);
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
            Debug.Log("🎁 Player Gift activated!");

            projectile.checkRotate = false;
            weaponAttack.PushThrowOrigin(1f);
            weaponAttack.SetAttackRadius(7f);

            CircleAroundPlayer circle = GetComponentInChildren<CircleAroundPlayer>();
            if (circle != null)
            {
                circle.radius = 10f;
                circle.DrawCircle();
            }

            CameraFollow.instance.ShiftUp(3f);
        }
    }

    public void DeactivateGift()
    {
        if (isGift)
        {
            isGift = false;
            Debug.Log("🔄 Player Gift ended.");

            projectile.checkRotate = originalWeaponRotateState;
            weaponAttack.ResetThrowOrigin();
            weaponAttack.SetAttackRadius(5);

            CircleAroundPlayer circle = GetComponentInChildren<CircleAroundPlayer>();
            if (circle != null)
            {
                circle.radius = 7f;
                circle.DrawCircle();
            }

            CameraFollow.instance.ResetOffset();
        }
    }
}