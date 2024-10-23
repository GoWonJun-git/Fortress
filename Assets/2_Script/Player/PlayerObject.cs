using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class PlayerObject : MonoBehaviour
{
    [Header("PlayerComponent")]
    public PhotonView PV;
    public new Rigidbody2D rigidbody;
    public SpriteRenderer spriteRenderer;

    [Header("Manager")]
    public SoundManager playerSound;
    public GameManager gameManager;
    CameraManager playerCamera;
    PlayerManager playerManager;
    ParticleManager particleManager;
    PlayerController playerController;

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
    private string gameMove;

    [Header("Item")]
    public GameObject itemRootingEffect;

    [Header("Info")]
    public int money;
    public float defence;
    public int characterType;
    public Characters characters;

    // 플레이어 캐릭터 초기 설정 세팅.
    void Start()
    {   
        gameManagerSetting();

        if (PV.IsMine)
        {
            StartCoroutine(SettingMyCharacterType());
            PV.RPC("SynchronizationTransform_RPC", RpcTarget.Others, transform.position.x, transform.position.y);
        }
    }

    // 이동 함수 호출, 이동 범위 제한, 위치 동기화.
    void FixedUpdate() 
    {
        Move();
        Move_Turret();
        Charge();

        if (playerCamera == null)
            return;

        if (transform.position.x <= playerCamera.minX + 1)
        {
            transform.position = new Vector3(playerCamera.minX + 1, transform.position.y, 0);
            RaycastHit2D rayHitRight = Physics2D.Raycast(transform.position, Vector3.right, 3f, LayerMask.GetMask("Map"));
            if (rayHitRight.collider != null && rayHitRight.collider.CompareTag("Ground") && rayHitRight.collider.gameObject.GetComponent<TerrainDestory>() != null)
                rayHitRight.collider.gameObject.GetComponent<TerrainDestory>().DestroyTerrain(transform.position + new Vector3(0, 1f, 0), 1.5f);
        }
        else if (transform.position.x >= playerCamera.maxX - 2)
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
            transform.position = new Vector3(playerCamera.maxX - 2.5f, transform.position.y, 0);
            RaycastHit2D rayHitLeft = Physics2D.Raycast(transform.position, Vector3.left, 3f, LayerMask.GetMask("Map"));
            if (rayHitLeft.collider != null && rayHitLeft.collider.CompareTag("Ground") && rayHitLeft.collider.gameObject.GetComponent<TerrainDestory>() != null)
                rayHitLeft.collider.gameObject.GetComponent<TerrainDestory>().DestroyTerrain(transform.position + new Vector3(0, 1f, 0), 1.5f);
        }
        else if (transform.position.y >= 19)
            transform.position = new Vector3(transform.position.x, 19, 0);

        RaycastHit2D[] rayHitDown = Physics2D.RaycastAll(transform.position, Vector3.down, 2f);
        for (int i = 0; i < rayHitDown.Length; i++)
        {
            if (rayHitDown[i].collider != null && rayHitDown[i].collider.gameObject != gameObject && rigidbody.velocity.y <= 0)
                jumpCount = 0;
        }

        if (PV.IsMine && gameManager.isGameStart)
            PV.RPC("SynchronizationTransform_RPC", RpcTarget.Others, transform.position.x, transform.position.y);
    }

