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
    public TextMeshProUGUI textCoin;
    private int coin = 0;

    public GameObject setting;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //textCoin.text = coin.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (textCoin != null) {
            coin = PlayerPrefs.GetInt("PlayerCoin");
            textCoin.text = coin.ToString();
        }
        if(IsPlayerAlive && winScene != null)
        {
            if(SpawnEnemy.Instance.GetRemainingCount() == 1)
            {
                winScene.SetActive(true);
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
        PlayerPrefs.SetInt("WeaponData", ChooseWepon.instance.count);
    }
}
