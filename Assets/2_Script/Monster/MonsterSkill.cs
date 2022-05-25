using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkill : MonoBehaviour
{
    public Vector3 targetVec;
    public float speed;
    public float power;
    public GameObject skillEffect;

    void Start() => StartCoroutine(DestroyObject());

    // 특정 방향(플레이어)으로 이동.
    void Update() => transform.Translate(targetVec.normalized * speed * Time.deltaTime);
    
    // 생성 후 일정 시간이 경과되면 제거.
    public IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    // 플레이어나 지형에 접촉 시 제거.
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") || other.CompareTag("TutorialPlayer") || other.CompareTag("Ground"))
        {
            Destroy(Instantiate(skillEffect, transform.position, Quaternion.identity), 1f);
            Destroy(gameObject);
        }

        if (other.CompareTag("Ground") && other.gameObject.GetComponent<TerrainDestory>() != null)
        {
            other.gameObject.GetComponent<TerrainDestory>().DestroyTerrain(transform.position, 2f);
            Destroy(gameObject);
        }
    }

}
