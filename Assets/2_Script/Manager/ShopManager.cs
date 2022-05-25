using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerManager playerManager;
    public GameObject shopPanel;
    public ItemInfo[] itemInfo;
    public Text playerMouny1;
    public Text playerMouny2;
    public Sprite checkImage;
    public Sprite unCheckImage;

    [Header("MyCharacter")]
    public Image[] characterImages;
    public Image[] hpBars;
    public GameObject[] arrows1;
    public GameObject[] arrows2;

    void Start()
    {
        for (int i = 0; i < itemInfo.Length; i++) itemInfo[i].priceText.text = itemInfo[i].price[0].ToString();
    }

    // 상점창 활성화 여부 변경.
    public void ChangeShopActive()
    {
        if (!gameManager.isGameStart)
            return;
        
        shopPanel.SetActive(!shopPanel.activeSelf);
        gameManager.soundManager.buttonTouch.Play();
    }

    // 상점, 옵션창의 캐릭터 상태, 골드 동기화.
    void Update()
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd || playerManager.myPlayerObject == null)
            return;

        playerMouny1.text = playerMouny2.text = playerManager.myPlayerObject.money.ToString();

        for (int i = 0; i < hpBars.Length; i++) hpBars[i].fillAmount = playerManager.myPlayerObject.hpBar.fillAmount;
        
        for (int i = 0; i < characterImages.Length; i++) characterImages[i].sprite = playerManager.myPlayerObject.spriteRenderer.sprite;
        
        for (int i = 0; i < arrows1.Length; i++)
        {
            arrows1[i].SetActive(playerManager.myPlayerObject.turret.transform.GetChild(i).gameObject.activeSelf);
            arrows2[i].SetActive(arrows1[i].activeSelf);
        }
    }

    // 업그레이드 버튼 클릭 시.
    public void UpgradeButtonClick(int btnNum)
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd)
            return;

        // 업그레이드 종류가 회복이 아니고, 업그레이드 수치가 남아있고, 돈이 충분한 경우.
        if (btnNum != 3 &&
            itemInfo[btnNum].upgradedCheck < itemInfo[btnNum].price.Length && 
            itemInfo[btnNum].price[itemInfo[btnNum].upgradedCheck] <= playerManager.myPlayerObject.money)
        {
            itemInfo[btnNum].upgradedIcon[itemInfo[btnNum].upgradedCheck].sprite = checkImage;
            playerManager.myPlayerObject.money -= itemInfo[btnNum].price[itemInfo[btnNum].upgradedCheck];
            playerMouny1.text = playerManager.myPlayerObject.money.ToString();
            playerManager.myPlayerObject.UpGrade(btnNum, itemInfo[btnNum].upgradedCheck);
            gameManager.soundManager.buttonTouch.Play();

            // 최종 업그레이드 여부 구분.
            if (++itemInfo[btnNum].upgradedCheck == itemInfo[btnNum].price.Length)
                itemInfo[btnNum].priceText.text = "None";
            else
                itemInfo[btnNum].priceText.text = itemInfo[btnNum].price[itemInfo[btnNum].upgradedCheck].ToString();
        }
        // 업그레이드 종류가 회복고, 업그레이드 수치가 남아있고, 돈이 충분한 경우.
        else if (btnNum == 3 &&
                 itemInfo[btnNum].upgradedCheck < itemInfo[btnNum].price.Length && 
                 itemInfo[btnNum].price[0] <= playerManager.myPlayerObject.money)
        {
            itemInfo[btnNum].upgradedIcon[0].sprite = checkImage;
            playerManager.myPlayerObject.money -= itemInfo[btnNum].price[0];
            playerMouny1.text = playerManager.myPlayerObject.money.ToString();
            itemInfo[btnNum].priceText.text = (itemInfo[btnNum].price[0] += 200).ToString();
            playerManager.myPlayerObject.UpGrade(btnNum, itemInfo[btnNum].upgradedCheck++);
            gameManager.soundManager.buttonTouch.Play();
        }
        else
            gameManager.soundManager.buttonFail.Play();
    }

}