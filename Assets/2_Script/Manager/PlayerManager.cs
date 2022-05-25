using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : GameSingleton<PlayerManager>
{
    public PhotonView PV;
    public GameManager gameManager;
    public List<PlayerObject> listPlayerObjects = new List<PlayerObject>();
    public PlayerObject myPlayerObject;
    public GameObject playerDieEffect;

    // 플레이어 캐릭터 생성.
    public IEnumerator CreatePlayerObject() 
    {
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Instantiate("Player/Player", new Vector3(0, 25, 0), Quaternion.identity);
    }

    // 플레이어 리스트 Null값 구분.
    void Update()
    {
        for (int i = 0; i < listPlayerObjects.Count; i++)
        {
            if (listPlayerObjects[i] == null)
                listPlayerObjects.RemoveAt(i);
        }
    }

    // 캐릭터를 리스트에 저장.
    public void AddPlayerObject(PlayerObject playerObject, bool isMine)
    {
        if (isMine)
            myPlayerObject = playerObject;
        
        if (!listPlayerObjects.Contains(playerObject))
            listPlayerObjects.Add(playerObject);
    }

    // 캐릭터를 리스트에서 제거 -> 게임 종료 로직 호출.
    public void RemovePlayerObject(PlayerObject playerObject, bool isMine) 
    {
        listPlayerObjects.Remove(playerObject);
        Destroy(Instantiate(playerDieEffect, playerObject.transform.position, Quaternion.identity), 5f);

        if (isMine)
        {
            myPlayerObject = null;
            gameManager.StartCoroutine(gameManager.PlayerDie());
        }
    }

}