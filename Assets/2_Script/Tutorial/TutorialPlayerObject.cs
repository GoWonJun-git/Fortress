using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlayerObject : MonoBehaviour
{
    [Header("PlayerComponent")]
    public new Rigidbody2D rigidbody;
    public SpriteRenderer spriteRenderer;

    [Header("Manager")]
    public SoundManager playerSound;
    CameraManager playerCamera;
    PlayerManager playerManager;
    TutorialManager tutorialManager;
    ParticleManager particleManager;
    PlayerController_Tutorial playerController;

    [Header("Attack")]
    public float deg_Attack;
    public GameObject bullet;
    public GameObject turret;

    [Header("Moving")]
    public float speed;
    public int jumpCount;
    public bool isMoving;
    public float deg_Move;

    [Header("UI")]
    public Image hpBar;
    public Image chargeBar;
    public Image chargeArea;
    public Image freezingImage;

    [Header("State")]
    public Shield shield;
    private int attackItem;
    private bool shieldItem;
    public bool isCharging;
    private bool chargeState;

    [Header("Effect")]
    public GameObject potionEffect;
    public GameObject itemRootingEffect;

    [Header("Info")]
    public int money;
    public float defence;
    public int characterType;

    // 플레이어 캐릭터 초기 설정 세팅.
    void Start()
    {   
        tutorialManager  = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        playerSound      = tutorialManager.soundManager;
        playerCamera     = tutorialManager.playerCamera;
        particleManager  = tutorialManager.particleManager;
        playerController = tutorialManager.playerController;

        characterType = 0;
        playerController.moveBarHandleImage.sprite = spriteRenderer.sprite;
        turret.SetActive(true);
    }

    // 이동 함수 호출 + 이동 범위 제한.
    void FixedUpdate()
    {
        Move();
        Move_Turret();
        Charge();

        if (transform.position.x <= playerCamera.minX + 1)
            transform.position = new Vector3(playerCamera.minX + 1, transform.position.y, 0);
        else if (transform.position.x >= playerCamera.maxX - 2)
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
            transform.position = new Vector3(playerCamera.maxX - 2.5f, transform.position.y, 0);
        }
        else if (transform.position.y >= 19)
             transform.position = new Vector3(transform.position.x, 19, 0);

        RaycastHit2D[] rayHitDown = Physics2D.RaycastAll(transform.position, Vector3.down, 2f);
        for (int i = 0; i < rayHitDown.Length; i++)
        {
            if (rayHitDown[i].collider != null && rayHitDown[i].collider.gameObject != gameObject && rigidbody.velocity.y <= 0)
                jumpCount = 0;
        }
    }

#region CONTROL
    // 좌우 이동.
    public void Move()
    {
        if (!isMoving)
            return;

        transform.position += new Vector3(0, 0.0001f, 0);

        if (deg_Move < 0)
        {
            if (!spriteRenderer.flipX && !isCharging)
                ChangeFlipX();
                
            if (transform.position.x >= 0f)
                rigidbody.AddForce(Vector2.left * speed / 20, ForceMode2D.Impulse);
            else
                rigidbody.velocity = new Vector2(-1.5f * speed, rigidbody.velocity.y);
        }
        else if (deg_Move > 0)
        {
            if (spriteRenderer.flipX && !isCharging)
                ChangeFlipX();

            if (transform.position.x >= 0f)
                rigidbody.AddForce(Vector2.right * speed / 20, ForceMode2D.Impulse);
            else
                rigidbody.velocity = new Vector2(1.5f * speed, rigidbody.velocity.y);
        }
    }

    // 점프.
    public void Jump()
    {
        if ( (jumpCount >= 1 && speed <= 8) || jumpCount >= 2)
            return;

        jumpCount++;
        playerSound.jump.Play();
        rigidbody.velocity = new Vector2(rigidbody.velocity.x * 0.75f, 0f);

        float distance1 = Vector2.Distance(new Vector2(particleManager.particle_Up1.transform.position.x, 0), new Vector2(transform.position.x, 0));
        float distance2 = Vector2.Distance(new Vector2(particleManager.particle_Up2.transform.position.x, 0), new Vector2(transform.position.x, 0));
        
        if ( (distance1 <= 3 || distance2 <= 3) && transform.position.y <= 9f)
            rigidbody.AddForce(Vector2.up * 50f, ForceMode2D.Impulse);
        else
            rigidbody.AddForce(Vector2.up * 30f, ForceMode2D.Impulse);

    }

    // 발사 각도 조정.
    public void Move_Turret()
    {
        if (isCharging)
        {
            float rad = deg_Attack * Mathf.Deg2Rad;
            turret.transform.localPosition = new Vector2(spriteRenderer.flipX ? Mathf.Cos(rad) * -1 : Mathf.Cos(rad), Mathf.Sin(rad));
            turret.transform.eulerAngles = new Vector3(0, turret.transform.eulerAngles.y, deg_Attack);
        }
    }

    // 공격 거리 조절.
    public void Charge()
    {
        if (!isCharging)
            return;

        if (chargeBar.fillAmount <= 0.5f)
            chargeState = true;
        else if (chargeBar.fillAmount == 1)
            chargeState = false;

        if (chargeState)
            chargeBar.fillAmount += Time.fixedDeltaTime * 1.5f;
        else
            chargeBar.fillAmount -= Time.fixedDeltaTime * 1.5f;
    }

    // 미사일 발사.
    public void Shooting()
    {
        isCharging = false;
        playerSound.shoot.Play();

        for (int i = 0; i <= attackItem; i++)
        {
            for (int j = 0; j < turret.transform.childCount; j++)
            {
                if (turret.transform.GetChild(j).gameObject.activeSelf)
                {
                    Transform arrow = turret.transform.GetChild(1);
                    Bullet _bullet = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.Bullet_1, arrow.position, Quaternion.identity).GetComponent<Bullet>();
                    _bullet.particleManager = particleManager;
                    _bullet.speed = _bullet.startingSpeed * chargeBar.fillAmount;
                    _bullet.bulletRigidbody.velocity = new Vector3(turret.transform.localPosition.x, turret.transform.localPosition.y + arrow.localPosition.y, 0) * _bullet.speed;
                    _bullet.StartCoroutine(_bullet.DestroyBullet());

                    if (j == 1)
                        playerCamera.ChangeTarget(_bullet.gameObject);
                }
            }
        }

        deg_Attack = 0;
        turret.SetActive(false);
        chargeBar.gameObject.SetActive(false);
        chargeArea.gameObject.SetActive(false);
        chargeBar.fillAmount = 0;
        attackItem = 0;
    }

    // 좌우 반전.
    public void ChangeFlipX()
    {
        rigidbody.velocity = new Vector2(0f, rigidbody.velocity.y);

        turret.transform.localPosition = new Vector2(turret.transform.localPosition.x * -1, turret.transform.localPosition.y);
        turret.transform.rotation = Quaternion.Euler(0, turret.transform.eulerAngles.y + 180, turret.transform.eulerAngles.z);
        spriteRenderer.flipX = !spriteRenderer.flipX;

        chargeArea.transform.localPosition = new Vector2(chargeArea.transform.localPosition.x * -1, chargeArea.transform.localPosition.y);
        chargeBar.transform.localPosition = new Vector2(chargeBar.transform.localPosition.x * -1, chargeBar.transform.localPosition.y);
    }
