using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class ItemManager : MonoBehaviour
{
    public PhotonView PV;

    public CameraManager cameraManager;
    int []randPositionArray = new int[5];
    int []randTypeArray = new int[5];

    // Item 생성 Packet을 전달.
    public void DropItem()
    {
        for (int i = 0; i < randPositionArray.Length; i++) 
        {
            randPositionArray[i] = UnityEngine.Random.Range((int)cameraManager.minX + 2, (int)cameraManager.maxX - 2);
            randTypeArray[i] = UnityEngine.Random.Range(0, 3);
        }

        PV.RPC("DropItem", RpcTarget.All, randPositionArray, randTypeArray);
    }
    
    // Item 생성 로직.
    [PunRPC]
    public void DropItem(int[] _randPositionArray, int[] _randTypeArray)
    {
        for (int i = 0; i < _randPositionArray.Length; i++) 
        {
            Item item = null;

            if (_randTypeArray[i] == 0)
            {
                item = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.AttackItem, 
                        new Vector3(_randPositionArray[i], 20, 0), Quaternion.identity).GetComponent<Item>();
                item.itemType = 0;
            }
            else if (_randTypeArray[i] == 1)
            {
                item = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.ShieldItem, 
                        new Vector3(_randPositionArray[i], 20, 0), Quaternion.identity).GetComponent<Item>();
                item.itemType = 1;
            }
            else if (_randTypeArray[i] == 2)
            {
                item = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.MoneyItem, 
                        new Vector3(_randPositionArray[i], 20, 0), Quaternion.identity).GetComponent<Item>();
                item.itemType = 2;
            }

            item.transform.localEulerAngles = new Vector3(180, 0, 0);
            item.StartCoroutine(item.DestroyItem());
        }
    }
}
