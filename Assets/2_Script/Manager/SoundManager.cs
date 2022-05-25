using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider soundSlider;
    public GameObject VolumeZeroImage;
    public AudioSource BGM;
    public AudioSource move;
    public AudioSource jump;
    public AudioSource shoot;
    public AudioSource itemGet;
    public AudioSource buttonFail;
    public AudioSource buttonTouch;
    public AudioSource monsterSkill;

    // 로비에서 음량 버튼 조절 시.
    public void SoundValueChange_Lobby()
    {
        BGM.volume = soundSlider.value * 0.2f;
        buttonFail.volume = soundSlider.value;
        buttonTouch.volume = soundSlider.value;
    }

    // 인게임 내에서 음량 버튼 조절 시.
    public void SoundValueChange_InGame()
    {
        BGM.volume = soundSlider.value * 0.2f;
        move.volume = soundSlider.value;
        jump.volume = soundSlider.value;
        shoot.volume = soundSlider.value;
        itemGet.volume = soundSlider.value;
        buttonFail.volume = soundSlider.value;
        buttonTouch.volume = soundSlider.value;
        monsterSkill.volume = soundSlider.value;
    }

    public void SoundButtonClick()
    {
        if (soundSlider.value > 0)
        {   
            soundSlider.value = 0;
            VolumeZeroImage.SetActive(true);
        }
        else 
        {
            soundSlider.value = 0.5f;
            VolumeZeroImage.SetActive(false);
        }
        
        buttonTouch.Play();
        BGM.volume = soundSlider.value * 0.2f;
        move.volume = soundSlider.value;
        jump.volume = soundSlider.value;
        shoot.volume = soundSlider.value;
        itemGet.volume = soundSlider.value;
        buttonFail.volume = soundSlider.value;
        buttonTouch.volume = soundSlider.value;
        monsterSkill.volume = soundSlider.value;
    }
}
