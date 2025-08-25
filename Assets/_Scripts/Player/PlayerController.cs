using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
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

    //[SerializeField] private CoinSystem coinSystem;
    //public CoinSystem GetCoinSystem => coinSystem;

    private int coin = 0;
    public int countUpCoin = 0;
    private bool isGift = false;

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
    }
  
    void Update()
    {
        //GetPant();
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



        //if (Input.GetKey(KeyCode.A))
        //{
        //    animationController.SetDeadAnimation();
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    animationController.SetDanceWinAnimation();
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    animationController.SetDanceAnimation();
        //}
        //else if (Input.GetKey(KeyCode.W))
        //{
        //    animationController.SetUltiAnimation();
        //}
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


                // Set rotate
                projectile.checkRotate = listWeapon.weaponList[i].isRotate;
            }
        }
    }


    public void GetPant()
    {
        int index = PlayerPrefs.GetInt("SelectedPant");
        for (int i = 0; i < listPant.pantInfo.Length; i++)
        {
            if (index == listPant.pantInfo[i].index)
            {
                currentPant.GetComponent<SkinnedMeshRenderer>().material = listPant.pantInfo[i].pantMaterials;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hammer"))
        {
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
            //int coin = ((SpawnEnemy.Instance.totalEnemiesToSpawn - SpawnEnemy.Instance.GetRemainingCount()) * 5);
            int coinGet = coin;

            //Save coin
            GameController.instance.SaveCoin(coin);
            coinDeadScene.text = coin.ToString();
            //UIManager.instance.Load();
            UIManager.instance.isDead = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gift"))
        {
            isGift = true;
            //Debug.Log("Va cham");
            weaponAttack.PushThrowOrigin(1f);
            weaponAttack.SetAttackRadius(7f);
            CircleAroundPlayer circle = GetComponentInChildren<CircleAroundPlayer>();
            if (circle != null)
            {
                circle.radius = 10f;
                circle.DrawCircle();
                projectile.transform.localScale = new Vector3(50, 50, 50);
            }
            CameraFollow.instance.ShiftUp(3f);
        }
    }
    public void SetDefault()
    {
        if (isGift)
        {
            isGift = false;
            weaponAttack.ResetThrowOrigin();
            weaponAttack.SetAttackRadius(5);
            CircleAroundPlayer circle = GetComponentInChildren<CircleAroundPlayer>();
            if (circle != null)
            {
                circle.radius = 7f;
                circle.DrawCircle();
                projectile.transform.localScale = new Vector3(20, 20, 20);
            }
            CameraFollow.instance.ResetOffset();
        }
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
            Invoke(nameof(DisUpScale), 2f);
        }
    }
    public void DisUpScale()
    {
        particleUpScale.gameObject.SetActive(false);
    }

    void SetDeactiveGameObj() => gameObject.SetActive(false);
}