using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemType;
    public BoxCollider2D boxCollider;
    public GameObject parachute;

    // 플레이어와 충돌 시 제거.
    void OnCollisionEnter2D(Collision2D other) 
    {
        parachute.SetActive(false);

        if (other.collider.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerObject>().GetItem(itemType);
            ObjectPoolManager.Instance.Destroy(gameObject);
        }

        if (other.collider.CompareTag("TutorialPlayer"))
        {
            other.gameObject.GetComponent<TutorialPlayerObject>().GetItem(itemType);
            Destroy(gameObject);
        }
    }

    // 플레이어와 충돌 시 제거.
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerObject>().GetItem(itemType);
            ObjectPoolManager.Instance.Destroy(gameObject);
        }

        if (other.CompareTag("TutorialPlayer"))
        {
            other.GetComponent<TutorialPlayerObject>().GetItem(itemType);
            Destroy(gameObject);
        }
    }
    
    // 생성 이후 일정시간 경과 시 제거.
    public IEnumerator DestroyItem()
    {
        yield return new WaitForSeconds(6f);
        boxCollider.isTrigger = false;

        yield return new WaitForSeconds(24f);
        ObjectPoolManager.Instance.Destroy(gameObject);
    }
    
}
