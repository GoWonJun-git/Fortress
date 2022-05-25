using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

enum particleSystemTransform
{
    wind_MinX = -60,
    wind_ManX = 5,
    light_MinX = -45,
    light_MaxX = -6 
}

public class GameManager : MonoBehaviour
{
    public PhotonView PV;
    public int waveCount;
    public bool isGameStart;
    public bool isGameEnd;
    private bool isExit;

    [Header("Script")]
    public Characters characters;
    public ShopManager shopManager;
    public ItemManager itemManager;
    public PhotonObject photonObject;
    public SoundManager soundManager;
    public CameraManager playerCamera;
    public PlayerManager playerManager;
    public MonsterManager monsterManager;
    public ParticleManager particleManager;
    public PlayerController playerController;
    public TerrainGenerator terrainGenerator_1;
    public TerrainGenerator terrainGenerator_2;

    [Header("UI")]
    public Image waveBar;
    public GameObject dieIcon;
    public GameObject waitImage;
    public GameObject warningBox;
    public GameObject optionPanel;
    public GameObject playerDiePanel;
    public GameObject playerWinPanel;
    public GameObject gameStartButton;

    // 플레이어 생성 + 오브젝트 풀링.
    void Start()
    {
        ResourceDataManager.LoadResourcesData();
        InitObjectPool();

        playerManager = PlayerManager.Instance;
        playerManager.StartCoroutine(playerManager.CreatePlayerObject());
        characters = GameObject.Find("SelectCharacter").GetComponent<Characters>();
        photonObject = GameObject.Find("PhotonObject").GetComponent<PhotonObject>();
        photonObject.gameManager = this;
    }

    // 게임 시작 시 Wave 관리.
    void Update()
    {
        if (isGameStart)
        {
            if(photonObject.roomType == "PVE")
            {
                if (waveCount == 1)
                    waveBar.fillAmount += Time.deltaTime / 10;
                else
                     waveBar.fillAmount += Time.deltaTime / 30;
            }
            else
                waveBar.fillAmount += Time.deltaTime / 20;

            if (waveBar.fillAmount >= 1f)
            {
                ++waveCount;
                waveBar.fillAmount = 0f;

                if (PhotonNetwork.IsMasterClient)
                    PV.RPC("Synchronization_Wave", RpcTarget.All, waveCount);
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                gameStartButton.SetActive(true);
                waitImage.SetActive(false);
            }
            else
            {
                gameStartButton.SetActive(false);
                waitImage.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            OptionButtonClick();
    }

    // Wave 동기화.
    [PunRPC]
    public void Synchronization_Wave(int _waveCount) 
    {
        waveCount = _waveCount;
        NextWaveLogic(waveCount);
    }

    // 오브젝트 풀링.
    public void InitObjectPool()
    {
        ObjectPoolManager objectPoolManager = ObjectPoolManager.Instance;

        objectPoolManager.CreatePool(ResourceDataManager.Bullet_1, 30, 15);
        
        objectPoolManager.CreatePool(ResourceDataManager.AttackItem, 20, 5);
        objectPoolManager.CreatePool(ResourceDataManager.ShieldItem, 20, 5);
        objectPoolManager.CreatePool(ResourceDataManager.MoneyItem, 20, 5);

        objectPoolManager.CreatePool(ResourceDataManager.Dragon, 20, 5);
        objectPoolManager.CreatePool(ResourceDataManager.EvilMage, 20, 5);
        objectPoolManager.CreatePool(ResourceDataManager.Golem, 10, 5);
    }

    // 게임 시작 버튼 클릭 시.
    public void GameStartButtonClick() 
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && photonObject.roomType == "PVP")
        {
            GameObject _warningBox = Instantiate(warningBox);
            _warningBox.transform.parent = GameObject.Find("Canvas").transform;
            _warningBox.transform.localPosition = Vector3.zero;
            Destroy(_warningBox, 2f);
            return;
        }

        soundManager.buttonTouch.Play();
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PV.RPC("GameStartLogic", RpcTarget.All);
    }

