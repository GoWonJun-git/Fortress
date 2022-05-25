using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager_Tutorial : MonoBehaviour
{
    public TutorialManager tutorialManager;
    public TutorialPlayerObject player;
    public GameObject shopPanel;
    public ItemInfo[] itemInfo;
    public Text playerMouny1;
    public Text playerMouny2;
    public Sprite checkImage;
    public Sprite unCheckImage;
    public Sprite[] tankImages;

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
        if (!tutorialManager.isGameStart)
            return;
        
        shopPanel.SetActive(!shopPanel.activeSelf);
        tutorialManager.soundManager.buttonTouch.Play();
    }

    // 상점, 옵션창의 캐릭터 상태, 골드 동기화.
    void Update()
    {
        playerMouny1.text = playerMouny2.text = player.money.ToString();

        if (!shopPanel.activeSelf)
            return;

        for (int i = 0; i < hpBars.Length; i++) hpBars[i].fillAmount = player.hpBar.fillAmount;
        
        for (int i = 0; i < characterImages.Length; i++) characterImages[i].sprite = player.spriteRenderer.sprite;
        
        for (int i = 0; i < arrows1.Length; i++)
        {
            arrows1[i].SetActive(player.turret.transform.GetChild(i).gameObject.activeSelf);
            arrows2[i].SetActive(arrows1[i].activeSelf);
        }
    }

    // 업그레이드 버튼 클릭 시.
    public void UpgradeButtonClick(int btnNum)
    {
        if (btnNum != 3 &&
            itemInfo[btnNum].upgradedCheck < itemInfo[btnNum].price.Length && 
            itemInfo[btnNum].price[itemInfo[btnNum].upgradedCheck] <= player.money)
        {
            itemInfo[btnNum].upgradedIcon[itemInfo[btnNum].upgradedCheck].sprite = checkImage;
            player.money -= itemInfo[btnNum].price[itemInfo[btnNum].upgradedCheck];
            playerMouny1.text = player.money.ToString();
            player.UpGrade(btnNum, itemInfo[btnNum].upgradedCheck);
            tutorialManager.soundManager.buttonTouch.Play();

            if (++itemInfo[btnNum].upgradedCheck == itemInfo[btnNum].price.Length)
                itemInfo[btnNum].priceText.text = "None";
            else
                itemInfo[btnNum].priceText.text = itemInfo[btnNum].price[itemInfo[btnNum].upgradedCheck].ToString();
        }
        else if (btnNum == 3 &&
                 itemInfo[btnNum].upgradedCheck < itemInfo[btnNum].price.Length && 
                 itemInfo[btnNum].price[0] <= player.money)
        {
            itemInfo[btnNum].upgradedIcon[0].sprite = checkImage;
            player.money -= itemInfo[btnNum].price[0];
            playerMouny1.text = player.money.ToString();
            itemInfo[btnNum].priceText.text = (itemInfo[btnNum].price[0] += 200).ToString();
            player.UpGrade(btnNum, itemInfo[btnNum].upgradedCheck++);
            tutorialManager.soundManager.buttonTouch.Play();
        }
        else
            tutorialManager.soundManager.buttonFail.Play();
    }

}