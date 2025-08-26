using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panelCountDown;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Image circleImg;
    //[SerializeField] private float countdownTime;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float currentTime;
    [SerializeField] private GameObject deadScene;
    public TextMeshProUGUI shieldText;

    public static UIManager instance;

    public bool isDead = false;

    private void Start()
    {
        instance = this;
        currentTime = 5;
    }


    private void Update()
    {
        if (isDead)
        {
            Load();
        }
    }

    public void UpdateShield(int count)
    {
        shieldText.text = count + " Time";
    }

    public void Load()
    {
        currentTime -= Time.deltaTime;
        if (currentTime < 0)
        {
            currentTime = 0;
        }

        countdownText.text = Mathf.CeilToInt(currentTime).ToString();

        circleImg.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

        if (currentTime <= 0)
        {
            panelCountDown.SetActive(false);
            deadScene.SetActive(true);

        }
    }

}
