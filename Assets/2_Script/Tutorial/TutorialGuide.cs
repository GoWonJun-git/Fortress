using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGuide : MonoBehaviour
{
    public TutorialManager tutorialManager;
    public TutorialPlayerObject player;
    public Text guideText;
    public int guideLevel;
    public Image guideArrow;
    public string[] guideStrings;
    public Vector2[] guideArrowPositions;
    public int[] guideArrowRotations;
    
    void Start() => guideLevel = 0;

    // 다음 Guide로 안내.
    public void NextGuide()
    {
        guideText.fontSize = 35;
        tutorialManager.soundManager.buttonTouch.Play();

        if (guideLevel <= guideStrings.Length - 1)
        {
            guideText.text = guideStrings[guideLevel].Replace("\\n", "\n");
            guideArrow.GetComponent<RectTransform>().anchoredPosition = guideArrowPositions[guideLevel];
            guideArrow.transform.localEulerAngles = new Vector3(0, 0, guideArrowRotations[guideLevel]);

            if (++guideLevel == 11)
            {
                Item[] items = new Item[3];
                items[0] = Instantiate(Resources.Load<GameObject>("Item/AttackItem"), new Vector3(player.transform.position.x - 7, 15, 0), Quaternion.Euler(180, 0, 0)).GetComponent<Item>();
                items[1] = Instantiate(Resources.Load<GameObject>("Item/ShieldItem"), new Vector3(player.transform.position.x, 15, 0), Quaternion.Euler(180, 0, 0)).GetComponent<Item>();
                items[2] = Instantiate(Resources.Load<GameObject>("Item/MoneyItem"), new Vector3(player.transform.position.x + 7, 15, 0), Quaternion.Euler(180, 0, 0)).GetComponent<Item>();
                for (int i = 0; i < items.Length; i++) items[i].StartCoroutine(items[i].DestroyItem());
            }
            else if (guideLevel == 12)
            {
                Monster dragon1 = Instantiate(Resources.Load<Monster>("Monster/Dragon"), 
                    player.transform.position + new Vector3(10, 3, 0), Quaternion.Euler(0, 180, 0));
                dragon1.target = player.gameObject;
                dragon1.monsterType = -1;
                dragon1.StartCoroutine(dragon1.Attack1());

                Monster dragon2 = Instantiate(Resources.Load<Monster>("Monster/Dragon"), 
                    player.transform.position + new Vector3(-10, 3, 0), Quaternion.Euler(0, 180, 0));
                dragon2.target = player.gameObject;
                dragon2.monsterType = -1;
                dragon2.StartCoroutine(dragon2.Attack1());
            }
        }
    }
    
}
