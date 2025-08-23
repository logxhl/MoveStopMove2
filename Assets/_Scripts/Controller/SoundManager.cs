using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Button soundBtn;
    public Image soundIcon;
    public Sprite soundOn;
    public Sprite soundOff;

    private bool isMuted = false;
    private void Start()
    {
        soundBtn.onClick.AddListener(ToggleSound);
        UpdateBtnIcon();
    }

    private void UpdateBtnIcon()
    {
        if(soundIcon != null)
        {
            soundIcon.sprite = isMuted ? soundOff : soundOn;
        }
    }

    private void ToggleSound()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0 : 1;
        UpdateBtnIcon();
    }
}
