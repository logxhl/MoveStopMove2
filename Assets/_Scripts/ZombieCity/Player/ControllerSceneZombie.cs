using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerSceneZombie : MonoBehaviour
{
    public static ControllerSceneZombie instance;
    public bool IsPlayerAlive { get; private set; } = true;
    public GameObject winScene;
    public GameObject joyStick;
    public TextMeshProUGUI textCoin;
    private int coin = 0;

    public GameObject setting;
    [SerializeField] private AnimationController anim;

    private void Awake()
    {
        //Time.timeScale = 0f;
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
        instance = this;
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
            if (SpawnZombie.instance.GetRemainingCount() == 0)
            {
                CameraFollow.instance.WinCam();
                anim.SetDanceWinAnimation();
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
    public void UnPause()
    {
        Time.timeScale = 1f;
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
}
