using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Characters : MonoBehaviour
{
    public CharacterInfo[] characterInfo;
    public GameObject PVPButton;
    public GameObject PVEButton;
    public SoundManager soundManager;

    [Header("SelectCharacter")]
    public Image tankImage;
    public Text tankName;
    public GameObject graph;
    public Image[] graphValue;
    public int myCharacterType;

    void Start() => DontDestroyOnLoad(this);
    

    void Update()
    {
        if (graph != null && graph.activeSelf && myCharacterType >= 0)
        {
            for (int i = 0; i < graphValue.Length;i ++) 
                graphValue[i].fillAmount = characterInfo[myCharacterType].graphValue[i];
        }

        if (SceneManager.GetActiveScene().buildIndex.Equals(1) && PVPButton == null)
            Destroy(gameObject);
    }
    
    // 탱크 선택 시 해당 탱크 정보 출력.
    public void SelectButtonClick(int btnNum)
    {
        if (!graph.activeSelf)
        {
            graph.SetActive(true);
            tankImage.transform.localPosition = new Vector2(-125, 10);
            tankName.transform.localPosition = new Vector2(-140, -100);
        }
        tankImage.sprite = characterInfo[btnNum].sprites[0];
        tankName.text = characterInfo[btnNum].name;

        myCharacterType = btnNum;
        soundManager.buttonTouch.Play();

        PVPButton.SetActive(true);
        PVEButton.SetActive(true);
    }

}
