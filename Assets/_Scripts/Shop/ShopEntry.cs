using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class ShopEntry
{
    public string id;                 // ví dụ: "hat_01"
    public Button tileButton;         // nút item trong grid
    public GameObject previewObject;  // object để gắn lên player
    public Image lockIcon;            // icon khóa (nếu có)
}
