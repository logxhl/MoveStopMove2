using UnityEngine;
using UnityEngine.UI;

public class PantsShopUI : MonoBehaviour
{
    [SerializeField] private PantsChange pantsChange;
    [SerializeField] private Transform buttonParent; // Nơi chứa các Button quần

    private const string KEY = "SelectedPant";

    private void Start()
    {
        for (int i = 0; i < buttonParent.childCount; i++)
        {
            int index = i; // tránh lỗi delegate
            buttonParent.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                pantsChange.ChangePants(index);
                PlayerPrefs.SetInt(KEY, index);
                PlayerPrefs.Save();
                Debug.Log("Saved Pant Index: " + index);
            });
        }
    }
}
