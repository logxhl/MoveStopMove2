using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameTag : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public TMP_Text nameText;
    public TMP_Text coinText;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
            transform.position = screenPos;
        }
    }
    public void SetNameAndCoin(string name, int coins)
    {
        nameText.text = name;
        coinText.text = coins.ToString();
    }
}
