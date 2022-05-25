using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class TextManager : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerManager playerManager;
    public Text waveText;
    public Text startButtomtext;
    public Text waitImagetext;

    // Wave Text 변경.
    void Update() 
    {
        waveText.text = gameManager.waveCount + " Wave - " + PhotonNetwork.CurrentRoom.PlayerCount + "명";
        startButtomtext.text = waitImagetext.text = "(" + PhotonNetwork.CurrentRoom.PlayerCount + " / 4)";
    }
}
