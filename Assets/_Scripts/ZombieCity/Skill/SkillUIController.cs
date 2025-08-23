using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIController : MonoBehaviour
{
    [Header("SkillData")]
    public SkillData skillData;

    [Header("UI")]
    public Image skillImg;
    public TextMeshProUGUI skillNameText;
    public Button selectBtn;
    public Button shuffleBtn;

    private Skill currentSkill;
    public GameObject skillPanel;
    public GameObject joyStick;
    private void Start()
    {
        LoadRandomSkill();
        selectBtn.onClick.AddListener(OnSelectSkill);
        shuffleBtn.onClick.AddListener(LoadRandomSkill);
    }
    public void LoadRandomSkill()
    {
        Skill newSkill;
        do
        {
            int rand = Random.Range(0, skillData.skills.Length);
            newSkill = skillData.skills[rand];
        } while (newSkill == currentSkill && skillData.skills.Length > 1);

        currentSkill = newSkill;
        skillImg.sprite = currentSkill.spriteSkill;
        skillNameText.text = currentSkill.skillName;
    }
    public void OnSelectSkill()
    {
        Debug.Log("Select skill: " + currentSkill.skillName);
        PlayerSceneZombie.instance.SetSkill(currentSkill);
        skillPanel.SetActive(false);
        joyStick.SetActive(true);
        Time.timeScale = 1.0f;
    }
}
