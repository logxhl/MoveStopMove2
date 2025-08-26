using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using System.Collections;

public class PlayerSceneZombie : MonoBehaviour
{
    public static PlayerSceneZombie instance;
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

    private Skill activeSkill;
    public TextMeshProUGUI levelUp;

    [SerializeField] private GameObject shieldShpere;
    public float shieldDuration = 3f;
    public int shieldCount = 0;

    public bool isShieldActive = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;

        rigid = GetComponent<Rigidbody>();
        if (animationController == null)
            animationController = GetComponentInChildren<AnimationController>();
    }
    private void Start()
    {
        shieldShpere.SetActive(false);
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
    public void SetSkill(Skill skill)
    {
        activeSkill = skill;
        if (skill.skillSpecial)
        {
            switch (skill.skillType)
            {
                case SkillType.DoubleThrow:
                    Debug.Log("Double Throw actived: " + skill.skillName);
                    weaponAttack.EnableDoubleThrow(true);
                    break;
                case SkillType.Triple:
                    Debug.Log("Triple Spread actived: " + skill.skillName);
                    weaponAttack.EnableTripleSpread(true);
                    break;
                case SkillType.UpScale:
                    Debug.Log("Skill UpScale");
                    SkillUpScale();
                    break;
                case SkillType.MoveFaster:
                    Debug.Log("Skill MoveFaster");
                    SkillMoveFaster();
                    break;
                default:
                    Debug.Log("Unknown special skill: " + skill.skillName);
                    break;
            }
        }
    }
    public void SkillUpScale()
    {
        transform.localScale += new Vector3(0.4f, 0.4f, 0.4f);
        weaponAttack.SetAttackRadius(10f);
        CircleAroundPlayer circle = GetComponentInChildren<CircleAroundPlayer>();
        if (circle != null)
        {
            circle.radius = 9f;
            circle.DrawCircle();
        }
    }
    public void SkillMoveFaster()
    {
        moveSpeed = 7f;
    }
    public Skill GetActiveSkill()
    {
        return activeSkill;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Nếu bị zombie (Bot) đụng thì gọi OnHitByZombie
        if (other.CompareTag("Bot"))
        {
            AIOfZombie zombie = other.GetComponent<AIOfZombie>();
            if (zombie != null)
            {
                OnHitByZombie(zombie);
            }
        }
    }

    public void OnHitByZombie(AIOfZombie zombie)
    {
        if (isShieldActive)
        {
            Debug.Log("⚡ Player đang có shield, không chết");
            return;
        }

        if (shieldCount > 0)
        {
            Debug.Log("Shield = " + shieldCount);
            StartCoroutine(ActivateShield());
            Debug.Log("⚡ Shield được kích hoạt để chặn zombie");
        }
        else
        {
            Debug.Log("💀 Player Dead");

            // Báo cho zombie thắng
            zombie.SwitchState(ZombieState.Victory);

            //SFX
            SFXManager.Instance.DeadSFX();
            SFXManager.Instance.LoseSFX();

            playerState = PlayerState.Die;
            animationController.SetDeadAnimation();
            Invoke(nameof(SetDeactiveGameObj), 2);

            if (ControllerSceneZombie.instance != null)
                ControllerSceneZombie.instance.SetPlayerAlive(false);

            SpawnZombie.instance?.NotifyCharacterDied(true);
            fixedJoyStick.SetActive(false);
            topUI.SetActive(false);
            panelCountDown.SetActive(true);

            //Save coin
            ControllerSceneZombie.instance.SaveCoin(coin);
            coinDeadScene.text = coin.ToString();
            UIManager.instance.isDead = true;
        }
    }



    private IEnumerator ActivateShield()
    {
        shieldCount--;
        isShieldActive = true;
        shieldShpere.SetActive(true);
        yield return new WaitForSeconds(shieldDuration);
        shieldShpere.SetActive(false);
        isShieldActive = false;
    }
    private void UpdateShieldUI()
    {

    }

    public void AddCoin(int amount)
    {
        coin += amount;
        coinText.text = coin.ToString();
        countUpCoin++;
        //UpScale();
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
    public void SetDefault()
    {

    }
    public void ShowLevelUp()
    {
        levelUp.color = new Color(1, 1, 1, 1);
        levelUp.transform.localPosition = Vector3.zero;
        levelUp.transform.DOLocalMoveY(100f, 1f).SetEase(Ease.OutCubic);

        DG.Tweening.DOTweenModuleUI.DOFade(levelUp, 0f, 1f) // gọi tường minh
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                levelUp.gameObject.SetActive(false); // tắt đi sau khi xong
            });

        levelUp.gameObject.SetActive(true);
        weaponAttack.EnableDoubleThrow(true);
    }

}
