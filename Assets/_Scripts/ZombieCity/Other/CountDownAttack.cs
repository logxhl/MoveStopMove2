using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountDownAttack : MonoBehaviour
{
    [SerializeField] private GameObject panelCountDown;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float currentTime;
    [SerializeField] private GameObject joyStick;
    [SerializeField] private GameObject spawnZombie;
    public static CountDownAttack instance;

    private void Start()
    {
        instance = this;
        currentTime = 3;
    }


    private void Update()
    {
        Load();
    }

    public void Load()
    {
        currentTime -= Time.deltaTime;
        if (currentTime < 0)
        {
            currentTime = 0;
        }

        countdownText.text = Mathf.CeilToInt(currentTime).ToString();
                if (currentTime <= 0)
        {
            panelCountDown.SetActive(false);
            joyStick.SetActive(true);
            spawnZombie.SetActive(true);
        }
    }
}
