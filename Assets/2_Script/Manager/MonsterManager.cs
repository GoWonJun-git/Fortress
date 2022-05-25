using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class MonsterManager : MonoBehaviour
{
    public PhotonView PV;

    public PlayerManager playerManager;
    public GameManager gameManager;
    int rand;

    // Monster 생성 Packet을 호출.
    public void CreateMonster(int wave)
    {
        rand = UnityEngine.Random.Range(0, 2);
        PV.RPC("CreateMonster_RPC", RpcTarget.All, wave);
    }

    // Monster 생성 로직. wave에 맞춰서 몬스터를 생성.
    [PunRPC]
    public void CreateMonster_RPC(int wave)
    {
        switch (wave)
        {
            case 2:
                for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
                {
                    int monsterCreateCount = 3;
                    for (int j = 0; j < monsterCreateCount; j++)
                        MonsterSetting(0, monsterCreateCount, playerManager.listPlayerObjects[i].gameObject, i, j);
                }
                break;
            case 3:
                for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
                {
                    int monsterCreateCount = 2;
                    for (int j = 0; j < monsterCreateCount; j++)
                        MonsterSetting(1, monsterCreateCount, playerManager.listPlayerObjects[i].gameObject, i, j);
                }
                break;
            case 4:
                for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
                {
                    int monsterCreateCount = 4;
                    for (int j = 0; j < monsterCreateCount; j++)
                        MonsterSetting(0, monsterCreateCount, playerManager.listPlayerObjects[i].gameObject, i, j);
                }
                break;
            case 5:
                for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
                {
                    int monsterCreateCount = 4;
                    for (int j = 0; j < monsterCreateCount; j++)
                        MonsterSetting(1, monsterCreateCount, playerManager.listPlayerObjects[i].gameObject, i, j);
                }
                break;
            case 6:
                for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
                {
                    int monsterCreateCount = 5;
                    for (int j = 0; j < monsterCreateCount; j++)
                        MonsterSetting(0, monsterCreateCount, playerManager.listPlayerObjects[i].gameObject, i, j);
                }
                break;
            case 7:
                for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
                {
                    int monsterCreateCount = 4;
                    for (int j = 0; j < monsterCreateCount; j++)
                        MonsterSetting(1, monsterCreateCount, playerManager.listPlayerObjects[i].gameObject, i, j);
                }
                break;
            case 8:
                for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
                {
                    int monsterCreateCount = 6;
                    for (int j = 0; j < monsterCreateCount; j++)
                        MonsterSetting(0, monsterCreateCount, playerManager.listPlayerObjects[i].gameObject, i, j);
                }
                break;
            case 9:
                for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
                {
                    int monsterCreateCount = 6;
                    for (int j = 0; j < monsterCreateCount; j++)
                        MonsterSetting(0, monsterCreateCount, playerManager.listPlayerObjects[i].gameObject, i, j);
                    
                    monsterCreateCount = 4;
                    for (int j = 0; j < monsterCreateCount; j++)
                        MonsterSetting(1, monsterCreateCount, playerManager.listPlayerObjects[i].gameObject, i, j);
                }
                break;
            case 10:
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.Instantiate("Monster/Golem", new Vector3(0, 2, -1), Quaternion.identity);
                else
                    return;
                break;
        }
    }

    // Monster 설정.
    void MonsterSetting(int type, int _monsterCreateCount, GameObject _target, int _i, int _j)
    {
        if (type == 0)
        {
            Monster dragon = Instantiate(Resources.Load<Monster>("Monster/Dragon"),
                playerManager.listPlayerObjects[_i].transform.position
                + new Vector3(-14 + (28 / (_monsterCreateCount - 1) * _j),
                (_j < _monsterCreateCount / 2) ? 1 + (_j * 3f) : 4 - ((_j - _monsterCreateCount / 2) * 3f), 0),
                Quaternion.Euler(0, 180, 0));

            dragon.monsterType = type;
            dragon.StartCoroutine(dragon.Attack1());
            dragon.defence += ((float)gameManager.waveCount / 100);
            dragon.target = _target;
        }
        else
        {
            Monster evilMage = Instantiate(Resources.Load<Monster>("Monster/EvilMage"),
                playerManager.listPlayerObjects[_i].transform.position
                + new Vector3(-14 + (28 / (_monsterCreateCount - 1) * _j), 2, 0), Quaternion.Euler(0, 180, 0));

            evilMage.monsterType = type;
            evilMage.StartCoroutine(evilMage.Attack1());
            evilMage.defence += ((float)gameManager.waveCount / 100);
            evilMage.target = _target;
        }
    }

}
