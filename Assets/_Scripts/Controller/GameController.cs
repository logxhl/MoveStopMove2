using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool IsPlayerAlive { get; private set; } = true;
    public GameObject winScene;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
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
        SceneManager.LoadScene(1);
    }
    public void LoadSceneMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetPlayerAlive(bool alive)
    {
        IsPlayerAlive = alive;
    }
}