#endregion

#region SHOP
    // 터치한 UpGrade와 등급에 맞춰 함수 호출.
    public void UpGrade(int upgradeType, int upgradeLevel)
    {
        switch (upgradeType)
        {
            case 0:
                UpGradeType0(upgradeLevel);
                break;
            case 1:
                UpGradeType1(upgradeLevel);
                break;
            case 2:
                UpGradeType2();
                break;
            case 3:
                UpGradeType3(0.3f);
                break;
        }
    }

    // 탱크 강화 RPC.
    public void UpGradeType0(int upgradeLevel)
    {
        spriteRenderer.sprite = tutorialManager.shopManager.tankImages[upgradeLevel + 1];
        transform.localScale += new Vector3(0.2f, 0.2f, 0);
        defence += 1f;
        speed += 0.5f;
    }

    // 공격 강화 RPC.
    public void UpGradeType1(int upgradeLevel)
    {
        if (upgradeLevel == 0)
        {
            turret.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (upgradeLevel == 1)
        {
            turret.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    // 기동성 강화 RPC.
    public void UpGradeType2() => speed += 1.5f;

    // 회복 RPC.
    public void UpGradeType3(float healing)
    {
        hpBar.fillAmount += healing;
        Destroy(potionEffect, 2f);
    }
#endregion

#region  ITEM
    // 습득한 ITEM에 맞춰 함수 호출.
    public void GetItem(int itemType)
    {
        switch (itemType)
        {
            case 0:
                ItemType0();
                break;
            case 1:
                ItemType1();
                break;
            case 2:
                ItemType2();
                break;
        }

        playerSound.itemGet.Play();
    }

    // 공격 강화 ITEM.
    public void ItemType0() 
    {
        attackItem++;
        Destroy(Instantiate(itemRootingEffect, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity), 2);
    }

    // Shield ITEM.
    public void ItemType1()
    {
        if (!shieldItem)
        {
            shieldItem = true;
            shield.HP = 1f;
            shield.gameObject.SetActive(true);
        }
        else
            shield.HP = 1f;

        Destroy(Instantiate(itemRootingEffect, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity), 2);
    }

    // Money ITEM.
    public void ItemType2() 
    {
        money += 300;
        Destroy(Instantiate(itemRootingEffect, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity), 2);
    }
#endregion

#region Collision&Trigger
    // 미사일이나 Monster 공격에 접촉시 피격 판정 함수 호출.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (freezingImage.gameObject.activeSelf)
            return;

        if (other.gameObject.CompareTag("Explosion"))
            hit(50f);

        if (other.CompareTag("MonsterSkill"))
        {
            playerSound.monsterSkill.Play();
            hit(15f * other.gameObject.GetComponent<MonsterSkill>().power);
        }
    }

    // 폭포에 접촉중일 시 캐릭터 해상도 조절.
    void OnTriggerStay2D(Collider2D other) 
    {
        if (other.CompareTag("Water"))
            spriteRenderer.color = new Color(1, 1, 1, 0.75f);
    }

    // 폭포에서 떨어질 시 캐릭터 해상도 정상화.
    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Water"))
            spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    // 반딧불이(Light)와 충돌시 회복.
    void OnParticleCollision(GameObject other) 
    {
        if (other.name == "Light")
            UpGradeType3(0.01f);
    }

    // 캐릭터 피격 및 사망 로직.
    public void hit(float damage)
    {
        damage /= defence;
        if (shieldItem)
        {
            shield.HP -= (damage / 25f);

            if (shield.HP <= 0)
            {
                shieldItem = false;
                shield.gameObject.SetActive(false);
            }
            return;
        }

        hpBar.fillAmount -= (damage / 100);
        if (hpBar.fillAmount <= 0)
        {
            transform.position = new Vector3(-40, 2, -1);
            hpBar.fillAmount = 1f;
        }
    }
#endregion

}