/*** 이동 및 공격 함수 모음. ***/
#region CONTROL
    // 이동 RPC 호출.
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
                rigidbody.AddForce(Vector2.left * (speed) / 20, ForceMode2D.Impulse);
            else
                rigidbody.velocity = new Vector2(-1.5f * speed, rigidbody.velocity.y);
        }
        else if (deg_Move > 0)
        {
            if (spriteRenderer.flipX && !isCharging)
                ChangeFlipX();

            if (transform.position.x >= 0f)
                rigidbody.AddForce(Vector2.right * (speed) / 20, ForceMode2D.Impulse);
            else
                rigidbody.velocity = new Vector2(1.5f * speed, rigidbody.velocity.y);
        }
    }

    // 좌우 반전 RPC 호출.
    public void ChangeFlipX() => PV.RPC("SynchronizationFlipX", RpcTarget.All);
    
    // 점프 RPC 호출.
    public void Jump()
    {
        if ( (jumpCount >= 1 && speed < 8) || jumpCount >= 2)
            return;

        jumpCount++;
        playerSound.jump.Play();
        PV.RPC("JumpRPC", RpcTarget.All);
    }

    // 발사 각도 조정 RPC 호출.
    public void Move_Turret()
    {
        if (isCharging)
            PV.RPC("Move_TurretRPC", RpcTarget.All, deg_Attack);
    }

    // 발사 거리 조절 RPC 호출.
    public void Charge()
    {
        if (!isCharging)
            return;

        PV.RPC("ChargeRPC", RpcTarget.All);
    }

    // 미사일 발사 RPC 호출.
    public void Shooting()
    {
        playerSound.shoot.Play();
        PV.RPC("ShootingRPC", RpcTarget.All, chargeBar.fillAmount, PV.ViewID);
    }

    // 빙결(무적)여부 변경 RPC 호출.
    public void ChangeFreezing() => PV.RPC("ChangeFreezingRPC", RpcTarget.All);

    /*** 아래부터는 RPC에 의해 호출됨. ***/
    // 캐릭터 위치 및 좌표 동기화.
    [PunRPC]
    public void SynchronizationTransform_RPC(float x, float y)
    {
        transform.position = new Vector3(x, y, -1);
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    // 좌우 반전 동기화.
    [PunRPC]
    public void SynchronizationFlipX()
    {
        rigidbody.velocity = new Vector2(0f, rigidbody.velocity.y);

        turret.transform.localPosition = new Vector2(turret.transform.localPosition.x * -1, turret.transform.localPosition.y);
        turret.transform.rotation = Quaternion.Euler(0, turret.transform.eulerAngles.y + 180, turret.transform.eulerAngles.z);
        spriteRenderer.flipX = !spriteRenderer.flipX;

        chargeArea.transform.localPosition = new Vector2(chargeArea.transform.localPosition.x * -1, chargeArea.transform.localPosition.y);
        chargeBar.transform.localPosition = new Vector2(chargeBar.transform.localPosition.x * -1, chargeBar.transform.localPosition.y);
    }

    // Local에서 점프 후 동기화 시 랙이 심하여 RPC에서 점프.
    [PunRPC]
    public void JumpRPC()
    {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x * 0.75f, 0f);

        float distance1 = Vector2.Distance(new Vector2(particleManager.particle_Up1.transform.position.x, 0), new Vector2(transform.position.x, 0));
        float distance2 = Vector2.Distance(new Vector2(particleManager.particle_Up2.transform.position.x, 0), new Vector2(transform.position.x, 0));

        if ( (distance1 <= 3 || distance2 <= 3 ) && transform.position.y <= 9f)
            rigidbody.AddForce(Vector2.up * 50f, ForceMode2D.Impulse);
        else
            rigidbody.AddForce(Vector2.up * 30f, ForceMode2D.Impulse);
    }

    // 발사 각도 동기화.
    [PunRPC]
    public void Move_TurretRPC(float _deg)
    {
        float rad = _deg * Mathf.Deg2Rad;
        turret.transform.localPosition = new Vector2(spriteRenderer.flipX ? Mathf.Cos(rad) * -1 : Mathf.Cos(rad), Mathf.Sin(rad));
        turret.transform.eulerAngles = new Vector3(0, turret.transform.eulerAngles.y, _deg);
    }

    // 발사 거리 동기화.
    [PunRPC]
    public void ChargeRPC()
    {
        if (!chargeArea.gameObject.activeSelf)
        {
            chargeArea.gameObject.SetActive(true);
            chargeBar.gameObject.SetActive(true);
        }

        if (chargeBar.fillAmount <= 0.5f)
            chargeState = true;
        else if (chargeBar.fillAmount == 1)
            chargeState = false;

        if (chargeState)
            chargeBar.fillAmount += Time.fixedDeltaTime * 1.5f;
        else
            chargeBar.fillAmount -= Time.fixedDeltaTime * 1.5f;
    }

    // 미사일 발사 로직.
    [PunRPC]
    public void ShootingRPC(float charge, int viewId)
    {
        for (int i = 0; i <= attackItem; i++)
        {
            for (int j = 0; j < turret.transform.childCount; j++)
            {
                if (turret.transform.GetChild(j).gameObject.activeSelf)
                {
                    Transform arrow = turret.transform.GetChild(1);
                    Bullet _bullet = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.Bullet_1, 
                            arrow.position, Quaternion.identity).GetComponent<Bullet>();
                    _bullet.particleManager = particleManager;
                    _bullet.speed = _bullet.startingSpeed * charge;
                    _bullet.StartCoroutine(_bullet.DestroyBullet());
                    _bullet.bulletRigidbody.velocity = new Vector3(turret.transform.localPosition.x, 
                            turret.transform.localPosition.y + arrow.localPosition.y, 0) * _bullet.speed;
                    
                    if (j == 1 && gameManager.playerManager.myPlayerObject.PV.ViewID == viewId)
                        playerCamera.ChangeTarget(_bullet.gameObject);
                }
            }
        }

        attackItem = 0;
        isCharging = false;
        chargeBar.fillAmount = 0;
        chargeBar.gameObject.SetActive(false);
        chargeArea.gameObject.SetActive(false);
    }

    // 빙결(무적) 여부 변경.
    [PunRPC]
    public void ChangeFreezingRPC() => freezingImage.gameObject.SetActive(!freezingImage.gameObject.activeSelf);
