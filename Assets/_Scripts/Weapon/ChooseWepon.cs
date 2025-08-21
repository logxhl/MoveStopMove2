using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseWepon : MonoBehaviour
{
    public static ChooseWepon instance;
    public WeaponTableObject weaponData;
    public Image imgWeapon;
    public int count;
    public TextMeshProUGUI nameWeapon;
    public TextMeshProUGUI unLock;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI coin;

    [Header("Weapon Holder(Right Hand)")]
    public Transform weaponHolder;
    private GameObject currentWeapon;

    [Header("UI Buy Button")]
    public Button btnBuyCoin;
    public Image btnBuyCoinImg;
    public TextMeshProUGUI textBuyCoin;
    public Color equipColor = Color.yellow * 0.8f; // nhân < 1 để màu tối hơn
    public Color buyColor = Color.green;
    public Color ownedColor = Color.yellow;
    public Image imgCoin;

    //--Peristed keys--
    private const string LoadWeaponKey = "LoadWeapon";
    private const string OwnedWeaponsKey = "OwnedWeapons";
    private const string PlayerCoinKey = "PlayerCoin";

    //Danh sach da mua
    private HashSet<int> ownedSet = new HashSet<int>();

    [SerializeField] private GameObject panelItemSelect;
    [SerializeField] private List<Button> btnSelect;
    private bool isBought = false;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);

        if (weaponHolder.childCount > 0 && weaponHolder != null)
        {
            currentWeapon = weaponHolder.GetChild(0).gameObject;
        }
        if (btnBuyCoin != null)
        {
            btnBuyCoin.onClick.RemoveListener(OnBuyOrEquip);
            btnBuyCoin.onClick.AddListener(OnBuyOrEquip);
        }
    }

    private void Start()
    {
        count = PlayerPrefs.GetInt(LoadWeaponKey, 0);

        //Load danh sach da mua
        LoadOwnedWeapons();
        if (!ownedSet.Contains(count))
        {
            ownedSet.Add(count);
            SaveOwnedWeapons();
        }

        UpdateWeapon(count);

        //Clean weapon cu trong holder
        if (weaponHolder != null)
        {
            for (int i = weaponHolder.childCount - 1; i >= 0; i--)
            {
                Destroy(weaponHolder.GetChild(i).gameObject);
            }
        }
        EquipWeapon(count);
    }

    public void NextButton()
    {
        count++;
        if (count > (weaponData.GetCountWeapon() - 1))
        {
            count = 0;
        }
        UpdateWeapon(count);
        //UpdateBuyBtnUI();
    }

    public void BackButton()
    {
        count--;
        if (count < 0)
        {
            count = weaponData.GetCountWeapon() - 1;
        }
        UpdateWeapon(count);
        //UpdateBuyBtnUI();
    }

    //public void UpdateWeapon(int count)
    //{
    //    nameWeapon.text = weaponData.GetWeapon(count).nameWepon;
    //    unLock.text = weaponData.GetWeapon(count).unlock;
    //    damage.text = weaponData.GetWeapon(count).damage;
    //    imgWeapon.sprite = weaponData.GetWeapon(count).spite[0];
    //    coin.text = weaponData.GetWeapon(count).coin.ToString();
    //    if(isBought)
    //    {
    //        panelItemSelect.SetActive(true);
    //        for(int i = 0; i < btnSelect.Count; i++)
    //        {
    //            btnSelect[i].GetComponentInChildren<Image>().sprite = weaponData.weapon[i].spite[i];
    //        }
    //    }
    //    else
    //    {
    //        panelItemSelect.SetActive(false);
    //    }

    //    UpdateBuyBtnUI();
    //}
    public void UpdateWeapon(int count)
    {
        Weapon currentWp = weaponData.GetWeapon(count);

        nameWeapon.text = currentWp.nameWepon;
        unLock.text = currentWp.unlock;
        damage.text = currentWp.damage;
        imgWeapon.sprite = currentWp.spite[0]; // sprite mặc định
        coin.text = currentWp.coin.ToString();

        // Nếu đã mua thì bật panel skin + điền sprite vào nút
        if (ownedSet.Contains(count))  // hoặc if (isBought)
        {
            panelItemSelect.SetActive(true);

            for (int i = 0; i < btnSelect.Count; i++)
            {
                if (i < currentWp.spite.Length)
                {
                    // Hiện nút và gán sprite
                    btnSelect[i].gameObject.SetActive(true);
                    Image childImg = btnSelect[i].transform.Find("Image").GetComponent<Image>();
                    childImg.sprite = currentWp.spite[i];
                    // Xóa listener cũ và add listener mới để chọn skin
                    int index = i; // cần biến local tránh lỗi closure
                    btnSelect[i].onClick.RemoveAllListeners();
                    btnSelect[i].onClick.AddListener(() =>
                    {
                        OnSelectSkin(index, currentWp);
                    });
                }
                else
                {
                    // Ẩn nút nếu không có sprite tương ứng
                    btnSelect[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            panelItemSelect.SetActive(false);
        }

        UpdateBuyBtnUI();
    }
    private void OnSelectSkin(int spriteIndex, Weapon weapon)
    {
        // đổi sprite chính trên UI
        imgWeapon.sprite = weapon.spite[spriteIndex];

        // Đổi màu vũ khí trong game (nếu weaponPrefab có MeshRenderer)
        if (currentWeapon != null)
        {
            var renderer = currentWeapon.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                // Ví dụ: đổi màu theo index
                switch (spriteIndex)
                {
                    case 0: renderer.material.color = Color.red; break;
                    case 1: renderer.material.color = Color.gray; break;
                    case 2: renderer.material.color = Color.green; break;
                    case 3: renderer.material.color = Color.blue; break;
                }
            }
        }
    }


    private void EquipWeapon(int index)
    {
        Weapon selectedWeapon = weaponData.GetWeapon(index);
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentWeapon = Instantiate(selectedWeapon.weaponPrefab, weaponHolder);
    }

    public void BuyWeapon()
    {
        int coinWeapon = weaponData.GetWeapon(count).coin;
        int coinPlayer = PlayerPrefs.GetInt("PlayerCoin", 0);
        if (coinWeapon <= coinPlayer)
        {
            coinPlayer -= coinWeapon;

            PlayerPrefs.SetInt("PlayerCoin", coinPlayer);
            PlayerPrefs.Save();
            Debug.Log("Mua thành công! Xu còn lại: " + coinPlayer);

            EquipWeapon(count);
            PlayerPrefs.SetInt("LoadWeapon", count);
            PlayerPrefs.Save();
            Debug.Log("Equiped: " + weaponData.GetWeapon(count).nameWepon);

            //Doi giao dien thanh equip
            btnBuyCoinImg.color = equipColor;
            textBuyCoin.text = "EQUIP";
        }
        else
        {
            Debug.Log("Không đủ xu để mua!");
        }

    }
    public void OnBuyOrEquip()
    {
        //Neu da mua => chi trang bi
        if (ownedSet.Contains(count))
        {
            PlayerPrefs.SetInt(LoadWeaponKey, count);
            PlayerPrefs.Save();
            EquipWeapon(count);
            UpdateBuyBtnUI();
            return;
        }

        //Chua mua => tru xu
        int price = weaponData.GetWeapon(count).coin;
        int playerCoin = PlayerPrefs.GetInt(PlayerCoinKey, 0);
        if (playerCoin >= price)
        {
            playerCoin -= price;
            //playerCoin += 1000000;
            PlayerPrefs.SetInt(PlayerCoinKey, playerCoin);
            //Danh dau da mua
            ownedSet.Add(count);
            SaveOwnedWeapons();

            //Trang bi luon
            PlayerPrefs.SetInt(LoadWeaponKey, count);
            PlayerPrefs.Save();

            EquipWeapon(count);
            UpdateBuyBtnUI();
            Debug.Log("Mua thanh cong, coin con lai: " + playerCoin);
        }
        else
        {
            Debug.Log("Khong du xu de mua");
        }
    }

    private void SaveOwnedWeapons()
    {
        string raw = string.Join(",", ownedSet.OrderBy(i => i));
        PlayerPrefs.SetString(OwnedWeaponsKey, raw);
        PlayerPrefs.Save();
    }

    private void LoadOwnedWeapons()
    {
        ownedSet.Clear();
        string raw = PlayerPrefs.GetString(OwnedWeaponsKey, "");
        if (string.IsNullOrEmpty(raw)) return;
        foreach (var s in raw.Split(','))
        {
            if (int.TryParse(s, out int idx))
                ownedSet.Add(idx);
        }
    }

    private void UpdateBuyBtnUI()
    {
        int saveWeaponInd = PlayerPrefs.GetInt(LoadWeaponKey, -1);
        if (saveWeaponInd == count)
        {
            //Vu khi da dc trang bi
            btnBuyCoinImg.color = equipColor;
            textBuyCoin.text = "    EQUIPPED";
            if (imgCoin != null) imgCoin.gameObject.SetActive(false);
            isBought = true;
        }
        else if (ownedSet.Contains(count))
        {
            //Da mua nhung chua trang bi
            btnBuyCoinImg.color = ownedColor;
            textBuyCoin.text = "    EQUIP";
            if (imgCoin != null) imgCoin.gameObject.SetActive(false);
            isBought = true;
        }
        else
        {
            //Vu khi chua mua
            btnBuyCoinImg.color = Color.green;
            textBuyCoin.text = weaponData.GetWeapon(count).coin.ToString();
            if (imgCoin != null) imgCoin.gameObject.SetActive(true);
            isBought = false;
        }
    }
}
