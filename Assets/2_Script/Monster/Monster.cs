using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class Monster : MonoBehaviour
{
    public float power;
    public Image hpBar;
    public float defence;
    public int monsterType;
    public Animator animator;
    public GameObject target;
    public MonsterSkill skill;
    public AudioSource attackSound;
    public GameManager gameManager;
    PhotonView PV;
    List<Collider2D> colliders = new List<Collider2D>();

    // 보스(Golem)인 경우 초기 설정 세팅.
    void Start()
    {
        if (monsterType == 2)
        {
            PV = GetComponent<PhotonView>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            StartCoroutine(Attack2());
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    // 일반 Monster인 경우 플레이어를 추적.
    void Update()
    {
        if (monsterType <= 1)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (target.GetComponent<PlayerObject>() != null && target.GetComponent<PlayerObject>().hpBar.fillAmount <= 0)
                Destroy(gameObject);

            if (distance >= 10f && hpBar.fillAmount > 0)
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 2 * Time.deltaTime);

            if (transform.position.x <= -77)
                transform.position = new Vector3(-76, 5, -1);
            else if (transform.position.x >= 75)
                transform.position = new Vector3(74, 5, -1);
        } 
    }

    // 일반 Monster 공격 로직.
    public IEnumerator Attack1()
    {
        while (hpBar.fillAmount > 0)
        {
            yield return new WaitForSeconds(2f);
            animator.SetBool("Attack", true);

            if (monsterType == 0)
                yield return new WaitForSeconds(0.6f);
            else if (monsterType == 1)
                yield return new WaitForSeconds(1.5f);

            MonsterSkill _skill = Instantiate(skill);
            _skill.transform.position = transform.position + new Vector3(0, 1, 0);
            _skill.power = power;
            _skill.targetVec = (target.transform.position - _skill.transform.position).normalized;
            attackSound.Play();

            yield return new WaitForSeconds(0.8f);
            animator.SetBool("Attack", false);
        }
    }

    // 보스 Monster 공격 로직.
    public IEnumerator Attack2()
    {
        StartCoroutine(BossAttackSound());

        while (hpBar.fillAmount > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int []randX = new int[30];
                int []randY = new int[30];
                for (int i = 0; i < 30; i++)
                {
                    randX[i] = UnityEngine.Random.Range(-71, 76);
                    randY[i] = UnityEngine.Random.Range(20, 25);
                }

                PV.RPC("AttackBossMonster", RpcTarget.All, randX, randY);
            }

            yield return new WaitForSeconds(2f);
        }
    }

    // 보스 Monster 공격 Sound Play.
    IEnumerator BossAttackSound()
    {
        yield return new WaitForSeconds(0.3f);
        attackSound.Play();

        while (hpBar.fillAmount > 0)
        {
            yield return new WaitForSeconds(1.28f);
            attackSound.Play();
        }
    }

    // 피격 로직.
    IEnumerator Hit(Collider2D other)
    {
        if (monsterType <= 1)
            animator.SetBool("Hit", true);

        float damage = 10 / defence;
        hpBar.fillAmount -= damage / 200;

        if (hpBar.fillAmount <= 0)
            StartCoroutine(Die());

        yield return new WaitForSeconds(0.8f);
        if (monsterType <= 1)
            animator.SetBool("Hit", false);
    }

    // 사망 로직.
    IEnumerator Die()
    {
        animator.SetBool("Die", true);

        if (monsterType == 0)
            target.GetComponent<PlayerObject>().money += 150;
        else if (monsterType == 1)
            target.GetComponent<PlayerObject>().money += 200;
        else if (monsterType == 2)
            gameManager.GameWin();
        else if (monsterType == -1)
            target.GetComponent<TutorialPlayerObject>().money += 150;

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    // 미사일 공격에 접촉 시.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Explosion"))
        {
            for (int i = 0; i < colliders.Count; i++) 
            {
                if (colliders[i] == other)
                    return;
            }

            colliders.Add(other);
            StartCoroutine(Hit(other));
        }
    }

    // 보스 Monster의 돌떨구기 공격 동기화.
    [PunRPC]
    public void AttackBossMonster(int[] randX, int[] randY)
    {
        for (int i = 0; i < 30; i++)
        {
            MonsterSkill _skill = Instantiate(skill, new Vector3(randX[i], randY[i], 0), Quaternion.identity).GetComponent<MonsterSkill>();
            _skill.power = power;
        }
    }

}
