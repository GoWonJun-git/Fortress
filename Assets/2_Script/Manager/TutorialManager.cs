using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class TutorialManager : MonoBehaviour
{
    public int waveCount;
    public bool isGameStart;
    public TutorialPlayerObject player;

    [Header("Script")]
    
    public ItemManager itemManager;
    public SoundManager soundManager;
    public CameraManager playerCamera;
    public TutorialGuide tutorialGuide;
    public MonsterManager monsterManager;
    public ParticleManager particleManager;
    public ShopManager_Tutorial shopManager;
    public PlayerController_Tutorial playerController;
    public TerrainGenerator_Tutorial terrainGenerator_1;
    public TerrainGenerator_Tutorial terrainGenerator_2;

    [Header("UI")]
    public Image waveBar;
    public GameObject optionPanel;
    public GameObject gameStartButton;    

    // 플레이어와 맵 생성 + 오브젝트 풀링.
    void Start()
    {
        ResourceDataManager.LoadResourcesData();
        InitObjectPool();

        terrainGenerator_1.GenerateTerrain();
        terrainGenerator_2.GenerateTerrain();
    }

    // 게임 시작 시 Wave 관리(Wave 변화는 없음)
    void Update()
    {
        if (isGameStart)
            waveBar.fillAmount += Time.deltaTime / 15;

        if (waveBar.fillAmount >= 1f)
        {
            waveCount++;
            waveBar.fillAmount = 0f;
        }
    }

    // 오브젝트 풀링.
    public void InitObjectPool()
    {
        ObjectPoolManager objectPoolManager = ObjectPoolManager.Instance;
        objectPoolManager.CreatePool(ResourceDataManager.Bullet_1, 30, 15);
    }

    // 옵션 버튼 클릭 시.
    public void ChangeOptionPanelActive() 
    {
        optionPanel.SetActive(!optionPanel.activeSelf);
        soundManager.buttonTouch.Play();
    }

    // 게임 시작 버튼 클릭 시.
    public void GameStart() 
    {
        isGameStart = true;
        gameStartButton.GetComponent<Animator>().SetTrigger("Start");
        
        playerCamera.player = playerCamera.target = player.gameObject;
        particleManager.particle_Wind.SetActive(true);
        particleManager.particle_Light.SetActive(true);
        particleManager.particle_Up1.SetActive(true);
        particleManager.particle_Up2.SetActive(true);

        player.rigidbody.bodyType = RigidbodyType2D.Dynamic;
        player.rigidbody.mass = 1f;
        player.rigidbody.gravityScale = 10f;
        player.rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        tutorialGuide.NextGuide();
        gameStartButton.transform.GetChild(1).gameObject.SetActive(true);
        gameStartButton.transform.GetChild(2).gameObject.SetActive(true);

        soundManager.buttonTouch.Play();
    }

    // 게임 종료 버튼 클릭 시.
    public void GameExit() 
    {   soundManager.buttonTouch.Play();
        Destroy(GameObject.Find("SelectCharacter"));
        SceneManager.LoadScene(1);
    }
    
    // 버튼 비활성화.
    public void Disable(Button btn) => btn.gameObject.SetActive(false);

}