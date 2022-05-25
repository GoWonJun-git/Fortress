using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class PhotonObject : MonoBehaviourPunCallbacks
{
    public string roomType;
    public Lobby lobbyManager;
    public GameManager gameManager;

    public bool networkProblem;

    void Start() 
    {
        Application.targetFrameRate = 40;
        Application.runInBackground = true;
        Screen.SetResolution(960, 540, false);

        PhotonNetwork.ConnectUsingSettings();
    }

#region CONNECT
    // PVP 입장 시도.
    public void JoinRandomOrCreateRoom_PVP()
    {
        roomType = "PVP";
        PhotonNetwork.JoinOrCreateRoom("PVP", new RoomOptions { MaxPlayers = 4 }, null);
    }

    // PVE 입장 시도.
    public void JoinRandomOrCreateRoom_PVE()
    {
        roomType = "PVE";
        PhotonNetwork.JoinOrCreateRoom("PVE", new RoomOptions { MaxPlayers = 4 }, null);
    }

    // 방 나가기 시도.
    public void OutRoom() => PhotonNetwork.LeaveRoom();
    

    // 서버 접속 시.
    public override void OnConnectedToMaster() 
    {
        SceneManager.LoadScene(1);
        PhotonNetwork.UseRpcMonoBehaviourCache = true;
    }

    // 방 참가 시.
    public override void OnJoinedRoom() => lobbyManager.waitStartPanle.SetActive(true);
    
#endregion

}