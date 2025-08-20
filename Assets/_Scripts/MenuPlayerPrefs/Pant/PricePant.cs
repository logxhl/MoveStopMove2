using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PricePant : MonoBehaviour
{
    [SerializeField] private List<Button> shopButtons;   // kéo thả tất cả button vào đây
    [SerializeField] private int[] prices;               // mảng giá cho từng button
    [SerializeField] private TextMeshProUGUI textBuy;
    private int currentPrice = 0;
    private int playerCoin;
    [SerializeField] private GameObject btnSelect;
    [SerializeField] private GameObject btnBuy;
    [SerializeField] private GameObject btnAds;
    private int layerBtn;
    private TextMeshProUGUI txtSelect;
    private void Awake()
    {
        txtSelect = btnSelect.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        for (int i = 0; i < shopButtons.Count; i++)
        {
            int price = prices[i];   // lấy giá tương ứng
            int index = i;           // cần lưu index tránh lỗi delegate
            // Gán sự kiện khi click
            shopButtons[i].onClick.AddListener(() => OnItemClick(index, price));
        }
    }
    private void Update()
    {
        int indBtn = PlayerPrefs.GetInt("SelectHairBtn", -1);
        if (layerBtn == indBtn)
        {
            txtSelect.text = "Equipped";
            btnSelect.GetComponent<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
        }
        else
        {
            txtSelect.text = "SELECT";
            btnSelect.GetComponent<Image>().color = new Color(1f, 221f / 255f, 0f);
        }
    }

    private void OnItemClick(int index, int price)
    {
        currentPrice = price;
        textBuy.text = price.ToString();
        layerBtn = index;
        SetDefaulBtn(layerBtn);
    }

    public void OnBuyBtnClick()
    {
        if (currentPrice <= 0)
        {
            Debug.Log("Chua chon item nao");
            return;
        }
        playerCoin = PlayerPrefs.GetInt("PlayerCoin", 0);
        if (playerCoin >= currentPrice)
        {
            playerCoin -= currentPrice;
            PlayerPrefs.SetInt("PlayerCoin", playerCoin);
            PlayerPrefs.Save();


            PlayerPrefs.SetInt("Hair_" + layerBtn, 1);
            Debug.Log("Mua thành công! Xu còn lại: " + playerCoin);
            btnBuy.SetActive(false);
            btnAds.SetActive(false);
            btnSelect.SetActive(true);
        }
        else
        {
            Debug.Log("Khong du tien mua");
        }
    }
    public void SetDefaulBtn(int ind)
    {
        int selectInd = PlayerPrefs.GetInt("Hair_" + ind, 0);
        if (selectInd == 1)
        {
            btnSelect.SetActive(true);
            btnBuy.SetActive(false);
            btnAds.SetActive(false);
        }
        else
        {
            btnSelect.SetActive(false);
            btnBuy.SetActive(true);
            btnAds.SetActive(true);
        }
    }
    public void ButtonSelect()
    {
        if (txtSelect.text == "SELECT")
        {
            PlayerPrefs.SetInt("SelectHairBtn", layerBtn);
            txtSelect.text = "Equipped";
            btnSelect.GetComponent<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
        }
    }
}
