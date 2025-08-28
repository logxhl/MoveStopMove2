using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool IsPlayerAlive { get; private set; } = true;
    public GameObject winScene;
    public GameObject joyStick;
    public TextMeshProUGUI textCoin;
    private int coin = 0;

    public GameObject setting;

    [Header("FreeItemSetting")]
    [SerializeField] private Image iconSprite;
    [SerializeField] private TextMeshProUGUI itemDes;
    [SerializeField] private ItemDatabase freeItem;

    [SerializeField] private AnimationController animationController;

    private int currentLevel;

    
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
        //textCoin.text = coin.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (textCoin != null)
        {
            coin = PlayerPrefs.GetInt("PlayerCoin");
            textCoin.text = coin.ToString();
        }
        if (IsPlayerAlive && winScene != null)
        {
            if (SpawnEnemy.Instance.GetRemainingCount() == 1)
            {
                CameraFollow.instance.WinCam();
                animationController.SetDanceWinAnimation();
                winScene.SetActive(true);
                joyStick.SetActive(false);
            }
        }
    }
    public void LoadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
    public void LoadSceneMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void LoadSceneZombieCity()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }


    public void SetPlayerAlive(bool alive)
    {
        IsPlayerAlive = alive;
    }
    public void SaveCoin(int coinEarned)
    {
        // Lấy coin cũ từ PlayerPrefs (nếu chưa có thì mặc định = 0)
        int oldCoin = PlayerPrefs.GetInt("PlayerCoin", 0);

        // Cộng coin mới vào coin cũ
        int totalCoin = oldCoin + coinEarned;

        // Lưu lại
        PlayerPrefs.SetInt("PlayerCoin", totalCoin);
        PlayerPrefs.Save();
    }
    public void SaveWeapon()
    {
        //PlayerPrefs.SetInt("WeaponData", ChooseWepon.instance.count);
    }

    public void RandomItemGift()
    {
        if (freeItem.itemGift.Length != 0)
        {
            int ind = Random.Range(0, freeItem.itemGift.Length);
            iconSprite.sprite = freeItem.itemGift[ind].itemIcon;
            itemDes.text = freeItem.itemGift[ind].itemDescription;
        }
    }
}
