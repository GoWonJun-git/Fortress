using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;

public class Lobby : MonoBehaviour
{
    public PhotonView PV;
    private PhotonObject photonObject;
    public GameObject optionPanel;
    public GameObject problemPanel;
    public SoundManager soundManager;

    [Header("UI")]
    public GameObject waitStartPanle;
    public Text memberText;
    public GameObject startButton;
    public GameObject networkProblemPanel;

    void Start() 
    {
        photonObject = GameObject.Find("PhotonObject").GetComponent<PhotonObject>();
        photonObject.lobbyManager = this;

        if (photonObject.networkProblem)
            networkProblemPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
            optionPanel.SetActive(!optionPanel.activeSelf);

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient && !startButton.activeSelf)
                startButton.SetActive(true);
            if (!PhotonNetwork.IsMasterClient && startButton.activeSelf)
                startButton.SetActive(false);

            memberText.text = PhotonNetwork.CurrentRoom.PlayerCount + " / 4";
        }
    }

    // PVP 모드 입장 시.
    public void PVPButtonClick() => photonObject.JoinRandomOrCreateRoom_PVP();

    // PVE 모드 입장 시.
    public void PVEButtonClick() => photonObject.JoinRandomOrCreateRoom_PVE();

    // Tutorial 입장 시.
    public void TutorialButtonClick() => SceneManager.LoadScene(4);

    // 버튼 및 판넬 활성화 여부 변경.
    public void ChangeActive(GameObject disableObject) 
    {
        soundManager.buttonTouch.Play();
        disableObject.SetActive(!disableObject.activeSelf);

        if (disableObject.name.Equals("WaitStartPanel"))
            photonObject.OutRoom();
    }

    public void GameStart()
    {
        PV.RPC("GameStart_RPC", RpcTarget.All);
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    [PunRPC]
    public void GameStart_RPC()
    {
        if (photonObject.roomType.Equals("PVP"))
            SceneManager.LoadScene(2);
        else if (photonObject.roomType.Equals("PVE"))
            SceneManager.LoadScene(3);
    }

    // 게임 종료.
    public void GameExit() => Application.Quit();
}
