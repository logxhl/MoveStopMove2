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
    [SerializeField] private GameObject[] wps;
    //public Image imgWeapon;
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
    [SerializeField] private GameObject[] btnSelect;
    private bool isBought = false;
    public int indexWp;
    private int tempSelectedSkin = 0; // skin vừa chọn nhưng chưa equip

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
        foreach (GameObject wp in wps)
            wp.SetActive(false);


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
        SetMaterial();

        // Gán listener cho các nút chọn skin
        for (int i = 0; i < btnSelect.Length; i++)
        {
            int index = i;
            btnSelect[index].GetComponent<Button>().onClick.RemoveAllListeners();
            btnSelect[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                OnSelectSkin(index, weaponData.GetWeapon(count));
            });
        }
    }

    public void SetMaterial()
    {
        Weapon wp = weaponData.GetWeapon(count);
        if (ownedSet.Contains(count))
        {
            int indMaterial = PlayerPrefs.GetInt("MaterialOfWp" + count);
            MeshRenderer meshRenderer = currentWeapon.GetComponent<MeshRenderer>();
            Material[] mats = meshRenderer.materials;
            int max = Mathf.Min(
mats.Length,
weaponData.listMaterials[count].materialOfHammer[indMaterial].materials.Length
);

            for (int i = 0; i < max; i++)
            {
                mats[i] = weaponData.listMaterials[count].materialOfHammer[indMaterial].materials[i];
            }

            meshRenderer.materials = mats;

        }
    }

    /// <summary>
    /// Reset toàn bộ nút skin rồi set nút equip
    /// </summary>
    public void SetButtonMaterial(int equippedIndex)
    {
        for (int i = 0; i < btnSelect.Length; i++)
        {
            if (!btnSelect[i].activeSelf) continue;

            //var txt = btnBuyCoin.GetComponentInChildren<TextMeshProUGUI>();
            //txt.text = (i == equippedIndex) ? "Equipped" : "Select";

            var txt = btnSelect[i].GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                if (i == equippedIndex)
                    txt.text = "EQUIPPED"; // đã chọn
                else
                    txt.text = "    SELECT";    // chưa chọn
            }
        }
    }

    public void LoadCurrentWp()
    {
        foreach (GameObject wp in wps)
            wp.SetActive(false);
        count = PlayerPrefs.GetInt(LoadWeaponKey, 0);
        wps[count].SetActive(true);
    }

    public void SetActiveWp(bool active)
    {
        foreach (GameObject wp in wps)
            wp.SetActive(active);
    }
    public void NextButton()
    {
        wps[count].SetActive(false);
        count++;
        if (count > (weaponData.GetCountWeapon() - 1))
        {
            count = 0;
        }
        wps[count].SetActive(true);
        UpdateWeapon(count);
        //UpdateBuyBtnUI();
    }

    public void BackButton()
    {
        wps[count].SetActive(false);
        count--;
        if (count < 0)
        {
            count = weaponData.GetCountWeapon() - 1;
        }
        wps[count].SetActive(true);
        UpdateWeapon(count);
        //UpdateBuyBtnUI();
    }

    public void UpdateWeapon(int count)
    {
        Weapon currentWp = weaponData.GetWeapon(count);

        nameWeapon.text = currentWp.nameWepon;
        unLock.text = currentWp.unlock;
        damage.text = currentWp.damage;
        //imgWeapon.sprite = currentWp.spite[0]; // sprite mặc định
        coin.text = currentWp.coin.ToString();

        if (ownedSet.Contains(count))
        {
            panelItemSelect.SetActive(true);

            // Lấy skin hiện tại đang được trang bị
            int currentEquippedSkin = PlayerPrefs.GetInt("MaterialOfWp" + count, 0);

            for (int i = 0; i < btnSelect.Length; i++)
            {
                if (i < currentWp.spite.Length)
                {
                    btnSelect[i].gameObject.SetActive(true);
                    Image childImg = btnSelect[i].transform.Find("Image").GetComponent<Image>();
                    childImg.sprite = currentWp.spite[i];

                    // Setup listener cho từng button
                    int index = i;
                    btnSelect[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    btnSelect[i].GetComponent<Button>().onClick.AddListener(() =>
                    {
                        OnSelectSkin(index, currentWp);
                    });
                }
                else
                {
                    btnSelect[i].gameObject.SetActive(false);
                }
            }

            // Cập nhật trạng thái button ban đầu
            SetButtonMaterial(currentEquippedSkin);

            // Apply material hiện tại cho weapon display
            tempSelectedSkin = currentEquippedSkin;
            OnSelectSkin(currentEquippedSkin, currentWp);
        }
        else
        {
            panelItemSelect.SetActive(false);
        }

        UpdateBuyBtnUI();
    }
    private void OnSelectSkin(int spriteIndex, Weapon weapon)
    {
        // Đảm bảo lấy đúng weapon hiện tại
        weapon = weaponData.GetWeapon(count);

        // Lấy GameObject weapon đang hiển thị ở giữa (trong wps array)
        GameObject currentDisplayWeapon = wps[count];

        // Kiểm tra nếu đã mua weapon này
        if (ownedSet.Contains(count))
        {
            // Lấy MeshRenderer của weapon đang hiển thị
            MeshRenderer meshRenderer = currentDisplayWeapon.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                Debug.LogError("Không tìm thấy MeshRenderer trên weapon: " + currentDisplayWeapon.name);
                return;
            }

            // Kiểm tra xem có đủ material cho skin này không
            if (spriteIndex >= weaponData.listMaterials[count].materialOfHammer.Length)
            {
                Debug.LogError("Skin index vượt quá số lượng material có sẵn!");
                return;
            }

            // Lấy material array tương ứng với skin được chọn
            Material[] newMaterials = weaponData.listMaterials[count].materialOfHammer[spriteIndex].materials;

            // Tạo copy của materials hiện tại
            Material[] currentMaterials = meshRenderer.materials;

            // Thay thế material, đảm bảo không vượt quá số lượng material slots
            int maxMaterials = Mathf.Min(currentMaterials.Length, newMaterials.Length);

            for (int i = 0; i < maxMaterials; i++)
            {
                currentMaterials[i] = newMaterials[i];
            }

            // Apply materials mới
            meshRenderer.materials = currentMaterials;

            Debug.Log($"Đã thay đổi material skin {spriteIndex} cho weapon {weapon.nameWepon}");
        }

        // Lưu tạm index skin đã chọn
        tempSelectedSkin = spriteIndex;

        // Cập nhật UI button
        var txt = btnBuyCoin.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
            txt.text = "    SELECT";

        // Cập nhật trạng thái các button skin
        SetButtonMaterial(spriteIndex);
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
            textBuyCoin.text = "    SELECT";
        }
        else
        {
            Debug.Log("Không đủ xu để mua!");
        }

    }
    public void OnBuyOrEquip()
    {
        if (ownedSet.Contains(count)) // đã mua
        {
            PlayerPrefs.SetInt(LoadWeaponKey, count);
            PlayerPrefs.SetInt("MaterialOfWp" + count, tempSelectedSkin); // lưu skin đã chọn khi equip
            PlayerPrefs.Save();

            EquipWeapon(count);    // đổi weapon prefab
            SetMaterial();         // áp dụng material thật
            SetButtonMaterial(tempSelectedSkin); // đổi nút sang trạng thái equip

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
            PlayerPrefs.SetInt("MaterialOfWp" + count, tempSelectedSkin);
            PlayerPrefs.Save();

            EquipWeapon(count);
            SetMaterial();
            SetButtonMaterial(tempSelectedSkin);
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
            textBuyCoin.text = "    SELECT";
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
