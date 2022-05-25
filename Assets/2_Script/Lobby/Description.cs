using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Description : MonoBehaviour
{
    public GameObject descriptionPanel;
    public GameObject[] panels;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public int panelNumber;
    public SoundManager soundManager;

    // 설명창 순서 변경.
    public void ChangePanelValue(int check)
    {
        if (check == 0)
        {
            if (panelNumber == 0)
                return;

            panels[panelNumber--].SetActive(false);
            panels[panelNumber].SetActive(true);
        }
        else
        {
            if (panelNumber >= panels.Length - 1)
                return;

            panels[panelNumber++].SetActive(false);
            panels[panelNumber].SetActive(true);

        }

        soundManager.buttonTouch.Play();
    }



}