#endregion

/*** 플레이어 캐릭터 세팅 함수 모음. ***/
#region SETTING
    // gameManager 변수 초기화.
    void gameManagerSetting()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        characters = gameManager.characters;
        playerSound = gameManager.soundManager;
        playerCamera = gameManager.playerCamera;
        playerManager = gameManager.playerManager;
        particleManager = gameManager.particleManager;
        playerController = gameManager.playerController;
        gameMove = gameManager.photonObject.roomType;
    }

    // 방 입장 시 선택한 캐릭터로 설정하는 RPC 호출.
    IEnumerator SettingMyCharacterType()
    {
        yield return new WaitForSeconds(0.25f);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != this.gameObject)
                playerManager.listPlayerObjects.Add(player.GetComponent<PlayerObject>());
        }

        Debug.Log($"@@@@@ :: {gameManager.shopManager.characterImages.Length}, {characters.myCharacterType}");
        for (int i = 0; i < gameManager.shopManager.characterImages.Length; i++)
        {
            Debug.Log($"@@@@@ :: {gameManager.shopManager.characterImages[i].sprite == null}");
            gameManager.shopManager.characterImages[i].sprite = characters.characterInfo[characters.myCharacterType].sprites[0];
        }

        PV.RPC("SettingMyCharacterTypeRPC", RpcTarget.All, characters.myCharacterType);
    }

    // 방 입장 시 선택한 캐릭터로 설정.
    [PunRPC]
    public void SettingMyCharacterTypeRPC(int myCharacterType) => StartCoroutine(SettingCounter(myCharacterType));

    // 동시에 입장 시 오류가 발생하여 Coroutine으로 해결.
    IEnumerator SettingCounter(int myCharacterType)
    {
        while (true)
        {
            if (playerManager == null)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }

            characterType = myCharacterType;
            playerManager.AddPlayerObject(this, PV.IsMine);
            break;
        }
    }

    // 게임 시작 시 선택한 캐릭터에 맞춰 능력치를 설정하는 RPC 호출.
    public IEnumerator PushMyCharacter(int myCharacterType) 
    {   
        yield return new WaitForSeconds(0.3f);
        PV.RPC("PushMyCharacterRPC", RpcTarget.All, myCharacterType);
    }
    
    // 게임 시작 시 선택한 캐릭터에 맞춰 능력치를 설정.
    [PunRPC]
    public void PushMyCharacterRPC(int myCharacterType)
    {
        characterType = myCharacterType;
        turret.SetActive(true);
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        rigidbody.mass = 1f;
        rigidbody.gravityScale = 10f;
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        switch (myCharacterType)
        {
            case 0:
                spriteRenderer.sprite = characters.characterInfo[0].sprites[0];
                defence += 0.5f;
                speed += 0.5f;
                break;
            case 1:
                spriteRenderer.sprite = characters.characterInfo[1].sprites[0];
                defence += 1f;
                break;
            case 2:
                spriteRenderer.sprite = characters.characterInfo[2].sprites[0];
                speed += 2f;
                break;
            case 3:
                spriteRenderer.sprite = characters.characterInfo[3].sprites[0];
                money += 500;
                break;
        }

        if (PV.IsMine)
        {
            playerCamera.player = playerCamera.target = gameObject;
            playerController.moveBarHandleImage.sprite = spriteRenderer.sprite;

            for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
            {
                if (playerManager.listPlayerObjects[i] == this)
                {
                    switch (i)
                    {
                        case 0:
                            transform.position = new Vector3(-40, 2.5f, -1);
                            break;
                        case 1:
                            transform.position = new Vector3(-35, 2.5f, -1);
                            break;
                        case 2:
                            transform.position = new Vector3(-15, 2.5f, -1);
                            break;
                        case 3:
                            transform.position = new Vector3(15, 2.5f, -1);
                            break;
                    }
                }
            }
        }
    }
