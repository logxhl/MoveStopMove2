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

    [Header("Custom Color System")]
    public GameObject customColorPanel;
    public Transform colorButtonContainer;
    public GameObject colorButtonPrefab;
    public Button btnSelectCustom; // Nút "Select" màu vàng
    public Color[] availableColors = new Color[]
    {
        // Hàng 1
        Color.red * 0.7f, Color.green, Color.blue, Color.yellow * 0.8f, Color.cyan,
        Color.magenta, new Color(0.5f, 0f, 1f), new Color(0.5f, 0.5f, 0f), Color.black,
        // Hàng 2  
        Color.red, Color.green * 1.2f, new Color(0.3f, 0.3f, 1f), Color.yellow,
        new Color(0f, 1f, 1f), new Color(1f, 0.7f, 0.8f), new Color(0.7f, 0f, 1f),
        new Color(0.7f, 1f, 0.3f), Color.white
    };

    [Header("Material Selection")]
    public Transform materialSelectionContainer;
    public GameObject materialSelectionButtonPrefab;

    private List<Button> colorButtons = new List<Button>();
    private List<Button> materialButtons = new List<Button>();
    private int selectedMaterialIndex = 0;
    private Color selectedCustomColor = Color.white;
    private bool isCustomMode = false;

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
        int isCustomModeLoaded = PlayerPrefs.GetInt("IsCustomMode_" + count, 0);
        if (isCustomModeLoaded == 1)
        {
            isCustomMode = true;
            customColorPanel.SetActive(true);

            // Cập nhật UI để hiển thị đúng trạng thái
            UpdateBuyBtnUI();
        }
        else
        {
            isCustomMode = false;
            customColorPanel.SetActive(false);
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

        // Áp dụng skin/material đã lưu cho vũ khí
        ApplySavedSkinToWeapon(wps[count], count);
    }
    // Hàm mới để áp dụng skin/material đã lưu cho weapon
    private void ApplySavedSkinToWeapon(GameObject weaponObj, int weaponIndex)
    {
        if (weaponObj == null) return;

        // Kiểm tra xem vũ khí này có phải custom mode không
        bool isCustomMode = PlayerPrefs.GetInt("IsCustomMode_" + weaponIndex, 0) == 1;

        if (isCustomMode)
        {
            // Áp dụng màu custom đã lưu
            ApplySavedCustomColorsToWeapon(weaponObj, weaponIndex);
        }
        else
        {
            // Áp dụng skin material thông thường đã lưu
            ApplySavedMaterialToWeapon(weaponObj, weaponIndex);
        }
    }

    // Hàm áp dụng material thông thường đã lưu
    private void ApplySavedMaterialToWeapon(GameObject weaponObj, int weaponIndex)
    {
        MeshRenderer meshRenderer = weaponObj.GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;

        // Lấy index material đã lưu
        int materialIndex = PlayerPrefs.GetInt("MaterialOfWp" + weaponIndex, 0);

        // Kiểm tra xem material index có hợp lệ không
        if (weaponIndex < weaponData.listMaterials.Length &&
            materialIndex < weaponData.listMaterials[weaponIndex].materialOfHammer.Length)
        {
            Material[] newMaterials = weaponData.listMaterials[weaponIndex].materialOfHammer[materialIndex].materials;
            Material[] currentMaterials = meshRenderer.materials;

            // Thay thế material, đảm bảo không vượt quá số lượng material slots
            int maxMaterials = Mathf.Min(currentMaterials.Length, newMaterials.Length);

            for (int i = 0; i < maxMaterials; i++)
            {
                currentMaterials[i] = newMaterials[i];
            }

            meshRenderer.materials = currentMaterials;
        }
    }

    // Hàm áp dụng màu custom đã lưu (sửa lại từ hàm cũ để nhận weaponIndex)
    private void ApplySavedCustomColorsToWeapon(GameObject weaponObj, int weaponIndex)
    {
        if (weaponObj == null) return;

        MeshRenderer meshRenderer = weaponObj.GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;

        Material[] materials = meshRenderer.materials;
        bool hasCustomColors = false;

        for (int i = 0; i < materials.Length; i++)
        {
            string colorKey = "CustomColor_" + weaponIndex + "_" + i;
            string colorHtml = PlayerPrefs.GetString(colorKey, "");

            if (!string.IsNullOrEmpty(colorHtml))
            {
                Color customColor;
                if (ColorUtility.TryParseHtmlString("#" + colorHtml, out customColor))
                {
                    // Tạo material mới để không ảnh hưởng đến prefab gốc
                    Material newMat = new Material(materials[i]);
                    newMat.color = customColor;
                    materials[i] = newMat;
                    hasCustomColors = true;
                }
            }
        }

        if (hasCustomColors)
        {
            meshRenderer.materials = materials;
        }
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
        coin.text = currentWp.coin.ToString();

        // LUÔN RESET TRẠNG THÁI CUSTOM MODE KHI CHUYỂN VŨ KHÍ
        isCustomMode = false;

        // Nếu đã mua thì hiển thị cả panel skin và custom
        if (ownedSet.Contains(count))
        {
            panelItemSelect.SetActive(true);
            SetupSkinButtons(currentWp);
            SetupCustomColorSystem(currentWp);

            // Kiểm tra xem vũ khí này có phải custom mode không
            bool isCurrentWeaponCustom = PlayerPrefs.GetInt("IsCustomMode_" + count, 0) == 1;

            if (isCurrentWeaponCustom)
            {
                isCustomMode = true;
                customColorPanel.SetActive(true);

                // Áp dụng màu custom đã lưu cho weapon preview
                ApplySavedCustomColorsToWeapon(wps[count]);

                Debug.Log("Tự động kích hoạt chế độ custom cho weapon: " + currentWp.nameWepon);
            }
            else
            {
                // ĐẢM BẢO ẨN BẢNG MÀU KHI VŨ KHÍ KHÔNG PHẢI CUSTOM MODE
                isCustomMode = false;
                customColorPanel.SetActive(false);
            }
        }
        else
        {
            // ĐẢM BẢO ẨN BẢNG MÀU KHI VŨ KHÍ CHƯA MUA
            panelItemSelect.SetActive(false);
            customColorPanel.SetActive(false);
            isCustomMode = false;
        }

        UpdateBuyBtnUI();
    }
    // Hàm mới để áp dụng màu custom đã lưu cho weapon
    private void ApplySavedCustomColorsToWeapon(GameObject weaponObj)
    {
        if (weaponObj == null) return;

        MeshRenderer meshRenderer = weaponObj.GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;

        Material[] materials = meshRenderer.materials;
        bool hasCustomColors = false;

        for (int i = 0; i < materials.Length; i++)
        {
            string colorKey = "CustomColor_" + count + "_" + i;
            string colorHtml = PlayerPrefs.GetString(colorKey, "");

            if (!string.IsNullOrEmpty(colorHtml))
            {
                Color customColor;
                if (ColorUtility.TryParseHtmlString("#" + colorHtml, out customColor))
                {
                    // Tạo material mới để không ảnh hưởng đến prefab gốc
                    Material newMat = new Material(materials[i]);
                    newMat.color = customColor;
                    materials[i] = newMat;
                    hasCustomColors = true;
                }
            }
        }

        if (hasCustomColors)
        {
            meshRenderer.materials = materials;
        }
    }
    private void SetupSkinButtons(Weapon currentWp)
    {
        int lastIndex = btnSelect.Length - 1; // vị trí cuối cùng trong mảng

        for (int i = 0; i < btnSelect.Length; i++)
        {
            if (i == lastIndex)
            {
                // Button cuối cùng là Custom
                btnSelect[i].gameObject.SetActive(true);
                var childImg = btnSelect[i].transform.Find("Image").GetComponent<Image>();
                childImg.sprite = currentWp.spite[0]; // dùng sprite đầu tiên làm base

                // Set text "(Custom)"
                var customText = btnSelect[i].GetComponentInChildren<TextMeshProUGUI>();
                if (customText != null)
                    customText.text = "(Custom)";

                btnSelect[i].GetComponent<Button>().onClick.RemoveAllListeners();
                btnSelect[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnSelectCustomMode();
                });
            }
            else if (i < currentWp.spite.Length)
            {
                // Các button skin bình thường
                btnSelect[i].gameObject.SetActive(true);
                Image childImg = btnSelect[i].transform.Find("Image").GetComponent<Image>();
                childImg.sprite = currentWp.spite[i];

                int skinIndex = i;
                btnSelect[i].GetComponent<Button>().onClick.RemoveAllListeners();
                btnSelect[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnSelectSkin(skinIndex, currentWp);
                    isCustomMode = false;
                    customColorPanel.SetActive(false);
                });
            }
            else
            {
                btnSelect[i].gameObject.SetActive(false);
            }
        }
    }


    private void SetupCustomColorSystem(Weapon weapon)
    {
        // Tạo các nút chọn material parts
        SetupMaterialSelectionButtons(weapon);

        // Tạo các nút chọn màu
        SetupColorButtons();

        // Setup nút Select
        if (btnSelectCustom != null)
        {
            btnSelectCustom.onClick.RemoveAllListeners();
            btnSelectCustom.onClick.AddListener(OnApplyCustomColor);
        }
    }

    private void SetupMaterialSelectionButtons(Weapon weapon)
    {
        // Xóa các button cũ
        foreach (var btn in materialButtons)
        {
            if (btn != null) Destroy(btn.gameObject);
        }
        materialButtons.Clear();

        // Lấy số lượng material của weapon hiện tại
        GameObject weaponObj = wps[count];
        MeshRenderer meshRenderer = weaponObj.GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;

        Material[] materials = meshRenderer.materials;

        // Tạo button cho từng material
        for (int i = 0; i < materials.Length; i++)
        {
            GameObject btnObj = Instantiate(materialSelectionButtonPrefab, materialSelectionContainer);
            Button btn = btnObj.GetComponent<Button>();

            // Tạo màu preview cho button (dùng màu hiện tại của material)
            Image btnImage = btn.GetComponent<Image>();
            if (btnImage != null && materials[i].HasProperty("_Color"))
            {
                btnImage.color = materials[i].color;
            }

            int matIndex = i;
            btn.onClick.AddListener(() => OnSelectMaterialIndex(matIndex));

            materialButtons.Add(btn);
        }

        // Chọn material đầu tiên mặc định
        selectedMaterialIndex = 0;
        UpdateMaterialButtonSelection();
    }

    private void SetupColorButtons()
    {
        // Xóa các button màu cũ
        foreach (var btn in colorButtons)
        {
            if (btn != null) Destroy(btn.gameObject);
        }
        colorButtons.Clear();

        // Tạo button cho từng màu
        for (int i = 0; i < availableColors.Length; i++)
        {
            GameObject btnObj = Instantiate(colorButtonPrefab, colorButtonContainer);
            Button btn = btnObj.GetComponent<Button>();
            Image btnImage = btn.GetComponent<Image>();

            btnImage.color = availableColors[i];

            Color color = availableColors[i];
            btn.onClick.AddListener(() => OnSelectColor(color));

            colorButtons.Add(btn);
        }
    }

    private void OnSelectCustomMode()
    {
        isCustomMode = true;
        customColorPanel.SetActive(true);

        // Reset về skin gốc khi vào custom mode, NHƯNG KHÔNG GỌI OnSelectSkin
        // vì OnSelectSkin sẽ set isCustomMode = false
        tempSelectedSkin = 0;

        // Apply skin gốc cho weapon hiển thị
        GameObject currentDisplayWeapon = wps[count];
        ApplySkinToWeapon(currentDisplayWeapon, 0);

        // Nếu weapon đang được trang bị, apply cho currentWeapon luôn
        if (PlayerPrefs.GetInt(LoadWeaponKey, -1) == count && currentWeapon != null)
        {
            ApplySkinToWeapon(currentWeapon, 0);
        }

        // Update button text
        var txt = btnBuyCoin.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
            txt.text = "    SELECT";

        Debug.Log("Đã vào custom mode cho weapon: " + weaponData.GetWeapon(count).nameWepon);
    }

    private void OnSelectMaterialIndex(int matIndex)
    {
        selectedMaterialIndex = matIndex;
        Debug.Log("Đã chọn material index: " + matIndex);
        UpdateMaterialButtonSelection();

        // Preview màu đã chọn lên material này
        PreviewCustomColor();
    }

    private void OnSelectColor(Color color)
    {
        selectedCustomColor = color;
        Debug.Log("Đã chọn màu: " + color);
        PreviewCustomColor();
    }

    private void PreviewCustomColor()
    {
        if (!isCustomMode) return;

        // CHỈ ÁP DỤNG CHO WEAPON HIỂN THỊ Ở GIỮA (PREVIEW)
        GameObject weaponObj = wps[count];
        ApplyCustomColorToWeapon(weaponObj, selectedMaterialIndex, selectedCustomColor);

        Debug.Log($"Preview custom color: Material {selectedMaterialIndex}, Color {selectedCustomColor} (chỉ hiển thị)");
    }

    // Hàm helper để áp dụng custom color cho một weapon cụ thể
    private void ApplyCustomColorToWeapon(GameObject weaponObj, int materialIndex, Color color)
    {
        if (weaponObj == null) return;

        MeshRenderer meshRenderer = weaponObj.GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;

        Material[] materials = meshRenderer.materials;
        if (materialIndex < materials.Length)
        {
            // Tạo instance mới của material để không ảnh hưởng đến prefab gốc
            Material newMat = new Material(materials[materialIndex]);
            newMat.color = color;
            materials[materialIndex] = newMat;
            meshRenderer.materials = materials;
        }
    }

    private void OnApplyCustomColor()
    {
        if (!isCustomMode) return;

        // Lưu TẤT CẢ custom colors từ weapon hiển thị
        GameObject weaponObj = wps[count];
        MeshRenderer displayRenderer = weaponObj.GetComponent<MeshRenderer>();
        if (displayRenderer != null)
        {
            Material[] displayMaterials = displayRenderer.materials;
            for (int i = 0; i < displayMaterials.Length; i++)
            {
                // Lưu màu của từng material
                if (displayMaterials[i].HasProperty("_Color"))
                {
                    string customColorKey = "CustomColor_" + count + "_" + i;
                    PlayerPrefs.SetString(customColorKey, ColorUtility.ToHtmlStringRGBA(displayMaterials[i].color));
                }
            }
        }

        // Lưu flag custom mode cho weapon này
        PlayerPrefs.SetInt("IsCustomMode_" + count, 1);

        // Lưu weapon hiện tại là weapon được trang bị
        PlayerPrefs.SetInt(LoadWeaponKey, count);
        PlayerPrefs.Save();

        // BÂY GIỜ MỚI ÁP DỤNG CHO CURRENT WEAPON
        EquipWeapon(count);
        ApplyCustomColorToEquippedWeapon();

        // Ẩn panel custom và reset mode
        customColorPanel.SetActive(false);
        isCustomMode = false;

        UpdateBuyBtnUI();
        Debug.Log("Đã apply custom color cho currentWeapon: " + weaponData.GetWeapon(count).nameWepon);
    }

    private void ApplyCustomColorToEquippedWeapon()
    {
        if (currentWeapon == null) return;

        MeshRenderer meshRenderer = currentWeapon.GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;

        // Load và apply tất cả custom colors đã lưu cho weapon này
        Material[] materials = meshRenderer.materials;
        bool hasCustomColors = false;

        for (int i = 0; i < materials.Length; i++)
        {
            string colorKey = "CustomColor_" + count + "_" + i;
            string colorHtml = PlayerPrefs.GetString(colorKey, "");

            if (!string.IsNullOrEmpty(colorHtml))
            {
                Color customColor;
                if (ColorUtility.TryParseHtmlString("#" + colorHtml, out customColor))
                {
                    Material newMat = new Material(materials[i]);
                    newMat.color = customColor;
                    materials[i] = newMat;
                    hasCustomColors = true;
                }
            }
        }

        if (hasCustomColors)
        {
            meshRenderer.materials = materials;
        }
    }

    private void UpdateMaterialButtonSelection()
    {
        for (int i = 0; i < materialButtons.Count; i++)
        {
            // Add visual feedback cho button được chọn
            Transform outline = materialButtons[i].transform.Find("Outline");
            if (outline != null)
            {
                outline.gameObject.SetActive(i == selectedMaterialIndex);
            }
        }
    }

    // Override hàm SetMaterial để xử lý custom mode
    public void SetMaterial()
    {
        Weapon wp = weaponData.GetWeapon(count);
        if (ownedSet.Contains(count))
        {
            bool isCustom = PlayerPrefs.GetInt("IsCustomMode_" + count, 0) == 1;

            if (isCustom)
            {
                ApplyCustomColorToEquippedWeapon();
            }
            else
            {
                // Code cũ cho skin materials - ÁP DỤNG CHO CURRENT WEAPON
                int indMaterial = PlayerPrefs.GetInt("MaterialOfWp" + count);
                if (currentWeapon != null)
                {
                    MeshRenderer meshRenderer = currentWeapon.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        Material[] mats = meshRenderer.materials;
                        int max = Mathf.Min(mats.Length,
                            weaponData.listMaterials[count].materialOfHammer[indMaterial].materials.Length);

                        for (int i = 0; i < max; i++)
                        {
                            mats[i] = weaponData.listMaterials[count].materialOfHammer[indMaterial].materials[i];
                        }
                        meshRenderer.materials = mats;
                    }
                }
            }
        }
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
            // ÁP DỤNG CHO CẢ WEAPON HIỂN THỊ VÀ CURRENT WEAPON
            ApplySkinToWeapon(currentDisplayWeapon, spriteIndex);

            // Nếu weapon hiện tại đang được trang bị, apply skin cho nó luôn
            if (PlayerPrefs.GetInt(LoadWeaponKey, -1) == count && currentWeapon != null)
            {
                ApplySkinToWeapon(currentWeapon, spriteIndex);
            }

            Debug.Log($"Đã thay đổi material skin {spriteIndex} cho weapon {weapon.nameWepon}");
        }

        // Lưu tạm index skin đã chọn
        tempSelectedSkin = spriteIndex;

        // KIỂM TRA NẾU ĐÂY LÀ SKIN CUSTOM (NÚT CUỐI CÙNG)
        int lastIndex = btnSelect.Length - 1;
        if (spriteIndex == lastIndex)
        {
            // Đây là skin custom, hiện bảng màu
            isCustomMode = true;
            customColorPanel.SetActive(true);
            PlayerPrefs.SetInt("IsCustomMode_" + count, 1);

            Debug.Log("Đã chọn skin custom, hiện bảng màu");
        }
        else
        {
            // Đây là skin thường, ẩn bảng màu và tắt custom mode
            isCustomMode = false;
            customColorPanel.SetActive(false);
            PlayerPrefs.SetInt("IsCustomMode_" + count, 0);

            Debug.Log("Đã chọn skin thường, ẩn bảng màu");
        }

        // Cập nhật UI button
        var txt = btnBuyCoin.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
            txt.text = "    SELECT";

        // Cập nhật trạng thái các button skin
        SetButtonMaterial(spriteIndex);
    }

    // Hàm helper để áp dụng skin cho một weapon cụ thể
    private void ApplySkinToWeapon(GameObject weaponObj, int skinIndex)
    {
        if (weaponObj == null) return;

        MeshRenderer meshRenderer = weaponObj.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("Không tìm thấy MeshRenderer trên weapon: " + weaponObj.name);
            return;
        }

        // Kiểm tra xem có đủ material cho skin này không
        if (skinIndex >= weaponData.listMaterials[count].materialOfHammer.Length)
        {
            Debug.LogError("Skin index vượt quá số lượng material có sẵn!");
            return;
        }

        // Lấy material array tương ứng với skin được chọn
        Material[] newMaterials = weaponData.listMaterials[count].materialOfHammer[skinIndex].materials;

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

            if (isCustomMode)
            {
                // Nếu đang ở custom mode, không lưu MaterialOfWp
                PlayerPrefs.SetInt("IsCustomMode_" + count, 1);
            }
            else
            {
                // Nếu chọn skin bình thường, lưu skin index và tắt custom mode
                PlayerPrefs.SetInt("MaterialOfWp" + count, tempSelectedSkin);
                PlayerPrefs.SetInt("IsCustomMode_" + count, 0);
            }

            PlayerPrefs.Save();

            // ĐỔI WEAPON PREFAB VÀ ÁP DỤNG MATERIAL
            EquipWeapon(count);    // đổi weapon prefab
            SetMaterial();         // áp dụng material thật cho currentWeapon

            if (!isCustomMode)
            {
                SetButtonMaterial(tempSelectedSkin); // đổi nút sang trạng thái equip
            }

            UpdateBuyBtnUI();
            Debug.Log("Đã trang bị weapon: " + weaponData.GetWeapon(count).nameWepon);
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

            if (isCustomMode)
            {
                PlayerPrefs.SetInt("IsCustomMode_" + count, 1);
            }
            else
            {
                PlayerPrefs.SetInt("MaterialOfWp" + count, tempSelectedSkin);
                PlayerPrefs.SetInt("IsCustomMode_" + count, 0);
            }

            PlayerPrefs.Save();

            EquipWeapon(count);
            SetMaterial();

            if (!isCustomMode)
            {
                SetButtonMaterial(tempSelectedSkin);
            }

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