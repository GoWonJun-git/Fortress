using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDataManager : MonoBehaviour
{
    static private bool initState = false;
    static public GameObject Bullet_1;

    static public GameObject AttackItem;
    static public GameObject ShieldItem;
    static public GameObject MoneyItem;

    static public GameObject Dragon;
    static public GameObject EvilMage;
    static public GameObject Golem;


    public static void LoadResourcesData()
    {
        if(!initState)
        {
            initState = true;

            Bullet_1 = Resources.Load("Player/Bullet/Bullet_1") as GameObject;

            AttackItem = Resources.Load("Item/AttackItem") as GameObject;
            ShieldItem = Resources.Load("Item/ShieldItem") as GameObject;
            MoneyItem = Resources.Load("Item/MoneyItem") as GameObject;

            Dragon = Resources.Load("Monster/Dragon") as GameObject;
            EvilMage = Resources.Load("Monster/EvilMage") as GameObject;
            Golem = Resources.Load("Monster/Golem") as GameObject;
        }
    }

    public static T CreateObjectAndComponent<T>(GameObject resource, Vector3 position, Quaternion rotate)
    {
        GameObject obj = ObjectPoolManager.Instance.Instantiate(resource, position, rotate);
        T script = obj.GetComponent<T>();

        return script;
    }

    public static T CreateObjectAndComponent<T>(GameObject resource, Vector3 position)
    {
        return CreateObjectAndComponent<T>(resource, position, Quaternion.identity);
    }

    public static T CreateObjectAndComponent<T>(GameObject resource)
    {
        return CreateObjectAndComponent<T>(resource, Vector3.zero, Quaternion.identity);
    }

}