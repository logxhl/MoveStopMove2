using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [SerializeField] private GameObject[] levels;
    private int currentLevel;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
            // Đăng ký lắng nghe sự kiện scene load
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void OnDestroy()
    {
        // Hủy đăng ký để tránh memory leak
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Mỗi lần scene được load, gọi lại ShowLevel
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        ShowLevel(currentLevel);
    }
    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        Debug.Log("level: " + currentLevel);
        ShowLevel(currentLevel);
        //PlayerPrefs.DeleteKey("CurrentLevel");
    }
    public void NextLevel()
    {
        currentLevel = 2;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    public void ShowLevel(int level)
    {
        foreach (GameObject lv in levels)
        {
            lv.SetActive(false);
        }
        int ind = Mathf.Clamp(level - 1, 0, levels.Length - 1);
        levels[ind].SetActive(true);
    }
}