    // 게임 시작 로직.
    [PunRPC]
    public void GameStartLogic()
    {
        waveCount++;
        isGameStart = true;
        terrainGenerator_1.StartCoroutine(terrainGenerator_1.GenerateTerrain());
        terrainGenerator_2.StartCoroutine(terrainGenerator_2.GenerateTerrain());
        particleManager.particle_Wind.SetActive(true);
        particleManager.particle_Light.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < playerManager.listPlayerObjects.Count; i++) 
                playerManager.listPlayerObjects[i].StartCoroutine(
                    playerManager.listPlayerObjects[i].PushMyCharacter(
                        playerManager.listPlayerObjects[i].characterType));
        }

        waitImage.SetActive(false);
        gameStartButton.SetActive(false);
    }

    // 게임 나가기 버튼 클릭 시.
    public void ExitButtonClick() 
    {
        isExit = true;
        Destroy(GameObject.Find("SelectCharacter"));

        if (playerManager.myPlayerObject != null)
            playerManager.myPlayerObject.DestroyCharacter();

        StartCoroutine(ExitScene());
    }
    IEnumerator ExitScene()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(characters);
        photonObject.OutRoom();
        SceneManager.LoadScene(1);
        soundManager.buttonTouch.Play();
    }

    // 게임 대기 버튼 클릭 시.
    public void WaitButtonClick() 
    {
        playerDiePanel.SetActive(false);
        soundManager.buttonTouch.Play();

        foreach (PlayerObject player in playerManager.listPlayerObjects)
        {
            if (player.hpBar.fillAmount > 0f)
            {
                playerCamera.target = playerCamera.player = player.gameObject;
                break;
            }
        }
    }

    // 다른 플레이어 보기 버튼 클릭 시.
    public void WatchNextPlayerButtonClick()
    {
        soundManager.buttonTouch.Play();

        int idx = playerManager.listPlayerObjects.IndexOf(playerCamera.player.GetComponent<PlayerObject>());
        if (++idx > playerManager.listPlayerObjects.Count - 1)
            idx = 0;
        playerCamera.target = playerCamera.player = playerManager.listPlayerObjects[idx].gameObject;
    }

    // 옵션 버튼 클릭 시.
    public void OptionButtonClick() 
    {
        soundManager.buttonTouch.Play();
        optionPanel.SetActive(!optionPanel.activeSelf);
    }

    // 버튼 및 판넬 비활성화.
    public void Disable(GameObject disableObject) 
    {
        soundManager.buttonTouch.Play();
        disableObject.SetActive(false);
    }

    // 게임 패배 시(플레이어가 사망한 경우, 나가기 버튼을 클릭한 경우)
    public IEnumerator PlayerDie()
    {
        if (!playerWinPanel.activeSelf)
        {
            isGameEnd = true;

            yield return new WaitForSeconds(0.2f);
            if (!isExit)
            {
                dieIcon.SetActive(true);
                playerDiePanel.SetActive(true);

                if (playerManager.listPlayerObjects.Count <= 1)
                {
                    playerDiePanel.transform.GetChild(1).localPosition = new Vector2(0, -130);
                    playerDiePanel.transform.GetChild(2).gameObject.SetActive(false);
                }
            }

            PV.RPC("PlayerDie_RPC", RpcTarget.All);
        }
    }

    // 플레이어의 사망 구분.
    [PunRPC]
    public void PlayerDie_RPC()
    {
        if (!isGameStart || isExit)
            return;
        Debug.Log(playerManager.listPlayerObjects.Count);
        if (photonObject.roomType == "PVP" && playerManager.myPlayerObject != null && playerManager.listPlayerObjects.Count <= 1)
            GameWin();
        else if (photonObject.roomType == "PVP" && playerManager.myPlayerObject == null && playerManager.listPlayerObjects.Count <= 1)
        {
            playerDiePanel.SetActive(true);
            playerDiePanel.transform.GetChild(1).localPosition = new Vector2(0, -130);
            playerDiePanel.transform.GetChild(2).gameObject.SetActive(false);
        }
        else if (photonObject.roomType == "PVE" && playerManager.myPlayerObject == null && playerManager.listPlayerObjects.Count <= 0)
        {
            playerDiePanel.SetActive(true);
            playerDiePanel.transform.GetChild(1).localPosition = new Vector2(0, -130);
            playerDiePanel.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    // 게임 승리 시.
    public void GameWin() 
    {
        playerDiePanel.SetActive(false);
        playerWinPanel.SetActive(true);
    }

    // Wave 진행 로직.
    public void NextWaveLogic(int wave)
    {
        if (PhotonNetwork.IsMasterClient)
            particleManager.ChangeParticlePosition(wave);

        if (PhotonNetwork.IsMasterClient && wave >= 2)
            itemManager.DropItem();

        if (PhotonNetwork.IsMasterClient && wave >= 2 && photonObject.roomType == "PVE")
            monsterManager.CreateMonster(wave);

        if (playerManager.myPlayerObject != null)
        {
            playerManager.myPlayerObject.money += 150;

            if (shopManager.itemInfo[3].upgradedCheck >= 1)
            {
                shopManager.itemInfo[3].upgradedIcon[0].sprite = shopManager.unCheckImage;
                shopManager.itemInfo[3].upgradedCheck--;
            }
        }
    }

}