#endregion

/*** 상점창 함수 모음. ***/
#region SHOP
    // 터치한 UpGrade와 등급에 맞춰 RPC 호출.
    public void UpGrade(int upgradeType, int upgradeLevel)
    {
        switch (upgradeType)
        {
            case 0:
                PV.RPC("UpGradeType0", RpcTarget.All, upgradeLevel);
                break;
            case 1:
                PV.RPC("UpGradeType1", RpcTarget.All, upgradeLevel);
                break;
            case 2:
                PV.RPC("UpGradeType2", RpcTarget.All);
                break;
            case 3:
                PV.RPC("UpGradeType3", RpcTarget.All, 0.3f);
                break;
        }
    }

    // 탱크 강화.
    [PunRPC]
    public void UpGradeType0(int upgradeLevel)
    {
        spriteRenderer.sprite = characters.characterInfo[characterType].sprites[upgradeLevel + 1];
        transform.localScale += new Vector3(0.2f, 0.2f, 0);
        defence += 0.2f;
        speed += 0.5f;
    }

    // 공격 강화.
    [PunRPC]
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

    // 기동성 강화.
    [PunRPC]
    public void UpGradeType2() => speed += 1.5f;

    // 회복.
    [PunRPC]
    public void UpGradeType3(float healing) => hpBar.fillAmount += healing;
#endregion

/*** ITEM 함수 모음. ***/
#region  ITEM
    // 습득한 ITEM에 맞춰 RPC 호출.
    public void GetItem(int itemType)
    {
        if (!PV.IsMine)
            return;

        switch (itemType)
        {
            case 0:
                PV.RPC("ItemType0", RpcTarget.All);
                break;
            case 1:
                PV.RPC("ItemType1", RpcTarget.All);
                break;
            case 2:
                PV.RPC("ItemType2", RpcTarget.All);
                break;
        }

        playerSound.itemGet.Play();
    }

    // 공격 강화 ITEM.
    [PunRPC]
    public void ItemType0() 
    {
        attackItem++;
        Destroy(Instantiate(itemRootingEffect, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity), 2);
    }

    // Shield ITEM.
    [PunRPC]
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
    [PunRPC]
    public void ItemType2() 
    {
        money += 300;
        Destroy(Instantiate(itemRootingEffect, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity), 2);
    }
#endregion

/*** 충돌 및 접촉 함수 모음. ***/
#region Collision&Trigger
    // 미사일이나 Monster 공격에 접촉시 피격 판정 함수 호출.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (freezingImage.gameObject.activeSelf)
            return;

        if (other.CompareTag("Explosion") && PV.IsMine)
        {
            if (gameMove == "PVP")
                PV.RPC("hitRPC", RpcTarget.All, 70f, PV.ViewID);
            else if (gameMove == "PVE")
                PV.RPC("hitRPC", RpcTarget.All, 10f, PV.ViewID);
        }

        if (other.CompareTag("MonsterSkill") && PV.IsMine)
        {
            playerSound.monsterSkill.Play();
            PV.RPC("hitRPC", RpcTarget.All, 15f * other.gameObject.GetComponent<MonsterSkill>().power, PV.ViewID);
        }

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
        if (freezingImage.gameObject.activeSelf)
            return;

        if (other.name == "Light")
            PV.RPC("UpGradeType3", RpcTarget.All, 0.01f);
    }

    // 나가기 버튼 터치시 캐릭터 사망 처리.
    public void DestroyCharacter() => PV.RPC("hitRPC", RpcTarget.All, 9999f, PV.ViewID);
    
    // 캐릭터 피격 및 사망 로직.
    [PunRPC]
    public void hitRPC(float damage, int viewID)
    {
        damage /= defence;
        if (shieldItem)
        {
            shield.HP -= (damage / 25);

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
            Destroy(GetComponent<BoxCollider2D>());
            playerManager.RemovePlayerObject(this, gameManager.playerManager.myPlayerObject.PV.ViewID == viewID);
        }
    }
#endregion

}
